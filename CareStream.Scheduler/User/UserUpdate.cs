using CareStream.LoggerService;
using CareStream.Models;
using CareStream.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CareStream.Scheduler
{
    public class UserUpdate : IUserProcess
    {

        private readonly ILoggerManager _logger;
        private readonly CareStreamContext _cc;
        private readonly DbHelper _dbHelper;
        public UserUpdate(ILoggerManager logger, CareStreamContext cc)
        {
            _logger = logger;
            _cc = cc;
            _dbHelper = new DbHelper(logger, cc);
        }

        public async Task ProcessUser(List<BulkUser> bulkUsers)
        {
            var logConst = "UserUpdate-ProcessUser";
            try
            {
                foreach (var bulkUser in bulkUsers)
                {
                    try
                    {
                        _dbHelper.UpdateTable(bulkUser, CareStreamConst.Bulk_User_Started_Status);

                        #region UserModel

                        var userModel = new UserModel
                        {
                            AccountEnabled = bulkUser.BlockSignIn,
                            UserPrincipalName = bulkUser.UserPrincipalName,
                            DisplayName = bulkUser.DisplayName,
                            Password = bulkUser.InitialPassword,
                            GivenName = bulkUser.FirstName,
                            Surname = bulkUser.LastName,
                            JobTitle = bulkUser.JobTitle,
                            Department = bulkUser.Department,
                            UsageLocation = bulkUser.Usagelocation,
                            StreetAddress = bulkUser.StreetAddress,
                            State = bulkUser.State,
                            Country = bulkUser.Country,
                            OfficeLocation = bulkUser.Office,
                            City = bulkUser.City,
                            PostalCode = bulkUser.ZIP,
                            MobilePhone = bulkUser.MobilePhone,
                            BusinessPhone = bulkUser.OfficePhone
                        };


                        #endregion

                        if (string.IsNullOrEmpty(bulkUser.UserPrincipalName) && string.IsNullOrEmpty(bulkUser.ObjectID))
                        {
                            var errorMessage = $"[Cannot update user] For bulk user id [{bulkUser.Id}] required field is missing, please correct the record.";

                            _logger.LogWarn(errorMessage);
                            bulkUser.Error = errorMessage;
                            _dbHelper.UpdateTable(bulkUser, CareStreamConst.Bulk_User_Failed_Status);
                            continue;
                        }

                        var client = GraphClientUtility.GetGraphServiceClient();
                        if (client == null)
                        {
                            var errorMessage = $"[User Create] Unable to get the proxy for Graph client for user [{bulkUser.UserPrincipalName}]";
                            _logger.LogWarn(errorMessage);
                            continue;
                        }

                        var userId = bulkUser.ObjectID;

                        if (string.IsNullOrEmpty(userId))
                        {
                            var userAsync = await client.Users.Request().Filter($"userPrincipalName eq '{bulkUser.UserPrincipalName}'").GetAsync();

                            if (userAsync.Any())
                            {
                                userId = userAsync[0].Id;
                            }

                        }

                        if (string.IsNullOrEmpty(userId))
                        {
                            bulkUser.Error = $"Failed to get user for [{bulkUser.UserPrincipalName}]";
                            _dbHelper.UpdateTable(bulkUser, CareStreamConst.Bulk_User_Failed_Status);
                            continue;
                        }


                        var b2cExtensionAppClientId = GraphClientUtility.b2cExtensionAppClientId;

                        var userService = new UserService(_logger);
                        var updatingUser = userService.BuildUserForUpdate(userModel, b2cExtensionAppClientId);

                        var result = await client.Users[userId].Request().UpdateAsync(updatingUser);

                        if (result != null)
                        {
                            bulkUser.ObjectID = result.Id;
                            _dbHelper.UpdateTable(bulkUser, CareStreamConst.Bulk_User_Completed_Status);
                        }
                        else
                        {
                            bulkUser.Error = $"Failed to update user for [{bulkUser.UserPrincipalName}]";
                            _dbHelper.UpdateTable(bulkUser, CareStreamConst.Bulk_User_Failed_Status);
                        }

                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"{logConst}:error updating user for bulk user id {bulkUser.Id}");
                        _logger.LogError(ex);

                        bulkUser.Error = $"{ex.ToString()}. Message: {ex.Message}";
                        _dbHelper.UpdateTable(bulkUser, CareStreamConst.Bulk_User_Failed_Status);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{logConst}:Exception occured while processing the bulk user update...");
                _logger.LogError(ex);
            }
        }



    }
}
