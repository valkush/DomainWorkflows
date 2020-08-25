using System.Collections.Generic;
using System.Threading.Tasks;

using CustomerService.Domain;

namespace CustomerService.Services
{
    public interface IQuestionService
    {
        Task<IList<Question>> GetQuestions();

        Task<Question> GetQuestion(int questionId);

        Task AddQuestion(Question question);
        Task AddAnswer(int questionId, Answer answer);

        Task Assign(int questionId, int userId);
        Task Overdue(int questionId);
        Task Close(int questionId);
        
    }
}
