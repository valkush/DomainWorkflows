using System.Threading.Tasks;

using DomainWorkflows.Events;
using DomainWorkflows.Workflows;

using Messenger.Events;
using Messenger.Services;

namespace Messenger.Workflows
{
    public class NotificationWorkflow : Workflow
    {
        private readonly IEmailService _emailService;
        private readonly IEventSource _eventSource;

        [Persist]
        private string _userId;

        [Persist]
        private bool _newMessages;
        [Persist]
        private bool _emailSent;
        [Persist]
        private int _slider;

        public NotificationWorkflow(IEmailService emailService, IEventSource eventSource)
        {
            _emailService = emailService;
            _eventSource = eventSource;
        }

        protected override async Task OnStart()
        {
            _userId = ((MessageReceived)Event).UserId;
            _newMessages = false;
            _emailSent = false;
        }

        [WorkflowStart]
        public async Task Handle(MessageReceived messageReceived)
        {
            if (_newMessages == false && _emailSent == false)
            {
                _slider = 0;
            }
            else
            {
                _slider++;
            }

            _newMessages = true;            
        }

        public async Task Handle(MessagesRead messagesRead)
        {
            _newMessages = false;
            _emailSent = false;
        }

        [Timeout("10 min", State = "_newMessages = true AND _emailSent = false", Sliding = "_slider")]
        public async Task SlidingTimeout()
        {
            await OnNewMessages();
        }

        [Timeout("30 min", State = "_newMessages = true AND _emailSent = false")]
        public async Task LimitingTimeout()
        {
            await OnNewMessages();
        }

        private async Task OnNewMessages()
        {
            await _emailService.SendNewMessages(_userId);

            // raise MessagesUnread domain event
            await _eventSource.Raise(new MessagesUnread()
            {
                UserId = _userId
            });

            _emailSent = true;
            _newMessages = false;
        }
    }
}
