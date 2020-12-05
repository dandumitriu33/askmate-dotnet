using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Web.ViewModels;

namespace Web.Controllers
{
    [Authorize]
    public class CommentsController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IAsyncRepository _repository;
        private readonly UserManager<ApplicationUser> _userManager;

        public CommentsController(IMapper mapper,
                                  IAsyncRepository repository,
                                  UserManager<ApplicationUser> userManager)
        {
            _mapper = mapper;
            _repository = repository;
            _userManager = userManager;
        }

        // Get: comments/addQuestionComment/{questionId}
        [HttpGet]
        [Route("comments/addQuestionComment/{questionId}")]
        public async Task<IActionResult> AddQuestionComment(int questionId)
        {
            try
            {
                var question = await _repository.GetQuestionByIdWithoutDetailsAsync(questionId);
                if (question == null)
                {
                    Response.StatusCode = 404;
                    ViewData["ErrorMessage"] = "404 Resource not found.";
                    return View("Error");
                }
            }
            catch (DbUpdateException dbex)
            {
                ViewData["ErrorMessage"] = "DB issue - " + dbex.Message;
                return View("Error");
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Message;
                return View("Error");
            }
            var questionCommentViewModel = new QuestionCommentViewModel();
            questionCommentViewModel.QuestionId = questionId;
            return View("AddQuestionComment", questionCommentViewModel);
        }

        // Get: comments/addAnswerComment/{answerId}
        [HttpGet]
        [Route("comments/addAnswerComment/{answerId}")]
        public async Task<IActionResult> AddAnswerComment(int answerId, int questionId)
        {
            var answer = await _repository.GetAnswerByIdWithoutDetailsAsync(answerId);
            if (answer == null)
            {
                Response.StatusCode = 404;
                ViewData["ErrorMessage"] = "404 Resource not found.";
                return View("Error");
            }
            var question = await _repository.GetQuestionByIdWithoutDetailsAsync(questionId);
            if (question == null)
            {
                Response.StatusCode = 404;
                ViewData["ErrorMessage"] = "404 Resource not found.";
                return View("Error");
            }
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
            var question = await _repository.GetQuestionByIdWithoutDetailsAsync(questionCommentViewModel.QuestionId);
            if (question == null)
            {
                Response.StatusCode = 404;
                ViewData["ErrorMessage"] = "404 Resource not found.";
                return View("Error");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    var currentlyLoggedInUser = await _userManager.GetUserAsync(User);
                    questionCommentViewModel.UserId = currentlyLoggedInUser.Id;
                    var questionComment = _mapper.Map<QuestionCommentViewModel, QuestionComment>(questionCommentViewModel);
                    await _repository.AddQuestionCommentAsync(questionComment);
                    return RedirectToAction("Details", "Questions", new { questionId = questionCommentViewModel.QuestionId });
                }
                catch (DbUpdateException dbex)
                {
                    ViewData["ErrorMessage"] = "DB issue - " + dbex.Message;
                    return View("Error");
                }
                catch (Exception ex)
                {
                    ViewData["ErrorMessage"] = ex.Message;
                    return View("Error");
                }
            }
            return View(questionCommentViewModel);
        }

        // POST: comments/addAnswerComment/{answerId}
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("comments/addAnswerComment/{answerId}")]
        public async Task<IActionResult> AddAnswerComment(AnswerCommentViewModel answerCommentViewModel)
        {
            var answer = await _repository.GetAnswerByIdWithoutDetailsAsync(answerCommentViewModel.AnswerId);
            if (answer == null)
            {
                Response.StatusCode = 404;
                ViewData["ErrorMessage"] = "404 Resource not found.";
                return View("Error");
            }
            var question = await _repository.GetQuestionByIdWithoutDetailsAsync(answerCommentViewModel.QuestionId);
            if (question == null)
            {
                Response.StatusCode = 404;
                ViewData["ErrorMessage"] = "404 Resource not found.";
                return View("Error");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    var currentlyLoggedInUser = await _userManager.GetUserAsync(User);
                    answerCommentViewModel.UserId = currentlyLoggedInUser.Id;
                    var answerComment = _mapper.Map<AnswerCommentViewModel, AnswerComment>(answerCommentViewModel);
                    await _repository.AddAnswerCommentAsync(answerComment);
                    return RedirectToAction("Details", "Questions", new { questionId = answerCommentViewModel.QuestionId });
                }
                catch (DbUpdateException dbex)
                {
                    ViewData["ErrorMessage"] = "DB issue - " + dbex.Message;
                    return View("Error");
                }
                catch (Exception ex)
                {
                    ViewData["ErrorMessage"] = ex.Message;
                    return View("Error");
                }
            }
            return View(answerCommentViewModel);
        }

        // GET: CommentsController/QuestionComments/5/Edit
        [HttpGet]
        [Route("comments/questionComments/{questionCommentId}/edit")]
        public async Task<IActionResult> EditQuestionComment(int questionCommentId)
        {
            var questionComment = await _repository.GetQuestionCommentById(questionCommentId);
            if (questionComment == null)
            {
                Response.StatusCode = 404;
                ViewData["ErrorMessage"] = "404 Resource not found.";
                return View("Error");
            }
            var question = await _repository.GetQuestionByIdWithoutDetailsAsync(questionComment.QuestionId);
            if (question == null)
            {
                Response.StatusCode = 404;
                ViewData["ErrorMessage"] = "404 Resource not found.";
                return View("Error");
            }            
            if (String.Equals(User.FindFirstValue(ClaimTypes.NameIdentifier), questionComment.UserId) == false)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            var questionCommentViewModel = _mapper.Map<QuestionComment, QuestionCommentViewModel>(questionComment);
            return View(questionCommentViewModel);
        }

        // GET: CommentsController/answerComments/5/Edit
        [HttpGet]
        [Route("comments/answerComments/{answerCommentId}/edit")]
        public async Task<IActionResult> EditAnswerComment(int answerCommentId)
        {
            var answerComment = await _repository.GetAnswerCommentById(answerCommentId);
            if (answerComment == null)
            {
                Response.StatusCode = 404;
                ViewData["ErrorMessage"] = "404 Resource not found.";
                return View("Error");
            }
            var question = await _repository.GetQuestionByIdWithoutDetailsAsync(answerComment.QuestionId);
            if (question == null)
            {
                Response.StatusCode = 404;
                ViewData["ErrorMessage"] = "404 Resource not found.";
                return View("Error");
            }
            var answer = await _repository.GetAnswerByIdWithoutDetailsAsync(answerComment.AnswerId);
            if (answer == null)
            {
                Response.StatusCode = 404;
                ViewData["ErrorMessage"] = "404 Resource not found.";
                return View("Error");
            }
            if (String.Equals(User.FindFirstValue(ClaimTypes.NameIdentifier), answerComment.UserId) == false)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            var answerCommentViewModel = _mapper.Map<AnswerComment, AnswerCommentViewModel>(answerComment);
            return View(answerCommentViewModel);
        }

        // POST: CommentsController/QuestionComments/5/Edit
        [HttpPost]
        [Route("comments/questionComments/{questionCommentId}/edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditQuestionComment(QuestionCommentViewModel questionCommentViewModel)
        {
            var questionComment = await _repository.GetQuestionCommentById(questionCommentViewModel.Id);
            if (questionComment == null)
            {
                Response.StatusCode = 404;
                ViewData["ErrorMessage"] = "404 Resource not found.";
                return View("Error");
            }
            var question = await _repository.GetQuestionByIdWithoutDetailsAsync(questionComment.QuestionId);
            if (question == null)
            {
                Response.StatusCode = 404;
                ViewData["ErrorMessage"] = "404 Resource not found.";
                return View("Error");
            }
            if (String.Equals(User.FindFirstValue(ClaimTypes.NameIdentifier), questionComment.UserId) == false)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    questionComment = _mapper.Map<QuestionCommentViewModel, QuestionComment>(questionCommentViewModel);
                    // adding the "Edited" mark and refreshing the DateAdded
                    // this replaces the old Body
                    // to keep old data, mark old comment as IsRemoved and add the new one w "Edited" mark
                    questionComment.IsEdited = true;
                    questionComment.DateAdded = DateTime.Now;
                    await _repository.EditQuestionCommentAsync(questionComment);
                    return RedirectToAction("Details", "Questions", new { questionId = questionCommentViewModel.QuestionId });
                }
                catch (DbUpdateException dbex)
                {
                    ViewData["ErrorMessage"] = "DB issue - " + dbex.Message;
                    return View("Error");
                }
                catch (Exception ex)
                {
                    ViewData["ErrorMessage"] = ex.Message;
                    return View("Error");
                }
            }
            return View(questionCommentViewModel);
        }

        // POST: CommentsController/AnswerComments/5/Edit
        [HttpPost]
        [Route("comments/answerComments/{answerCommentId}/edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAnswerComment(AnswerCommentViewModel answerCommentViewModel)
        {
            var answerComment = await _repository.GetAnswerCommentById(answerCommentViewModel.Id);
            if (answerComment == null)
            {
                Response.StatusCode = 404;
                ViewData["ErrorMessage"] = "404 Resource not found.";
                return View("Error");
            }
            var question = await _repository.GetQuestionByIdWithoutDetailsAsync(answerComment.QuestionId);
            if (question == null)
            {
                Response.StatusCode = 404;
                ViewData["ErrorMessage"] = "404 Resource not found.";
                return View("Error");
            }
            var answer = await _repository.GetAnswerByIdWithoutDetailsAsync(answerComment.AnswerId);
            if (answer == null)
            {
                Response.StatusCode = 404;
                ViewData["ErrorMessage"] = "404 Resource not found.";
                return View("Error");
            }
            if (String.Equals(User.FindFirstValue(ClaimTypes.NameIdentifier), answerComment.UserId) == false)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    answerComment = _mapper.Map<AnswerCommentViewModel, AnswerComment>(answerCommentViewModel);
                    // adding the "Edited" mark and refreshing the DateAdded
                    // this replaces the old Body
                    // to keep old data, mark old comment as IsRemoved and add the new one w "Edited" mark
                    answerComment.IsEdited = true;
                    answerComment.DateAdded = DateTime.Now;
                    await _repository.EditAnswerCommentAsync(answerComment);
                    return RedirectToAction("Details", "Questions", new { questionId = answerCommentViewModel.QuestionId });
                }
                catch (DbUpdateException dbex)
                {
                    ViewData["ErrorMessage"] = "DB issue - " + dbex.Message;
                    return View("Error");
                }
                catch (Exception ex)
                {
                    ViewData["ErrorMessage"] = ex.Message;
                    return View("Error");
                }
            }
            return View(answerCommentViewModel);
        }

        // GET: CommentsController/QuestionComment/{questionCommentId}/Remove
        [HttpGet]
        [Route("comments/questionComments/{questionCommentId}/remove")]
        public async Task<IActionResult> RemoveQuestionComment(int questionCommentId, int questionId)
        {
            var questionComment = await _repository.GetQuestionCommentById(questionCommentId);
            if (questionComment == null)
            {
                Response.StatusCode = 404;
                ViewData["ErrorMessage"] = "404 Resource not found.";
                return View("Error");
            }
            var question = await _repository.GetQuestionByIdWithoutDetailsAsync(questionId);
            if (question == null)
            {
                Response.StatusCode = 404;
                ViewData["ErrorMessage"] = "404 Resource not found.";
                return View("Error");
            }
            if (String.Equals(User.FindFirstValue(ClaimTypes.NameIdentifier), questionComment.UserId) == false)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            try
            {
                await _repository.RemoveQuestionCommentById(questionCommentId);
            }
            catch (DbUpdateException dbex)
            {
                ViewData["ErrorMessage"] = "DB issue - " + dbex.Message;
                return View("Error");
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Message;
                return View("Error");
            }
            return RedirectToAction("Details", "Questions", new { questionId = questionId });
        }

        // GET: CommentsController/AnswerComment/{answerCommentId}/Remove
        [HttpGet]
        [Route("comments/answerComments/{answerCommentId}/remove")]
        public async Task<IActionResult> RemoveAnswerComment(int answerCommentId, int questionId)
        {
            var answerComment = await _repository.GetAnswerCommentById(answerCommentId);
            if (answerComment == null)
            {
                Response.StatusCode = 404;
                ViewData["ErrorMessage"] = "404 Resource not found.";
                return View("Error");
            }
            var question = await _repository.GetQuestionByIdWithoutDetailsAsync(questionId);
            if (question == null)
            {
                Response.StatusCode = 404;
                ViewData["ErrorMessage"] = "404 Resource not found.";
                return View("Error");
            }
            var answer = await _repository.GetAnswerByIdWithoutDetailsAsync(answerComment.AnswerId);
            if (answer == null)
            {
                Response.StatusCode = 404;
                ViewData["ErrorMessage"] = "404 Resource not found.";
                return View("Error");
            }
            if (String.Equals(User.FindFirstValue(ClaimTypes.NameIdentifier), answerComment.UserId) == false)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            try
            {
                await _repository.RemoveAnswerCommentById(answerCommentId);
            }
            catch (DbUpdateException dbex)
            {
                ViewData["ErrorMessage"] = "DB issue - " + dbex.Message;
                return View("Error");
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Message;
                return View("Error");
            }
            return RedirectToAction("Details", "Questions", new { questionId = questionId });
        }
        
    }
}
