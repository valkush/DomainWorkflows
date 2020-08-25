using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using DomainWorkflows.Events;
using DomainWorkflows.Workflows;

using ProgressNotifier.Events;

namespace ProgressNotifier.Workflows
{
    [Timeout("1 min")]
    public class ProgressWorkflow : Workflow
    {
        private readonly IEventSource _eventSource;
        private readonly ILogger<ProgressWorkflow> _logger;

        [Persist]
        private int _count = 0;

        [Persist]
        private int _processId;

        public ProgressWorkflow(IEventSource eventSource, ILogger<ProgressWorkflow> logger)
        {
            _eventSource = eventSource;
            _logger = logger;
        }

        protected override async Task OnStart()
        {
            _logger.LogDebug("[{WorkflowKey}] Workflow started", this.Key);
        }

        [WorkflowStart]
        public async Task Handle(ProgressRequested progressRequested)
        {
            _processId = progressRequested.ProcessId;

            _logger.LogInformation("[{WorkflowKey}] Progress Requested, ProcessId={ProcessId}",
                this.Key, _processId);
        }

        // ------ Try Cron and Repeat scheduling logic difference by comment in/out ------
        //[Cron("0/8 * * * * ?", State = "_count < 6")]
        [Repeat("8 sec", State = "_count < 6")]
        public async Task OnProgress()
        {
            _count++;

            _logger.LogInformation("[{WorkflowKey}] Progress Changed, Value={value}",
                this.Key, _count);

            await _eventSource.Raise(new ProgressChanged()
            {
                ProcessId = 1,
                Value = _count
            });
        }

        protected override async Task OnTimeout()
        {
            _logger.LogInformation("[{WorkflowKey}] WORKFLOW TIMEOUT",
                this.Key);
        }

        protected override async Task OnFinish()
        {
            _logger.LogDebug("[{WorkflowKey}] Workflow finished", this.Key);
        }
    }
}
