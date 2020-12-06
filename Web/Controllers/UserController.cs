using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.ViewModels;

namespace Web.Controllers
{
    [Authorize]
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
            try
            {
                List<ApplicationUser> allUsersFromDb = await _repository.GetAllUsers();
                var allUsersViewModel = _mapper.Map<List<ApplicationUser>, List<ApplicationUserViewModel>>(allUsersFromDb);
                return View("AllUsers", allUsersViewModel);
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

        [HttpGet]
        [Route("activity")]
        public async Task<IActionResult> UserActivity()
        {
            try
            {
                var currentlyLoggedInUser = await _userManager.GetUserAsync(User);
                string userId = currentlyLoggedInUser.Id;

                var userQuestionsFromDb = await _repository.GetUserQuestions(userId);
                List<QuestionViewModel> userQuestionsViewModel = _mapper.Map<List<Question>, List<QuestionViewModel>>(userQuestionsFromDb);

                var userAnswersFromDb = await _repository.GetUserAnswers(userId);
                List<AnswerViewModel> userAnswersViewModel = _mapper.Map<List<Answer>, List<AnswerViewModel>>(userAnswersFromDb);

                var userQuestionCommentsFromDb = await _repository.GetUserQuestionComments(userId);
                List<QuestionCommentViewModel> userQuestionCommentsViewModel = _mapper.Map<List<QuestionComment>, List<QuestionCommentViewModel>>(userQuestionCommentsFromDb);

                var userAnswerCommentsFromDb = await _repository.GetUserAnswerComments(userId);
                List<AnswerCommentViewModel> userAnswerCommentsViewModel = _mapper.Map<List<AnswerComment>, List<AnswerCommentViewModel>>(userAnswerCommentsFromDb);

                UserActivitiesViewModel allUserActivities = new UserActivitiesViewModel();
                allUserActivities.Questions = userQuestionsViewModel;
                allUserActivities.Answers = userAnswersViewModel;
                allUserActivities.QuestionComments = userQuestionCommentsViewModel;
                allUserActivities.AnswerComments = userAnswerCommentsViewModel;

                return View(allUserActivities);
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
    }
}
