using Quartz;
using System.Threading.Tasks;

namespace CareStream.Scheduler
{
    public class SchedulerJob : IJob
    {
        private UserScheduleProcessor _userScheduleProcessor;

        public async Task Execute(IJobExecutionContext context)
        {
            _userScheduleProcessor = new UserScheduleProcessor();

            await _userScheduleProcessor.Process();
        }
    }
}
