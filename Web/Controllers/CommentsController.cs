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

        // POST: comments/addAnswerComment/{answerId}
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("comments/addAnswerComment/{answerId}")]
        public async Task<IActionResult> AddAnswerComment(AnswerCommentViewModel answerCommentViewModel)
        {
            if (ModelState.IsValid)
            {
                var answerComment = _mapper.Map<AnswerCommentViewModel, AnswerComment>(answerCommentViewModel);
                await _repository.AddAnswerCommentAsync(answerComment);
                return RedirectToAction("Details", "Questions", new { questionId = answerCommentViewModel.QuestionId });
            }
            return View(answerCommentViewModel);
        }

        // GET: CommentsController/answerComments/5/Edit
        [HttpGet]
        [Route("comments/answerComments/{answerCommentId}/edit")]
        public async Task<IActionResult> EditAnswerComment(int answerCommentId)
        {
            var answerComment = await _repository.GetAnswerCommentById(answerCommentId);
            var answerCommentViewModel = _mapper.Map<AnswerComment, AnswerCommentViewModel>(answerComment);
            return View(answerCommentViewModel);
        }

        // GET: CommentsController/QuestionComments/5/Edit
        [HttpGet]
        [Route("comments/questionComments/{questionCommentId}/edit")]
        public async Task<IActionResult> EditQuestionComment(int questionCommentId)
        {
            var questionComment = await _repository.GetQuestionCommentById(questionCommentId);
            var questionCommentViewModel = _mapper.Map<QuestionComment, QuestionCommentViewModel>(questionComment);
            return View(questionCommentViewModel);
        }

        // POST: CommentsController/AnswerComments/5/Edit
        [HttpPost]
        [Route("comments/answerComments/{answerCommentId}/edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAnswerComment(AnswerCommentViewModel answerCommentViewModel)
        {
            if (ModelState.IsValid)
            {
                var answerComment = _mapper.Map<AnswerCommentViewModel, AnswerComment>(answerCommentViewModel);
                // adding the "Edited" mark and refreshing the DateAdded
                // this replaces the old Body
                // to keep old data, mark old comment as IsRemoved and add the new one w "Edited" mark
                answerComment.IsEdited = true;
                answerComment.DateAdded = DateTime.Now;
                await _repository.EditAnswerCommentAsync(answerComment);
                return RedirectToAction("Details", "Questions", new { questionId = answerCommentViewModel.QuestionId });
            }
            return View(answerCommentViewModel);
        }

        // POST: CommentsController/QuestionComments/5/Edit
        [HttpPost]
        [Route("comments/questionComments/{questionCommentId}/edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditQuestionComment(QuestionCommentViewModel questionCommentViewModel)
        {
            if (ModelState.IsValid)
            {
                var questionComment = _mapper.Map<QuestionCommentViewModel, QuestionComment>(questionCommentViewModel);
                // adding the "Edited" mark and refreshing the DateAdded
                // this replaces the old Body
                // to keep old data, mark old comment as IsRemoved and add the new one w "Edited" mark
                questionComment.IsEdited = true;
                questionComment.DateAdded = DateTime.Now;
                await _repository.EditQuestionCommentAsync(questionComment);
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
