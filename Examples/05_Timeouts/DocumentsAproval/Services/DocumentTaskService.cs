using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DocumentsAproval.Services
{
    internal class DocumentTaskService : IDocumentTaskService
    {
        private Random _rand = new Random();

        public async Task<int> CreateReviewTask(int reviwerId, int documentId)
        {
            return _rand.Next(1000);
        }

        public async Task<int?> CreateApproveTask(int approverId, int documentId)
        {
            return _rand.Next(1000);
        }

        public async Task CloseTask(int value)
        {            
        }

        public async Task EscalateTask(int taskId)
        {
            
        }

        public async Task TimeoutTask(int taskId)
        {
            
        }
    }
}
