using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerService.Domain
{
    public class Question
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string Email { get; set; }

        public string Text { get; set; }

        public QuestionStatus Status { get; set; }

        public Answer Answer { get; set; }        
    }
}
