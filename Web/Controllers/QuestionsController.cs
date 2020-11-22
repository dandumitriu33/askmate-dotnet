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
            var questionsViewModel = _mapper.Map<Question, QuestionViewModel>(question);
            return View(questionsViewModel);
        }

        // GET: QuestionsController/Create
        public ActionResult AddQuestion()
        {
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
