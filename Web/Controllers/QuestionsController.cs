using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Web.ViewModels;

namespace Web.Controllers
{
    public class QuestionsController : Controller
    {
        private readonly IAsyncRepository _repository;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public QuestionsController(IAsyncRepository repository,
                                   IMapper mapper,
                                   IWebHostEnvironment webHostEnvironment)
        {
            _repository = repository;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
        }
        // GET: QuestionsController
        public ActionResult Index()
        {
            return View();
        }

        // GET: QuestionsController/Details/5
        [Route("questions/{questionId}")]
        public async Task<IActionResult> Details(int questionId)
        {
            var question = await _repository.GetQuestionByIdAsync(questionId);
            var answersViewModel = new List<AnswerViewModel>();
            if (question.Answers != null && question.Answers.Count != 0)
            {
                answersViewModel = _mapper.Map<List<Answer>, List<AnswerViewModel>>(question.Answers);
            }
            var questionViewModel = _mapper.Map<Question, QuestionViewModel>(question);
            questionViewModel.Answers = answersViewModel;
            return View(questionViewModel);
        }

        // GET: QuestionsController/Create
        [HttpGet]
        public IActionResult AddQuestion()
        {
            return View();
        }

        // POST: QuestionsController/AddQuestion
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddQuestion(QuestionViewModel questionViewModel)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = null;
                if (questionViewModel.Image != null)
                {
                    // for more advanced projects add a composite file provider - for now wwwroot
                    // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/file-providers?view=aspnetcore-5.0#compositefileprovider
                    string serverImagesDirectory = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                    uniqueFileName = "QTitle_" 
                                    + questionViewModel.Title + "_" 
                                    + DateTime.Now.Year.ToString() + "_" 
                                    + DateTime.Now.Month.ToString() + "_" 
                                    + DateTime.Now.Day.ToString() + "_" 
                                    + DateTime.Now.Hour.ToString() + "_" 
                                    + DateTime.Now.Minute.ToString() + "_" 
                                    + DateTime.Now.Second.ToString() + "_" 
                                    + Guid.NewGuid().ToString() + "_" + questionViewModel.Image.FileName;
                    string filePath = Path.Combine(serverImagesDirectory, uniqueFileName);
                    await questionViewModel.Image.CopyToAsync(new FileStream(filePath, FileMode.Create));
                }
                var question = _mapper.Map<QuestionViewModel, Question>(questionViewModel);
                question.ImageNamePath = uniqueFileName;
                var resultQuestion = await _repository.AddQuestionAsync(question);
                return RedirectToAction("Details", new { questionId = resultQuestion.Id });
            }
            return View();
        }

        // POST: QuestionsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: QuestionsController/5/Edit
        [HttpGet]
        [Route("questions/{questionId}/edit")]
        public async Task<IActionResult> Edit(int questionId)
        {
            var question = await _repository.GetQuestionByIdWithoutDetailsAsync(questionId);
            var questionViewModel = _mapper.Map<Question, QuestionViewModel>(question);
            return View(questionViewModel);
        }

        // POST: QuestionsController/5/Edit
        [HttpPost]
        [Route("questions/{questionId}/edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(QuestionViewModel questionViewModel)
        {
            if (ModelState.IsValid)
            {
                var question = _mapper.Map<QuestionViewModel, Question>(questionViewModel);
                await _repository.EditQuestionAsync(question);
                return RedirectToAction("Details", new { questionId = questionViewModel.Id });
            }
            return View(questionViewModel.Id);
        }

        // Get: QuestionsController/5/Remove
        [HttpGet]
        [Route("questions/remove/{questionId}")]
        public async Task<IActionResult> Remove(int questionId)
        {
            await _repository.RemoveQuestionById(questionId);
            
            return RedirectToAction("Index", "List");
        }

        // Get: QuestionsController/5/VoteUp
        [HttpGet]
        [Route("questions/{questionId}/voteup")]
        public async Task<IActionResult> VoteUpQuestion(int questionId)
        {
            await _repository.VoteUpQuestionById(questionId);

            return RedirectToAction("Index", "List");
        }

        // Get: QuestionsController/5/VoteDown
        [HttpGet]
        [Route("questions/{questionId}/votedown")]
        public async Task<IActionResult> VoteDownQuestion(int questionId)
        {
            await _repository.VoteDownQuestionById(questionId);

            return RedirectToAction("Index", "List");
        }
    }
}
