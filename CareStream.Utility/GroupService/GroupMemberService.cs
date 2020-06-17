using CareStream.LoggerService;
using CareStream.Models;
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CareStream.Utility
{
    public interface IGroupMemberService
    {
        Task<GroupMemberModel> GetGroupMembers(string id);

        Task AddGroupMembers(GroupMemberAssignModel groupMemberAssignModel);

        Task RemoveGroupMembers(GroupMemberAssignModel val);
    }

    public class GroupMemberService : IGroupMemberService
    {
        private readonly ILoggerManager _logger;

        public GroupMemberService(ILoggerManager logger)
        {
            _logger = logger;
        }

        public async Task<GroupMemberModel> GetGroupMembers(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id) || string.IsNullOrWhiteSpace(id))
                {
                    _logger.LogError("GroupMemberService-GetGroupMembers: Id cannot be empty");
                    return null;
                }

                var groupMemberModel = new GroupMemberModel
                {
                    GroupId = id
                };

                _logger.LogInfo($"GroupMemberService-GetGroupMembers: [Started] to get member detail for group id {id} from Azure AD B2C");

                var client = GraphClientUtility.GetGraphServiceClient();

                if (client == null)
                {
                    _logger.LogError("GroupMemberService-GetGroupMembers: Unable to create object for graph client");
                    return null;
                }

                groupMemberModel = await GetGroupMemberAsync(id);

                _logger.LogInfo($"GroupMemberService-GetGroup: [Completed] to getting member detail for group id {id} from Azure AD B2C");
                return groupMemberModel;

            }
            catch (ServiceException ex)
            {
                _logger.LogError("GroupMemberService-GetGroup: Exception occured....");
                _logger.LogError(ex);
                throw ex;
            }
        }

        public async Task AddGroupMembers(GroupMemberAssignModel groupMemberAssignModel)
        {
            try
            {
                if (groupMemberAssignModel == null)
                {
                    _logger.LogError("GroupMemberService-AddGroupMembers: Group Member cannot be null...");
                    return;
                }

                _logger.LogInfo("GroupMemberService-AddGroupMembers: [Started] assigning member to group in Azure AD B2C");

                if (!string.IsNullOrEmpty(groupMemberAssignModel.GroupId) && groupMemberAssignModel.SelectedMembers.Any())
                {
                    var client = GraphClientUtility.GetGraphServiceClient();
                    if (client == null)
                    {
                        _logger.LogError("GroupMemberService-AddGroupMembers: Unable to create object for graph client ");
                        return;
                    }

                    // Get all user from Graph
                    var users = await client.Users.Request().GetAsync();
                    if (users != null)
                    {
                        var assigningUsers = users.Where(x => groupMemberAssignModel.SelectedMembers.Contains(x.Id)).ToList();
                        if (assigningUsers != null)
                        {
                            foreach (var assignUser in assigningUsers)
                            {
                                try
                                {
                                    await client.Groups[groupMemberAssignModel.GroupId].Members.References.Request().AddAsync(assignUser);
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError($"GroupMemberService-AddGroupMembers: unable to assign member [{assignUser.UserPrincipalName}] to the group [id:{groupMemberAssignModel.GroupId}]");
                                    _logger.LogError(ex);
                                }
                            }
                        }
                    }
                }

                _logger.LogInfo("GroupMemberService-AddGroupMembers: [Completed] assigning member(s) to group in Azure AD B2C");

            }
            catch (Exception ex)
            {
                _logger.LogError("GroupMemberService-AddGroupMembers: Exception occured....");
                _logger.LogError(ex);
            }
        }

        public async Task RemoveGroupMembers(GroupMemberAssignModel val)
        {
            try
            {
                if (val == null)
                {
                    _logger.LogError("GroupMemberService-RemoveGroupMembers: Input value cannot be empty");
                    return;
                }

                var groupId = val.GroupId;
                if (!string.IsNullOrEmpty(groupId))
                {
                    GraphServiceClient client = GraphClientUtility.GetGraphServiceClient();
                    if (client == null)
                    {
                        _logger.LogError("GroupMemberService-RemoveGroupMembers: Unable to create object for graph client");
                        return;
                    }

                    _logger.LogInfo($"GroupMemberService-RemoveGroupMembers: [Started] unassigning member(s) for group [Id:{groupId}]");

                    foreach (var memberId in val.SelectedMembers)
                    {
                        try
                        {
                            _logger.LogInfo($"GroupMemberService-RemoveGroupMembers: Removing member [{memberId}] from group [Id:{groupId}] on Azure AD B2C");

                            await client.Groups[groupId].Members[memberId].Reference.Request().DeleteAsync();

                            _logger.LogInfo($"GroupMemberService-RemoveGroupMembers: Removed member [{memberId}] from group [Id:{groupId}] on Azure AD B2C");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"GroupMemberService-RemoveGroupMembers: Exception occured while unassigning owner [Id:{memberId}] for group  [Id:{groupId}] ");
                            _logger.LogError(ex);
                        }
                    }

                    _logger.LogInfo($"GroupMemberService-RemoveGroupMembers: [Completed] unassigning owner(s) from group [Id:{val.GroupId}]");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError("GroupOwnersController-RemoveGroupOwners: Exception occured....");
                _logger.LogError(ex);
                throw ex;
            }
        }

        private async Task<GroupMemberModel> GetGroupMemberAsync(string groupId)
        {
            GroupMemberModel retVal = null;
            try
            {
                if (string.IsNullOrEmpty(groupId))
                {
                    _logger.LogError("GroupMemberService-GetGroupMemberAsync: Group Id cannot be null");
                    return retVal;

                }
                _logger.LogInfo("GroupMemberService-GetGroupMemberAsync: Starting to get group owner from Azure AD B2C");

                var client = GraphClientUtility.GetGraphServiceClient();

                retVal = new GroupMemberModel
                {
                    GroupId = groupId
                };

                var tasks = new Task[]
                {
                    Task.Run(async() => {

                         Dictionary<string, string> membersList = null;
                          var users = await client.Users.Request().GetAsync();

                            if (users != null)
                            {
                                membersList = new Dictionary<string, string>();
                                foreach (var user in users.Where(x => !string.IsNullOrEmpty(x.UserPrincipalName)))
                                {
                                    try
                                    {
                                        if (!membersList.ContainsKey(user.Id))
                                        {
                                            var value = string.IsNullOrEmpty(user.GivenName + user.Surname) ? user.UserPrincipalName : $"{user.GivenName} {user.Surname}";
                                            membersList.Add(user.Id, value);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        _logger.LogError($"GroupMemberService-GetGroupOwnerMemberAsync: unable to add user in the group owner and member collection for user {user.UserPrincipalName}");
                                        _logger.LogError(ex);
                                    }
                                }
                            }

                            if (membersList != null)
                            {
                                retVal.Members = new Dictionary<string, string>();
                                foreach(var aDict in membersList)
                                {
                                    retVal.Members.Add(aDict.Key, aDict.Value);
                                }
                            }
                    }),
                    Task.Run(async() =>
                    {
                         List<UserModel> assignedMembers = new List<UserModel>();
                        var members = await client.Groups[groupId].Members.Request().GetAsync();
                        if (members != null)
                        {
                            foreach (var member in members)
                            {
                                try
                                {
                                    if (!(member is User))
                                    {
                                        continue;
                                    }

                                    UserModel userModel = GraphClientUtility.ConvertGraphUserToUserModel((User)member, _logger);
                                    assignedMembers.Add(userModel);
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError($"GroupMemberService-GetMembersForGroup: Error adding group owner to the collection for owner {member.Id}");
                                    _logger.LogError(ex);
                                }
                            }
                        }

                        if(assignedMembers != null)
                        {
                            retVal.AssignedMembers = assignedMembers.ToList<UserModel>();
                        }
                    })
                };

                await Task.WhenAll(tasks);

                if (retVal != null)
                {
                    if ((retVal.AssignedMembers != null) && (retVal.Members != null))
                    {
                        var userIds = retVal.AssignedMembers.Select(x => x.Id).ToList();
                        if (retVal.Members.Any() && userIds.Any())
                        {
                            var keys = retVal.Members.Keys.Where(x => userIds.Contains(x)).ToList();
                            foreach (var key in keys)
                            {
                                retVal.Members.Remove(key);
                            }
                        }
                    }
                }
                _logger.LogInfo("GroupMemberService-GetGroupMemberAsync: Completed getting the group owner from Azure AD B2C");
            }
            catch (Exception ex)
            {
                _logger.LogError("GroupMemberService-GetGroupMemberAsync: Exception occured....");
                _logger.LogError(ex);
                throw ex;
            }
            return retVal;
        }
    }
}
