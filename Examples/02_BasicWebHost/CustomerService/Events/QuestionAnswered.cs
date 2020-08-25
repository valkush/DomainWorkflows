using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerService.Events
{
    public class QuestionAnswered
    {
        public int QuestionId { get; set; }

        public string Answer { get; set; }
    }
}
