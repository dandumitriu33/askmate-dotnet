﻿using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Web.ViewModels;

namespace Web.Controllers
{
    [Authorize]
    public class AnswersController : Controller
    {
        private readonly IAsyncRepository _repository;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IFileOperations _fileOperations;
        private readonly UserManager<ApplicationUser> _userManager;

        public AnswersController(IAsyncRepository repository,
                                 IMapper mapper,
                                 IWebHostEnvironment webHostEnvironment,
                                 IFileOperations fileOperations,
                                 UserManager<ApplicationUser> userManager)
        {
            _repository = repository;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
            _fileOperations = fileOperations;
            _userManager = userManager;
        }

        // Get
        [HttpGet]
        [Route("answers/addanswer/{questionId}")]
        public async Task<IActionResult> AddAnswer(int questionId)
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
            var answerViewModel = new AnswerViewModel();
            answerViewModel.QuestionId = questionId;
            return View("AddAnswer", answerViewModel);
        }

        // POST: AnswersController/AddAnswer
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("answers/addanswer/{questionId}")]
        public async Task<IActionResult> AddAnswer(AnswerViewModel answerViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var question = await _repository.GetQuestionByIdWithoutDetailsAsync(answerViewModel.QuestionId);
                    if (question == null)
                    {
                        Response.StatusCode = 404;
                        ViewData["ErrorMessage"] = "404 Resource not found.";
                        return View("Error");
                    }
                    var currentlyLoggedInUser = await _userManager.GetUserAsync(User);
                    answerViewModel.UserId = currentlyLoggedInUser.Id;
                    string uniqueFileName = null;
                    if (answerViewModel.Image != null && _fileOperations.ValidateImageType(answerViewModel.Image.FileName) == true)
                    {
                        uniqueFileName = await SetPathAndUpload(answerViewModel);
                    }
                    var answer = _mapper.Map<AnswerViewModel, Answer>(answerViewModel);
                    answer.ImageNamePath = uniqueFileName;
                    await _repository.AddAnswerAsync(answer);
                    return RedirectToAction("Details", "Questions", new { questionId = answerViewModel.QuestionId });
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
            return View("AddAnswer", answerViewModel);
        }

        // GET: AnswersController/5/Edit
        [HttpGet]
        [Route("answers/{answerId}/edit")]
        public async Task<IActionResult> EditAnswer(int answerId)
        {
            try
            {
                var answer = await _repository.GetAnswerByIdWithoutDetailsAsync(answerId);
                if (answer == null)
                {
                    Response.StatusCode = 404;
                    ViewData["ErrorMessage"] = "404 Resource not found.";
                    return View("Error");
                }
                var question = await _repository.GetQuestionByIdWithoutDetailsAsync(answer.QuestionId);
                if (question == null)
                {
                    Response.StatusCode = 404;
                    ViewData["ErrorMessage"] = "404 Resource not found.";
                    return View("Error");
                }
                if (String.Equals(User.FindFirstValue(ClaimTypes.NameIdentifier), answer.UserId) == false)
                {
                    return RedirectToAction("AccessDenied", "Account");
                }
                var answerViewModel = _mapper.Map<Answer, AnswerViewModel>(answer);
                return View("EditAnswer", answerViewModel);
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

        // POST: AnswersController/5/Edit
        [HttpPost]
        [Route("answers/{answerId}/edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAnswer(AnswerViewModel answerViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var answer = await _repository.GetAnswerByIdWithoutDetailsAsync(answerViewModel.Id);
                    if (answer == null)
                    {
                        Response.StatusCode = 404;
                        ViewData["ErrorMessage"] = "404 Resource not found.";
                        return View("Error");
                    }
                    var question = await _repository.GetQuestionByIdWithoutDetailsAsync(answer.QuestionId);
                    if (question == null)
                    {
                        Response.StatusCode = 404;
                        ViewData["ErrorMessage"] = "404 Resource not found.";
                        return View("Error");
                    }
                    if (String.Equals(User.FindFirstValue(ClaimTypes.NameIdentifier), answer.UserId) == false)
                    {
                        return RedirectToAction("AccessDenied", "Account");
                    }
                    var currentlyLoggedInUser = await _userManager.GetUserAsync(User);
                    answerViewModel.UserId = currentlyLoggedInUser.Id;
                    string uniqueFileName = null;
                    if (answerViewModel.Image != null && _fileOperations.ValidateImageType(answerViewModel.Image.FileName) == true)
                    {
                        uniqueFileName = await SetPathAndUpload(answerViewModel);
                    }
                    else if (answerViewModel.Image != null && _fileOperations.ValidateImageType(answerViewModel.Image.FileName) == false)
                    {
                        return View("EditAnswer", answerViewModel);
                    }
                    answer = _mapper.Map<AnswerViewModel, Answer>(answerViewModel);
                    answer.ImageNamePath = uniqueFileName;
                    await _repository.EditAnswerAsync(answer);
                    return RedirectToAction("Details", "Questions", new { questionId = answerViewModel.QuestionId });
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
            return View("EditAnswer", answerViewModel);
        }

        // Get: AnswersController/5/Remove
        [HttpGet]
        [Route("answers/remove/{answerId}")]
        public async Task<IActionResult> Remove(int answerId, int questionId)
        {
            try
            {
                var answer = await _repository.GetAnswerByIdWithoutDetailsAsync(answerId);
                if (answer == null)
                {
                    Response.StatusCode = 404;
                    ViewData["ErrorMessage"] = "404 Resource not found.";
                    return View("Error");
                }
                var question = await _repository.GetQuestionByIdWithoutDetailsAsync(answer.QuestionId);
                if (question == null)
                {
                    Response.StatusCode = 404;
                    ViewData["ErrorMessage"] = "404 Resource not found.";
                    return View("Error");
                }
                if (String.Equals(User.FindFirstValue(ClaimTypes.NameIdentifier), answer.UserId) == false)
                {
                    return RedirectToAction("AccessDenied", "Account");
                }
                await _repository.RemoveAnswerById(answerId);
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

        // Get: AnswersController/5/RemoveImage
        [HttpGet]
        [Route("answers/removeimage/{answerId}")]
        public async Task<IActionResult> RemoveImage(int answerId, int questionId)
        {
            try
            {
                var answer = await _repository.GetAnswerByIdWithoutDetailsAsync(answerId);
                if (answer == null)
                {
                    Response.StatusCode = 404;
                    ViewData["ErrorMessage"] = "404 Resource not found.";
                    return View("Error");
                }
                var question = await _repository.GetQuestionByIdWithoutDetailsAsync(answer.QuestionId);
                if (question == null)
                {
                    Response.StatusCode = 404;
                    ViewData["ErrorMessage"] = "404 Resource not found.";
                    return View("Error");
                }
                if (String.Equals(User.FindFirstValue(ClaimTypes.NameIdentifier), answer.UserId) == false)
                {
                    return RedirectToAction("AccessDenied", "Account");
                }
                await _repository.RemoveAnswerImageByAnswerId(answerId);
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

        // Get: AnswersController/5/VoteUp
        [HttpGet]
        [Route("answers/{answerId}/voteup")]
        public async Task<IActionResult> VoteUpAnswer(int answerId, int questionId)
        {
            try
            {
                var answer = await _repository.GetAnswerByIdWithoutDetailsAsync(answerId);
                if (answer == null)
                {
                    Response.StatusCode = 404;
                    ViewData["ErrorMessage"] = "404 Resource not found.";
                    return View("Error");
                }
                await _repository.VoteUpAnswerById(answerId);
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

        // Get: AnswersController/5/VoteDown
        [HttpGet]
        [Route("answers/{answerId}/votedown")]
        public async Task<IActionResult> VoteDownAnswer(int answerId, int questionId)
        {
            try
            {
                var answer = await _repository.GetAnswerByIdWithoutDetailsAsync(answerId);
                if (answer == null)
                {
                    Response.StatusCode = 404;
                    ViewData["ErrorMessage"] = "404 Resource not found.";
                    return View("Error");
                }
                await _repository.VoteDownAnswerById(answerId);
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

        // GET: AnswersController/5/Accept
        [HttpGet]
        [Route("answers/{answerId}/accept")]
        public async Task<IActionResult> AcceptAnswer(int answerId, int questionId)
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
            if (String.Equals(User.FindFirstValue(ClaimTypes.NameIdentifier), question.UserId) == false)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            try
            {
                await _repository.EditAnswerAccepted(answerId);
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

        // helper methods
        private async Task<string> SetPathAndUpload(AnswerViewModel answerViewModel)
        {
            // for more advanced projects add a composite file provider - for now wwwroot
            // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/file-providers?view=aspnetcore-5.0#compositefileprovider
            string serverImagesDirectory = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
            string uniqueFileName = _fileOperations.AssembleAnswerUploadedFileName(answerViewModel.UserId, answerViewModel.Image.FileName);
            string filePath = Path.Combine(serverImagesDirectory, uniqueFileName);
            await answerViewModel.Image.CopyToAsync(new FileStream(filePath, FileMode.Create));
            return uniqueFileName;
        }

    }
}
