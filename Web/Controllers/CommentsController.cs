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
    public class CommentsController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IAsyncRepository _repository;

        public CommentsController(IMapper mapper,
                                  IAsyncRepository repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        // Get: comments/addQuestionComment/{questionId}
        [HttpGet]
        [Route("comments/addQuestionComment/{questionId}")]
        public IActionResult AddQuestionComment(int questionId)
        {
            var questionCommentViewModel = new QuestionCommentViewModel();
            questionCommentViewModel.QuestionId = questionId;
            return View(questionCommentViewModel);
        }

        // Get: comments/addAnswerComment/{answerId}
        [HttpGet]
        [Route("comments/addAnswerComment/{answerId}")]
        public IActionResult AddAnswerComment(int answerId, int questionId)
        {
            var answerCommentViewModel = new AnswerCommentViewModel();
            answerCommentViewModel.QuestionId = questionId;
            answerCommentViewModel.AnswerId = answerId;
            return View(answerCommentViewModel);
        }

        // POST: comments/addQuestionComment/{questionId}
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("comments/addQuestionComment/{questionId}")]
        public async Task<IActionResult> AddQuestionComment(QuestionCommentViewModel questionCommentViewModel)
        {
            if (ModelState.IsValid)
            {
                var questionComment = _mapper.Map<QuestionCommentViewModel, QuestionComment>(questionCommentViewModel);
                await _repository.AddQuestionCommentAsync(questionComment);
                return RedirectToAction("Details", "Questions", new { questionId = questionCommentViewModel.QuestionId });
            }
            return View(questionCommentViewModel);
        }

        // GET: CommentsController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: CommentsController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CommentsController/Create
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

        // GET: CommentsController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: CommentsController/Edit/5
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

        // GET: CommentsController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: CommentsController/Delete/5
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
