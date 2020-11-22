using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.ViewModels;

namespace Web.Controllers
{
    public class QuestionsController : Controller
    {
        private readonly IAsyncRepository _repository;
        private readonly IMapper _mapper;

        public QuestionsController(IAsyncRepository repository,
                                   IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
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
                var question = _mapper.Map<QuestionViewModel, Question>(questionViewModel);
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

        // GET: QuestionsController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: QuestionsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
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

        // GET: QuestionsController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: QuestionsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
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
    }
}
