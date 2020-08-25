using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using CustomerService.Domain;
using CustomerService.Models;
using CustomerService.Systems;

namespace CustomerService.Controllers
{
    public class QuestionsController : Controller
    {
        private readonly IQuestionSystem _questionSystem;

        public QuestionsController(IQuestionSystem questionSystem)
        {
            _questionSystem = questionSystem;
        }

        public async Task<IActionResult> Index()
        {
            IList<Question> questions = await _questionSystem.GetQuestions();

            var model = questions.Select(q => new QuestionModel()
            {
                Id = q.Id,
                Name = q.Name,
                Email = q.Email,
                Text = q.Text,
                Status = q.Status
            });            

            return View(model);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(NewQuestionModel model)
        {
            if (ModelState.IsValid)
            {
                Question question = new Question()
                {
                    Name = model.Name,
                    Email = model.Email,
                    Text = model.Text
                };

                await _questionSystem.AskQuestion(question);

                return RedirectToAction("ConfirmQuestion");
            }

            return View();
        }

        public IActionResult ConfirmQuestion()
        {
            return View();
        }

        public async Task<IActionResult> Answer(int id)
        {
            var question = await _questionSystem.GetQuestion(id);

            var model = new AnswerModel()
            {
                QuestionId = id,
                QuestionText = question.Text                
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Answer(AnswerModel model)
        {
            if (ModelState.IsValid)
            {
                await _questionSystem.AnswerQuestion(model.QuestionId, model.AnswerText);

                return RedirectToAction("Index", "Home");
            }

            return View();
        }
    }
}