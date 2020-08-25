using System.Threading.Tasks;

namespace Reviewing.Services
{
    public interface IReviewPolicyService
    {
        Task<int[]> GetReviewerChain(int articleId);
    }
}
