using System.Threading.Tasks;

using DomainWorkflows.Workflows;

using Reviewing.Domain;
using Reviewing.Events;
using Reviewing.Services;

namespace Reviewing.Workflows
{
    public class ReviewChainWorkflow : Workflow
    {
        private readonly IReviewPolicyService _reviewPolicyService;
        private readonly IUserService _userService;
        private readonly INotificationService _notificationService;

        [Persist]
        private int _articleId;

        [Persist]
        private ChainReviwer[] _reviwers;

        [Persist]
        private int _currReviwerIndex = -1;

        public ReviewChainWorkflow(IReviewPolicyService reviewPolicyService, IUserService userService, 
            INotificationService notificationService)
        {
            _reviewPolicyService = reviewPolicyService;
            _userService = userService;
            _notificationService = notificationService;
        }

        [WorkflowStart]
        public async Task Handle(ReviewRequested request)
        {
            _articleId = request.ArticleId;

            int[] reviwerIds = await _reviewPolicyService.GetReviewerChain(request.ArticleId);
            _reviwers = await PrepareReviwers(reviwerIds);

            this.Tracker.AddEntry($"Chain of {_reviwers.Length} reviewrs is defined", "Prepared");

            await SetNextReviwer();
        }

        // restart timeout everytime when State condition is true and Sliding field _currReviwerIndex is changed
        [Timeout("1 min", State = "_currReviwerIndex <> -1", Sliding = "_currReviwerIndex")]
        public async Task ReviewTimeout()
        {            
            this.Tracker.AddEntry($"Review is overdue by {_reviwers[_currReviwerIndex].Name}", "Overdue");

            await this.Complete();
        }

        public async Task Handle(ArticleReviwed review)
        {
            _reviwers[_currReviwerIndex].Accepted = review.Accepted;

            if (review.Accepted == true)
            {
                this.Tracker.AddEntry($"Accepted by {_reviwers[_currReviwerIndex].Name}", "Accepted");

                await SetNextReviwer();
            }
            else
            {
                this.Tracker.AddEntry($"Rejected by {_reviwers[_currReviwerIndex].Name}", "Rejected", "REJECTED");

                await this.Complete();                
            }
        }

        private async Task SetNextReviwer()
        {
            int nextReviwerIndex = GetNextReviwerIndex();
            if (nextReviwerIndex != -1)
            {
                _currReviwerIndex = nextReviwerIndex;

                this.Tracker.AddEntry($"{_reviwers[_currReviwerIndex].Name} is assigned as current reviwer", "Reviwing");

                int reviwerId = _reviwers[_currReviwerIndex].UserId;
                await _notificationService.NotifyReviewAssigned(reviwerId, _articleId);
            }
            else
            {
                this.Tracker.AddEntry($"All revivers are accepted the article", "Accepted", "REVIWED");

                await this.Complete();
            }
        }

        private async Task<ChainReviwer[]> PrepareReviwers(int[] reviwerIds)
        {
            ChainReviwer[] reviwers = new ChainReviwer[reviwerIds.Length];
            for (int i = 0; i < reviwerIds.Length; i++)
            {
                int reviwerId = reviwerIds[i];
                UserInfo userInfo = await _userService.GetUserInfo(reviwerId);
                ChainReviwer reviwer = new ChainReviwer()
                {
                    UserId = reviwerId,
                    Name = userInfo.FirstName + " " + userInfo.LastName
                };
                reviwers[i] = reviwer;
            }

            return reviwers;
        }

        private int GetNextReviwerIndex()
        {
            for (int i = 0; i < _reviwers.Length; i++)
            {
                if (_reviwers[i].Accepted == null)
                    return i;
            }

            return -1;
        }
    }

    public sealed class ChainReviwer
    {
        public int UserId { get; set; }
        public string Name { get; set; }

        public bool? Accepted { get; set; }
    }
}
