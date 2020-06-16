using CareStream.LoggerService;
using CareStream.Models;
using CareStream.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CareStream.Scheduler
{
    public class UserInvite : IUserProcess
    {
        private readonly ILoggerManager _logger;
        private readonly CareStreamContext _cc;
        private readonly DbHelper _dbHelper;
        public UserInvite(ILoggerManager logger, CareStreamContext cc)
        {
            _logger = logger;
            _cc = cc;
            _dbHelper = new DbHelper(logger, cc);
        }

        public async Task ProcessUser(List<BulkUser> bulkUsers)
        {
            var logConst = "UserInvite-ProcessUser";
            try
            {
                foreach (var bulkUser in bulkUsers)
                {
                    try
                    {
                        _dbHelper.UpdateTable(bulkUser, CareStreamConst.Bulk_User_Started_Status);

                        #region UserModel

                        var inviteUser = new InviteUser
                        {
                            CustomizedMessageBody = bulkUser.CustomizedMessageBody,
                            InvitedUserEmailAddress = bulkUser.InviteeEmail,
                            InviteRedeemUrl = bulkUser.InviteRedirectURL,
                            SendInvitationMessage = bulkUser.SendEmail.ToString()
                        };


                        #endregion

                        if (string.IsNullOrEmpty(inviteUser.InvitedUserEmailAddress) && string.IsNullOrEmpty(inviteUser.InviteRedeemUrl))
                        {
                            var errorMessage = $"[Cannot send user invite] for bulk user id [{bulkUser.Id}] required field is missing, please correct the record.";

                            _logger.LogWarn(errorMessage);
                            bulkUser.Error = errorMessage;
                            _dbHelper.UpdateTable(bulkUser, CareStreamConst.Bulk_User_Failed_Status);
                            continue;
                        }


                        var userService = new UserService(_logger);

                        await userService.SendInvite(inviteUser);

                        _dbHelper.UpdateTable(bulkUser, CareStreamConst.Bulk_User_Completed_Status);

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
