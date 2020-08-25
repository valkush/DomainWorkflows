using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CustomerService.Domain;

namespace CustomerService.Services
{
    public class QuestionService : IQuestionService
    {
        private int _nextId = 1;        

        private List<Question> _questions = new List<Question>();        

        public QuestionService()
        {            
        }

        public async Task<IList<Question>> GetQuestions()
        {
            return _questions;
        }

        public async Task<Question> GetQuestion(int questionId)
        {
            Question question = _questions.Where(q => q.Id == questionId).First();

            return question;
        }

        public async Task AddQuestion(Question question)
        {
            question.Id = _nextId++;
            question.Status = QuestionStatus.New;

            _questions.Add(question);
        }

        public async Task AddAnswer(int questionId, Answer answer)
        {
            Question question = _questions.Where(q => q.Id == questionId).First();
            
            question.Answer = answer;
            question.Status = QuestionStatus.Answered;
        }        

        public async Task Assign(int questionId, int userId)
        {
            Question question = _questions.Where(q => q.Id == questionId).First();

            question.Status = QuestionStatus.Assigned;
        }

        public async Task Overdue(int questionId)
        {
            Question question = _questions.Where(q => q.Id == questionId).First();

            question.Status = QuestionStatus.Overdue;
        }

        public async Task Close(int questionId)
        {
            Question question = _questions.Where(q => q.Id == questionId).First();

            question.Status = QuestionStatus.Closed;
        }        
    }
}
