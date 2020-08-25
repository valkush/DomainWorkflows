using System.Threading.Tasks;

namespace DocumentsAproval.Services
{
    public interface IApprovalPolicy
    {
        Task<int> GetApprover(int documentId);
    }
}
