using System.Threading.Tasks;

using DomainWorkflows.Workflows;

using CustomerService.Events;
using CustomerService.Services;
using CustomerService.Domain;

namespace CustomerService.Workflows
{
    public enum QuestionState { Open, Answered }

    [Timeout("3 min")]
    public class QuestionWorkflow : Workflow
    {
        private readonly IQuestionService _questionService;
        private readonly IAssignmentService _assignmentService;
        private readonly INotificationService _notificationService;

        [Persist]
        private int _questionId;

        [Persist("State")]
        private QuestionState _state = QuestionState.Open;

        public QuestionWorkflow(IQuestionService questionService, IAssignmentService assignmentService,
            INotificationService notificationService)
        {
            _questionService = questionService;
            _assignmentService = assignmentService;
            _notificationService = notificationService;
        }

        [LateKey]
        public async Task Handle(QuestionAsked questionAsked)
        {
            Question question = new Question()
            {
                Name = questionAsked.Name,
                Email = questionAsked.Email,
                Text = questionAsked.Text
            };

            await _questionService.AddQuestion(question);            

            base.ExtractKey(question.Id);

            _questionId = question.Id;

            // multistep handler
            int userId = _assignmentService.GetAppropriateUser(_questionId);
            if (userId != 0)
            {
                await _questionService.Assign(question.Id, userId);
            }

            // confirm question received
            await _notificationService.NotifyCustomerQuestion(_questionId);
        }

        public async Task Handle(QuestionAnswered questionAnswered)
        {
            Answer answer = new Answer() { Text = questionAnswered.Answer };
            await _questionService.AddAnswer(_questionId, answer);

            await _notificationService.NotifyCustomerAnswer(_questionId);

            // start question closing timer here
            _state = QuestionState.Answered;
        }

        // close question if it has been answered 30 sec
        [Timeout("30 sec", State = "State = #Answered")]
        public async Task Close()
        {
            await _questionService.Close(_questionId);

            // complete the workflow - cancel all timeouts
            await this.Complete();
        }

        protected override async Task OnTimeout()
        {
            if (_state == QuestionState.Answered)
            {
                await _questionService.Close(_questionId);
            }
            else
            {
                await _questionService.Overdue(_questionId);
            }
        }
    }
}
