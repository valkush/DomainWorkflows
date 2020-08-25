using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using DomainWorkflows.Workflows;

using DocumentsAproval.Events;
using DocumentsAproval.Services;

namespace DocumentsAproval.Workflows
{
    public enum ApprovalState { New, Review, Approving, Approved, Declined, Overdue }

    public class ExpenseApprovalWorkflow : Workflow
    {
        private readonly IDocumentService _documentService;
        private readonly IApprovalPolicy _approvalPolicy;
        private readonly IDocumentTaskService _documentTaskService;
        private readonly INotificationService _notificationService;
        private readonly ILogger<ExpenseApprovalWorkflow> _logger;        

        [Persist]
        private int _documentId;

        [Persist("State")]
        private ApprovalState _state;

        [Persist]
        private int? _taskId = null;

        [Persist]
        private int _startSlider;
        

        public ExpenseApprovalWorkflow(IDocumentService documentService, IApprovalPolicy approvalPolicy, IDocumentTaskService documentTaskService,
            INotificationService notificationService, ILogger<ExpenseApprovalWorkflow> logger)
        {            
            _documentService = documentService;
            _approvalPolicy = approvalPolicy;
            _documentTaskService = documentTaskService;
            _notificationService = notificationService;
            _logger = logger;
        }
        
        [WorkflowStart]
        public async Task Handle(ExpenseApprovalRequested request)
        {
            _documentId = request.DocumentId;

            _state = ApprovalState.New;

            _logger.LogInformation("Expense Approval started, DocumentId={DocumentId}", _documentId);
        }

        [Timeout("15 sec", State = "State=#New", Sliding = "_startSlider")]
        public async Task StartReview()
        {
            int reviwerId = await _approvalPolicy.GetApprover(_documentId);

            _taskId = await _documentTaskService.CreateReviewTask(reviwerId, _documentId);            

            _state = ApprovalState.Review;
            
            _logger.LogInformation("Expense reviwer assigned, ReviwerId={ReviewerId}, DocumentId={DocumentId}", reviwerId, _documentId);
        }

        public async Task Handle(ExpenseRequestUpdated update)
        {
            // move slider - restart Start Approval timeout
            _startSlider++;

            _logger.LogInformation("Document updated, DocumentId={DocumentId}", _documentId);
            _logger.LogDebug("Expense update slide moved, Slider={StartSlider}", _startSlider);
        }


        [Timeout("25 sec", State = "State=#Review")]
        public async Task ReviewEscalate()
        {
            await _documentTaskService.EscalateTask(_taskId.Value);
            
            await _notificationService.NotifyReviewEscalate(_taskId.Value);

            _logger.LogInformation("Review escalated, DocumentId={DocumentId}, TaskId={TaskId}", _documentId, _taskId);
        }

        [Timeout("45 sec", State = "State=#Review")]        
        public async Task ReviewTimeout()
        {
            await _documentTaskService.TimeoutTask(_taskId.Value);

            _state = ApprovalState.Overdue;

            _logger.LogInformation("REVIEW TIMEOUT, DocumentId={DocumentId}, TaskId={TaskId}", _documentId, _taskId);

            await Timeout();
        }

        public async Task Handle(ExpenseRequestReviewed review)
        {
            if (_state != ApprovalState.Review)
            {
                _logger.LogWarning("Invalid reviwing action for state {State}", _state);
                return;
            }

            // close review task
            await _documentTaskService.CloseTask(_taskId.Value);
            _taskId = null;

            // make the document readonly
            await _documentService.LockDocument(_documentId);

            int approverId = await _approvalPolicy.GetApprover(_documentId);

            _taskId = await _documentTaskService.CreateApproveTask(approverId, _documentId);            

            _state = ApprovalState.Approving;

            _logger.LogInformation("Expense approver assigned, ApproverId={ApproverId}, DocumentId={DocumentId}", approverId, _documentId);
        }

        [Timeout("1 min", State = "State=#Approving")]
        public async Task ApproveTimeout()
        {
            await _documentTaskService.TimeoutTask(_taskId.Value);

            _state = ApprovalState.Overdue;

            _logger.LogInformation("APPROVAL TIMEOUT, DocumentId={DocumentId}, TaskId={TaskId}", _documentId, _taskId);

            await Timeout();
        }        
        
        public async Task Handle(ExpenseRequestApprove approve)
        {
            if (_state != ApprovalState.Approving)
            {
                _logger.LogWarning("Invalid appoving action for state {State}", _state);
                return;
            }

            // close approval task
            await _documentTaskService.CloseTask(_taskId.Value);
            _taskId = null;

            if (approve.Approved)
            {
                await _documentService.ApproveDocument(_documentId);

                _state = ApprovalState.Approved;
                _logger.LogInformation("Expense Request approved, DocumentId={DocumentId}", _documentId);
            }
            else
            {
                await _documentService.DeclineDocument(_documentId);

                _state = ApprovalState.Declined;
                _logger.LogInformation("Expense Request declined, DocumentId={DocumentId}", _documentId);
            }

            await Complete();
        }

        protected async override Task OnTimeout()
        {
            await _documentService.OverdueDocument(_documentId);

            _logger.LogWarning("WORKFLOW TIMEOUT, DocumentId={DocumentId}", _documentId);
        }
    }
}
