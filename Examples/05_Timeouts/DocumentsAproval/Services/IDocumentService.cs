using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DocumentsAproval.Services
{
    public interface IDocumentService
    {
        Task LockDocument(int documentId);
        Task ApproveDocument(int documentId);
        Task DeclineDocument(int documentId);
        Task OverdueDocument(int documentId);
    }
}
