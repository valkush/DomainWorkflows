using System.Collections.Generic;
using System.Threading.Tasks;

using CustomerService.Domain;

namespace CustomerService.Systems
{
    public interface IQuestionSystem
    {
        Task<IList<Question>> GetQuestions();

        Task<Question> GetQuestion(int questionId);

        Task AskQuestion(Question question);
        Task AnswerQuestion(int questionId, string answer);        
    }
}
