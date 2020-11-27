﻿using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Web.ViewModels;

namespace Web.Controllers
{
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
        public IActionResult AddAnswer(int questionId)
        {
            var answerViewModel = new AnswerViewModel();
            answerViewModel.QuestionId = questionId;
            return View(answerViewModel);
        }

        // POST: AnswersController/AddAnswer
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("answers/addanswer/{questionId}")]
        public async Task<IActionResult> AddAnswer(AnswerViewModel answerViewModel)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = null;
                if (answerViewModel.Image != null && _fileOperations.ValidateImageType(answerViewModel.Image.FileName) == true)
                {
                    // for more advanced projects add a composite file provider - for now wwwroot
                    // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/file-providers?view=aspnetcore-5.0#compositefileprovider
                    string serverImagesDirectory = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                    uniqueFileName = _fileOperations.AssembleAnswerUploadedFileName(answerViewModel.QuestionId, answerViewModel.Image.FileName);
                    string filePath = Path.Combine(serverImagesDirectory, uniqueFileName);
                    await answerViewModel.Image.CopyToAsync(new FileStream(filePath, FileMode.Create));
                }
                var currentlyLoggedInUser = await _userManager.GetUserAsync(User);
                answerViewModel.UserId = currentlyLoggedInUser.Id;
                var answer = _mapper.Map<AnswerViewModel, Answer>(answerViewModel);
                answer.ImageNamePath = uniqueFileName;
                await _repository.AddAnswerAsync(answer);
                return RedirectToAction("Details", "Questions", new { questionId = answerViewModel.QuestionId });
            }
            return View(answerViewModel);
        }

        // GET: AnswersController/5/Edit
        [HttpGet]
        [Route("answers/{answerId}/edit")]
        public async Task<IActionResult> EditAnswer(int answerId)
        {
            var answer = await _repository.GetAnswerByIdWithoutDetailsAsync(answerId);
            var answerViewModel = _mapper.Map<Answer, AnswerViewModel>(answer);
            return View(answerViewModel);
        }

        // POST: AnswersController/5/Edit
        [HttpPost]
        [Route("answers/{answerId}/edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAnswer(AnswerViewModel answerViewModel)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = null;
                if (answerViewModel.Image != null)
                {
                    // for more advanced projects add a composite file provider - for now wwwroot
                    // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/file-providers?view=aspnetcore-5.0#compositefileprovider
                    string serverImagesDirectory = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                    uniqueFileName = _fileOperations.AssembleAnswerUploadedFileName(answerViewModel.QuestionId, answerViewModel.Image.FileName);
                    string filePath = Path.Combine(serverImagesDirectory, uniqueFileName);
                    await answerViewModel.Image.CopyToAsync(new FileStream(filePath, FileMode.Create));
                }
                var answer = _mapper.Map<AnswerViewModel, Answer>(answerViewModel);
                answer.ImageNamePath = uniqueFileName;
                await _repository.EditAnswerAsync(answer);
                return RedirectToAction("Details", "Questions", new { questionId = answerViewModel.QuestionId });
            }
            return View(answerViewModel);
        }

        // Get: AnswersController/5/Remove
        [HttpGet]
        [Route("answers/remove/{answerId}")]
        public async Task<IActionResult> Remove(int answerId, int questionId)
        {
            await _repository.RemoveAnswerById(answerId);

            return RedirectToAction("Details", "Questions", new { questionId = questionId });
        }

        // Get: AnswersController/5/VoteUp
        [HttpGet]
        [Route("answers/{answerId}/voteup")]
        public async Task<IActionResult> VoteUpAnswer(int answerId, int questionId)
        {
            await _repository.VoteUpAnswerById(answerId);

            return RedirectToAction("Details", "Questions", new { questionId = questionId });
        }

        // Get: AnswersController/5/VoteDown
        [HttpGet]
        [Route("answers/{answerId}/votedown")]
        public async Task<IActionResult> VoteDownAnswer(int answerId, int questionId)
        {
            await _repository.VoteDownAnswerById(answerId);

            return RedirectToAction("Details", "Questions", new { questionId = questionId });
        }

        // GET: AnswersController/5/Accept
        [HttpGet]
        [Route("answers/{answerId}/accept")]
        public async Task<IActionResult> AcceptAnswer(int answerId, int questionId)
        {
            await _repository.EditAnswerAccepted(answerId);
            return RedirectToAction("Details", "Questions", new { questionId = questionId });
        }
    }
}
