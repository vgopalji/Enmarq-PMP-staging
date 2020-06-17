using CareStream.LoggerService;
using CareStream.Models;
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CareStream.Utility
{
    public interface IGroupOwnerService
    {
        Task<GroupOwnerModel> GetGroupOwners(string id);

        Task AddGroupOwners(GroupOwnerAssignModel groupOwnerAssign);

        Task RemoveGroupOwners(GroupOwnerAssignModel val);
    }

    public class GroupOwnerService : IGroupOwnerService
    {
        private readonly ILoggerManager _logger;

        public GroupOwnerService(ILoggerManager logger)
        {
            _logger = logger;
        }

        public async Task<GroupOwnerModel> GetGroupOwners(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id) || string.IsNullOrWhiteSpace(id))
                {
                    _logger.LogError("GroupOwnerService-GetGroupOwners: Id cannot be empty");
                    return null;
                }

                var groupOwnerModel = new GroupOwnerModel
                {
                    GroupId = id
                };

                _logger.LogInfo($"GroupOwnerService-GetGroupOwners: [Started] to get member detail for group id {id} from Azure AD B2C");

                var client = GraphClientUtility.GetGraphServiceClient();

                if (client == null)
                {
                    _logger.LogError("GroupOwnerService-GetGroupOwners: Unable to create object for graph client");
                    return null;
                }

                groupOwnerModel = await GetGroupOwnerAsync(id);

                _logger.LogInfo($"GroupOwnerService-GetGroupOwners: [Completed] to getting member detail for group id {id} from Azure AD B2C");
                return groupOwnerModel;

            }
            catch (ServiceException ex)
            {
                _logger.LogError("GroupOwnerService-GetGroupOwners: Exception occured....");
                _logger.LogError(ex);
                throw ex;
            }
        }

        public async Task AddGroupOwners(GroupOwnerAssignModel groupOwnerAssign)
        {
            try
            {
                if (groupOwnerAssign == null)
                {
                    _logger.LogError("GroupOwnerService-AddGroupOwners: Group owner cannot be null...");
                    return;
                }

                _logger.LogInfo("GroupOwnerService-AddGroupOwners: [Started] assigning owner(s) to group in Azure AD B2C");

                if (!string.IsNullOrEmpty(groupOwnerAssign.GroupId) && groupOwnerAssign.SelectedOwners.Any())
                {
                    var client = GraphClientUtility.GetGraphServiceClient();
                    if (client == null)
                    {
                        _logger.LogError("GroupOwnerService-AddGroupOwners: Unable to create object for graph client ");
                        return;
                    }

                    // Get all user from Graph
                    var users = await client.Users.Request().GetAsync();
                    if (users != null)
                    {
                        var assigningUsers = users.Where(x => groupOwnerAssign.SelectedOwners.Contains(x.Id)).ToList();
                        if (assigningUsers != null)
                        {
                            foreach (var assignUser in assigningUsers)
                            {
                                try
                                {
                                    await client.Groups[groupOwnerAssign.GroupId].Owners.References.Request().AddAsync(assignUser);
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError($"GroupOwnerService-AddGroupOwners: unable to assign owner [{assignUser.UserPrincipalName}] to the group [id:{groupOwnerAssign.GroupId}]");
                                    _logger.LogError(ex);
                                }
                            }
                        }
                    }
                }

                _logger.LogInfo("GroupOwnerService-AddGroupOwners: [Completed] assigning owner(s) to group in Azure AD B2C");

            }
            catch (Exception ex)
            {
                _logger.LogError("GroupMemberService-AddGroupOwners: Exception occured....");
                _logger.LogError(ex);
            }
        }

        public async Task RemoveGroupOwners(GroupOwnerAssignModel val)
        {
            try
            {
                if (val == null)
                {
                    _logger.LogError("GroupOwnerService-RemoveGroupOwners: Input value cannot be empty");
                    return;
                }

                var groupId = val.GroupId;
                if (!string.IsNullOrEmpty(groupId))
                {
                    GraphServiceClient client = GraphClientUtility.GetGraphServiceClient();
                    if (client == null)
                    {
                        _logger.LogError("GroupOwnerService-RemoveGroupOwners: Unable to create object for graph client");
                        return;
                    }

                    _logger.LogInfo($"GroupOwnerService-RemoveGroupOwners: [Started] unassigning owner(s) for group [Id:{groupId}]");

                    foreach (var ownerId in val.SelectedOwners)
                    {
                        try
                        {
                            _logger.LogInfo($"GroupOwnerService-RemoveGroupOwners: Removing owner [{ownerId}] from group [Id:{groupId}] on Azure AD B2C");

                            await client.Groups[groupId].Owners[ownerId].Reference.Request().DeleteAsync();

                            _logger.LogInfo($"GroupOwnerService-RemoveGroupOwners: Removed member [{ownerId}] from group [Id:{groupId}] on Azure AD B2C");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"GroupOwnerService-RemoveGroupOwners: Exception occured while unassigning owner [Id:{ownerId}] for group  [Id:{groupId}] ");
                            _logger.LogError(ex);
                        }
                    }

                    _logger.LogInfo($"GroupOwnerService-RemoveGroupOwners: [Completed] unassigning owner(s) from group [Id:{val.GroupId}]");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError("GroupOwnersController-RemoveGroupOwners: Exception occured....");
                _logger.LogError(ex);
                throw ex;
            }
        }

        private async Task<GroupOwnerModel> GetGroupOwnerAsync(string groupId)
        {
            GroupOwnerModel retVal = null;
            try
            {
                if (string.IsNullOrEmpty(groupId))
                {
                    _logger.LogError("GroupOwnerService-GetGroupOwnerAsync: Group Id cannot be null");
                    return retVal;

                }
                _logger.LogInfo("GroupOwnerService-GetGroupOwnerAsync: Starting to get group owner from Azure AD B2C");


                retVal = new GroupOwnerModel
                {
                    GroupId = groupId
                };

                var _graphServiceClient = GraphClientUtility.GetGraphServiceClient();

                var tasks = new Task[]
                {
                    Task.Run(async() => {
                        Dictionary<string, string> ownersList = null;

                        var users = await _graphServiceClient.Users.Request().GetAsync();

                        if (users != null)
                        {
                            ownersList = new Dictionary<string, string>();
                            foreach (var user in users.Where(x => !string.IsNullOrEmpty(x.UserPrincipalName)))
                            {
                                try
                                {
                                    if (!ownersList.ContainsKey(user.Id))
                                    {
                                        var value = string.IsNullOrEmpty(user.GivenName + user.Surname) ? user.UserPrincipalName : $"{user.GivenName} {user.Surname}";
                                        ownersList.Add(user.Id, value);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError($"GroupOwnerService-GetGroupOwnerMemberAsync: unable to add user in the group owner and member collection for user {user.UserPrincipalName}");
                                    _logger.LogError(ex);
                                }
                            }
                        }
                        if (ownersList != null)
                        {
                            retVal.Owners = new Dictionary<string, string>();
                            foreach(var aDict in ownersList)
                            {
                                retVal.Owners.Add(aDict.Key, aDict.Value);
                            }
                        }
                    }),
                    Task.Run(async() =>
                    {
                        List<UserModel> assignedOwners = new List<UserModel>();

                        var owners = await _graphServiceClient.Groups[groupId].Owners.Request().GetAsync();
                        if (owners != null)
                        {
                            foreach (var owner in owners)
                            {
                                try
                                {
                                    if (!(owner is User))
                                    {
                                        continue;
                                    }

                                    UserModel userModel = GraphClientUtility.ConvertGraphUserToUserModel((User)owner, _logger);
                                    assignedOwners.Add(userModel);
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError($"GroupOwnerService-GetOwnersForGroupGetOwnersForGroup: Error adding group owner to the collection for owner {owner.Id}");
                                    _logger.LogError(ex);
                                }
                            }
                        }

                        if(assignedOwners != null)
                        {
                            retVal.AssignedOwners = assignedOwners.ToList<UserModel>();
                        }
                    })
                };

                await Task.WhenAll(tasks);

                if (retVal != null)
                {
                    if ((retVal.AssignedOwners != null) && (retVal.Owners != null))
                    {
                        var userIds = retVal.AssignedOwners.Select(x => x.Id).ToList();
                        if (retVal.Owners.Any() && userIds.Any())
                        {
                            var keys = retVal.Owners.Keys.Where(x => userIds.Contains(x)).ToList();
                            foreach (var key in keys)
                            {
                                retVal.Owners.Remove(key);
                            }
                        }
                    }
                }

                _logger.LogInfo("GroupOwnerService-GetGroupOwnerAsync: Completed getting the group owner from Azure AD B2C");
            }
            catch (Exception ex)
            {
                _logger.LogError("GroupOwnerService-GetGroupOwnerAsync: Exception occured....");
                _logger.LogError(ex);
                throw ex;
            }
            return retVal;
        }
    }
}
