﻿using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.ViewModels;

namespace Web.Controllers
{
    public class UserController : Controller
    {
        private readonly IAsyncRepository _repository;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(IAsyncRepository repository,
                              IMapper mapper,
                              UserManager<ApplicationUser> userManager)
        {
            _repository = repository;
            _mapper = mapper;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> AllUsers()
        {
            List<ApplicationUser> allUsersFromDb = await _repository.GetAllUsers();
            var allUsersViewModel = _mapper.Map <List<ApplicationUser>, List<ApplicationUserViewModel>>(allUsersFromDb);
            return View(allUsersViewModel);
        }

        [HttpGet]
        [Route("activity")]
        public async Task<IActionResult> UserActivity()
        {
            var currentlyLoggedInUser = await _userManager.GetUserAsync(User);
            string userId = currentlyLoggedInUser.Id;

            List<Object> allActivities = new List<object>();
            
            var userQuestionsFromDb = await _repository.GetUserQuestions(userId);
            List<QuestionViewModel> userQuestionsViewModel = _mapper.Map<List<Question>, List<QuestionViewModel>>(userQuestionsFromDb);

            var userAnswersFromDb = await _repository.GetUserAnswers(userId);
            List<AnswerViewModel> userAnswersViewModel = _mapper.Map<List<Answer>, List<AnswerViewModel>>(userAnswersFromDb);

            var userQuestionCommentsFromDb = await _repository.GetUserQuestionComments(userId);
            List<QuestionCommentViewModel> userQuestionCommentsViewModel = _mapper.Map<List<QuestionComment>, List<QuestionCommentViewModel>>(userQuestionCommentsFromDb);

            var userAnswerCommentsFromDb = await _repository.GetUserAnswerComments(userId);
            List<AnswerCommentViewModel> userAnswerCommentsViewModel = _mapper.Map<List<AnswerComment>, List<AnswerCommentViewModel>>(userAnswerCommentsFromDb);

            // arrange in List to send to View - ORDER MATTERS
            allActivities.Add(userQuestionsViewModel);
            allActivities.Add(userAnswersViewModel);
            allActivities.Add(userQuestionCommentsViewModel);
            allActivities.Add(userAnswerCommentsViewModel);

            return View(allActivities);
        }
    }
}
