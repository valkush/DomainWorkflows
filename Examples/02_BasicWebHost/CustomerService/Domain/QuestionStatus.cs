using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerService.Domain
{
    public enum QuestionStatus
    {
        New,
        Assigned,        
        Answered,
        Reopen,
        Closed,
        Overdue
    }
}
