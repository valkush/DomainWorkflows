using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DocumentsAproval.Services
{
    internal class ApprovalPolicy : IApprovalPolicy
    {
        public async Task<int> GetApprover(int documentId)
        {
            return 23;
        }
    }
}
