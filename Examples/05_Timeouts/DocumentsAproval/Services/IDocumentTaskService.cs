using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DocumentsAproval.Services
{
    public interface IDocumentTaskService
    {
        Task<int> CreateReviewTask(int reviwerId, int documentId);
        Task EscalateTask(int taskId);
        Task TimeoutTask(int taskId);
        Task<int?> CreateApproveTask(int approverId, int documentId);
        Task CloseTask(int value);
    }
}
