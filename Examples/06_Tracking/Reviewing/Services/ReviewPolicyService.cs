using System.Threading.Tasks;

namespace Reviewing.Services
{
    internal class ReviewPolicyService : IReviewPolicyService
    {
        public async Task<int[]> GetReviewerChain(int articleId)
        {
            return new int[] {12, 14, 2 };
        }
    }
}
