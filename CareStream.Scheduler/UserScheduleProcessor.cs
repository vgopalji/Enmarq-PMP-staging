using CareStream.LoggerService;
using CareStream.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CareStream.Scheduler
{
    public class UserScheduleProcessor
    {
        private readonly ILoggerManager _logger;
        private CareStreamContext _cc;

        public UserScheduleProcessor()
        {
            _logger = new LoggerManager();

        }

        public async Task Process()
        {
            var logConst = "UserScheduleProcessor-Process";
            try
            {
                CareStreamContextFactory apiContextFactory = new CareStreamContextFactory();

                _cc = apiContextFactory.CreateDbContext(new string[] { });

                if (_cc == null)
                {
                    return;
                }



                var bulkUserFiles = _cc.BulkUserFiles.Where(x => x.Status == CareStreamConst.Bulk_User_Loaded_Status ||
                                       x.Status == CareStreamConst.Bulk_User_Started_Status).ToList();

                if (bulkUserFiles.Any())
                {
                    var _dbHelper = new DbHelper(_logger, _cc);
                    foreach (var bulkUserFile in bulkUserFiles)
                    {
                        try
                        {
                            _dbHelper.UpdateTable(bulkUserFile, CareStreamConst.Bulk_User_Started_Status);


                            var fileId = bulkUserFile.Id;

                            var bulkUsers = _cc.BulkUsers.Where(x => x.FileId == fileId
                                        && (x.Status == CareStreamConst.Bulk_User_Loaded_Status ||
                                            x.Status == CareStreamConst.Bulk_User_Started_Status ||
                                            x.Status == CareStreamConst.Bulk_User_InProgress_Status)).ToList();
                            if (bulkUsers.Any())
                            {
                                await ProcessUserAction(bulkUsers, bulkUserFile.Action);
                            }

                            _dbHelper.UpdateTable(bulkUserFile, CareStreamConst.Bulk_User_Completed_Status);

                        }
                        catch (Exception ex)
                        {

                            _logger.LogError($"{logConst}:error processing for file id {bulkUserFile.Id}");
                            _logger.LogError(ex);

                            bulkUserFile.Error = $"{ex.ToString()}. Message: {ex.Message}";
                            _dbHelper.UpdateTable(bulkUserFile, CareStreamConst.Bulk_User_Failed_Status);
                        }
                    }

                }

            }
            catch (Exception ex)
            {

                _logger.LogError($"{logConst}:Exception occured while processing the bulk user...");
                _logger.LogError(ex);
            }
        }

        private async Task ProcessUserAction(List<BulkUser> bulkUsers, string actionFor)
        {
            var logConst = "UserScheduleProcessor-ProcessUserAction";
            try
            {
                IUserProcess userProcess = null;
                switch (actionFor)
                {
                    case CareStreamConst.Bulk_User_Create:
                        userProcess = new UserCreate(_logger, _cc);
                        await userProcess.ProcessUser(bulkUsers);
                        break;
                    case CareStreamConst.Bulk_User_Invite:
                        userProcess = new UserInvite(_logger, _cc);
                        await userProcess.ProcessUser(bulkUsers);
                        break;
                    case CareStreamConst.Bulk_User_Update:
                        userProcess = new UserUpdate(_logger, _cc);
                        await userProcess.ProcessUser(bulkUsers);
                        break;
                    case CareStreamConst.Bulk_User_Delete:
                        userProcess = new UserDelete(_logger, _cc);
                        await userProcess.ProcessUser(bulkUsers);
                        break;

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
