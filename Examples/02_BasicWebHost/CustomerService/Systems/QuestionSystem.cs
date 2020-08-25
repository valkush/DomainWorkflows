using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using CustomerService.Domain;
using CustomerService.Events;
using CustomerService.Services;
using DomainWorkflows.Events;

namespace CustomerService.Systems
{
    // Gateway API pattern implementation
    public sealed class QuestionSystem : IQuestionSystem
    {
        private readonly IEventSource _eventSource;
        private readonly IQuestionService _questionService;

        public QuestionSystem(IEventSource eventSource, IQuestionService questionService)
        {
            _eventSource = eventSource;
            _questionService = questionService;
        }

        public async Task<Question> GetQuestion(int questionId)
        {
            return await _questionService.GetQuestion(questionId);
        }

        public async Task AskQuestion(Question question)
        {
            QuestionAsked questionAsked = new QuestionAsked()
            {
                Name = question.Name,
                Email = question.Email,
                Text = question.Text
            };

            await _eventSource.Raise(questionAsked);            
        }

        public async Task AnswerQuestion(int questionId, string answer)
        {            
            QuestionAnswered questionAnswered = new QuestionAnswered()
            {
                QuestionId = questionId,
                Answer = answer
            };

            await _eventSource.Raise(questionAnswered);
        }

        public async Task<IList<Question>> GetQuestions()
        {            
            return await _questionService.GetQuestions();
        }        
    }
}
