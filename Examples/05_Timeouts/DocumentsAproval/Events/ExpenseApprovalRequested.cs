using System;
using System.Collections.Generic;
using System.Text;

namespace DocumentsAproval.Events
{
    public class ExpenseApprovalRequested
    {
        public int DocumentId { get; set; }
    }
}
