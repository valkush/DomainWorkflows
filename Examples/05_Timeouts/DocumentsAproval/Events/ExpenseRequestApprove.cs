using System;
using System.Collections.Generic;
using System.Text;

namespace DocumentsAproval.Events
{
    public class ExpenseRequestApprove
    {
        public int DocumentId { get; set; }

        public bool Approved { get; set; }
    }
}
