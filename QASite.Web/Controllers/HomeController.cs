using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using QASite.Data;
using QASite.Web.Models;

namespace QASite.Web.Controllers
{
    public class HomeController : Controller
    {
        private string _connectionString;

        public HomeController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConStr");
        }
        public IActionResult Index()
        {
            var repo = new QARepository(_connectionString);
            return View(repo.GetQuestions());
        }

        public IActionResult ViewQuestion(int id)
        {
            var repo = new QARepository(_connectionString);
            var question = repo.GetQuestionById(id);

            if (question == null)
            {
                return RedirectToAction("Index");
            }

            return View(new ViewQuestionViewModel { Question = question });
        }
        [HttpPost]
        public IActionResult AddAnswer(Answer answer)
        {
            var userRepo = new UserRepository(_connectionString);
            var currentUser = userRepo.GetByEmail(User.Identity.Name);

            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            answer.UserId = currentUser.Id;
            answer.DatePosted = DateTime.Now;

            var qaRepo = new QARepository(_connectionString);
            qaRepo.AddAnswer(answer);

            return RedirectToAction("ViewQuestion", "Home", new { id = answer.QuestionId });
        }
        [HttpPost]
        public IActionResult AddQuestion(Question question, List<string> tags)
        {
            var userRepo = new UserRepository(_connectionString);
            var currentUser = userRepo.GetByEmail(User.Identity.Name);

            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            question.UserId = currentUser.Id;
            question.DatePosted = DateTime.Now;

            var repo = new QARepository(_connectionString);
            repo.AddQuestion(question, tags);
            return RedirectToAction("Index");


        }

        public IActionResult AskAQuestion()
        {
            return View();
        }
    }
}
