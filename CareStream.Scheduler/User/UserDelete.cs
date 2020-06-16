using CareStream.LoggerService;
using CareStream.Models;
using CareStream.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CareStream.Scheduler
{
    public class UserDelete : IUserProcess
    {
        private readonly ILoggerManager _logger;
        private readonly CareStreamContext _cc;
        private readonly DbHelper _dbHelper;
        public UserDelete(ILoggerManager logger, CareStreamContext cc)
        {
            _logger = logger;
            _cc = cc;
            _dbHelper = new DbHelper(logger, cc);
        }

        public async Task ProcessUser(List<BulkUser> bulkUsers)
        {
            var logConst = "UserDelete-ProcessUser";
            try
            {
                foreach (var bulkUser in bulkUsers)
                {
                    try
                    {
                        _dbHelper.UpdateTable(bulkUser, CareStreamConst.Bulk_User_Started_Status);

                        if (string.IsNullOrEmpty(bulkUser.UserPrincipalName))
                        {
                            var errorMessage = $"[User delete] user id is missing, please correct the record.";

                            _logger.LogWarn(errorMessage);
                            bulkUser.Error = errorMessage;
                            _dbHelper.UpdateTable(bulkUser, CareStreamConst.Bulk_User_Failed_Status);
                            continue;
                        }

                        var client = GraphClientUtility.GetGraphServiceClient();
                        if (client == null)
                        {
                            var errorMessage = $"[User Delete] Unable to get the proxy for Graph client for user [{bulkUser.UserPrincipalName}]";
                            _logger.LogWarn(errorMessage);
                            continue;
                        }

                        var userAsync = await client.Users.Request().Filter($"userPrincipalName eq '{bulkUser.UserPrincipalName}'").GetAsync();

                        if (userAsync.Any())
                        {
                            foreach (var user in userAsync)
                            {
                                try
                                {
                                    await client.Users[user.Id].Request().DeleteAsync();
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError($"{logConst}:error deleting user for bulk user id {bulkUser.Id}");
                                    _logger.LogError(ex);

                                    bulkUser.Error = $"{ex.ToString()}. Message: {ex.Message}";
                                    _dbHelper.UpdateTable(bulkUser, CareStreamConst.Bulk_User_Failed_Status);
                                    continue;
                                }
                                _dbHelper.UpdateTable(bulkUser, CareStreamConst.Bulk_User_Completed_Status);
                            }

                        }
                        else
                        {
                            var errorMessage = $"[User delete] unable to get the detail for user id {bulkUser.UserPrincipalName}";

                            _logger.LogWarn(errorMessage);
                            bulkUser.Error = errorMessage;
                            _dbHelper.UpdateTable(bulkUser, CareStreamConst.Bulk_User_Failed_Status);
                        }



                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"{logConst}:error creating user for bulk user id {bulkUser.Id}");
                        _logger.LogError(ex);

                        bulkUser.Error = $"{ex.ToString()}. Message: {ex.Message}";
                        _dbHelper.UpdateTable(bulkUser, CareStreamConst.Bulk_User_Failed_Status);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{logConst}:Exception occured while processing the bulk user...");
                _logger.LogError(ex);
            }
        }


    }
}
