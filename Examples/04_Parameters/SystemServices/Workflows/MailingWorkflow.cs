using System.Threading.Tasks;

using DomainWorkflows.Workflows;

using SystemServices.Services;

namespace SystemServices.Workflows
{
    [Singleton(RunAlways = true)] // start this workflow when host started
    public class MailingWorkflow : Workflow
    {
        private readonly INotificationService _notificationService;
        private readonly IProgressService _progressService;

        public MailingWorkflow(IProgressService progressService, INotificationService notificationService)
        {
            _progressService = progressService;
            _notificationService = notificationService;
        }

        // run every day from Monday to Friday at 8:00 AM
        //[Cron("0 0 8 ? * MON-FRI *", State = "@Enable=true")]
        [Cron("@DailyMailingSchedule", State = "@Enable=true")]
        public async Task DailyMailing()
        {
            var affectedCustomers = await _progressService.GetDailyProgressCustomers();
            foreach (int customerId in affectedCustomers)
            {                
                // generate and send email for customerId
                await _notificationService.SendDailyProgress(customerId);
            }
        }

        // run every Saturday at 7:00 AM
        //[Cron("0 0 7 ? * SAT *", State = "@Enable=true")]
        [Cron("@WeeklyMailingSchedule", State = "@Enable=true")]
        public async Task WeeklyMailing()
        {
            var affectedCustomers = await _progressService.GetWeeklyProgressCustomers();
            foreach (int customerId in affectedCustomers)
            {                
                await _notificationService.SendWeeklyProgress(customerId);
            }
        }
    }
}
