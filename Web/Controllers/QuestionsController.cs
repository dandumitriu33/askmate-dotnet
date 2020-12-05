using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using Web.ViewModels;

namespace Web.Controllers
{
    [Authorize]
    public class QuestionsController : Controller
    {
        private readonly IAsyncRepository _repository;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IFileOperations _fileOperations;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public QuestionsController(IAsyncRepository repository,
                                   IMapper mapper,
                                   IWebHostEnvironment webHostEnvironment,
                                   IFileOperations fileOperations,
                                   SignInManager<ApplicationUser> signInManager,
                                   UserManager<ApplicationUser> userManager)
        {
            _repository = repository;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
            _fileOperations = fileOperations;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [AllowAnonymous]
        // GET: QuestionsController/Details/5
        [Route("questions/{questionId}")]
        public async Task<IActionResult> Details(int questionId)
        {
            try
            {
                var simpleQuestion = await _repository.GetQuestionByIdWithoutDetailsAsync(questionId);
                if (simpleQuestion == null)
                {
                    Response.StatusCode = 404;
                    ViewData["ErrorMessage"] = "404 Resource not found.";
                    return View("Error");
                }
                var question = await _repository.GetQuestionByIdAsync(questionId);

                // pack question comments
                var questionCommentsViewModel = new List<QuestionCommentViewModel>();
                if (question.QuestionComments != null && question.QuestionComments.Count != 0)
                {
                    questionCommentsViewModel = _mapper.Map<List<QuestionComment>, List<QuestionCommentViewModel>>(question.QuestionComments);
                }
                // pack Tags
                List<int> tagIds = await _repository.GetTagIdsForQuestionId(questionId);
                List<Tag> tagsFromDb = await _repository.GetTagsFromListFromDb(tagIds);
                var questionTagsViewModel = _mapper.Map<List<Tag>, List<TagViewModel>>(tagsFromDb);

                // pack answersVM with commentsVM
                var answersViewModel = new List<AnswerViewModel>(); // create answersVM List to attach to qVM

                if (question.Answers != null && question.Answers.Count != 0) // if the q has answers, proceed with process
                {
                    foreach (var answer in question.Answers) // for each answer (not VM) in the list
                    {
                        AnswerViewModel tempAnswerVM = _mapper.Map<Answer, AnswerViewModel>(answer);
                        // pack comments
                        if (answer.AnswerComments != null && answer.AnswerComments.Count != 0) // if the answer has comments, proceed with process
                        {
                            var answerCommentsVM = _mapper.Map<List<AnswerComment>, List<AnswerCommentViewModel>>(answer.AnswerComments);
                            tempAnswerVM.AnswerComments = answerCommentsVM;
                        }
                        // add tempAnswerVM to the list answersVM
                        answersViewModel.Add(tempAnswerVM);
                    }
                }
                var questionViewModel = _mapper.Map<Question, QuestionViewModel>(question);
                questionViewModel.Answers = answersViewModel;
                questionViewModel.QuestionComments = questionCommentsViewModel;
                questionViewModel.Tags = questionTagsViewModel;

                return View("Details", questionViewModel);
            }
            catch(DbUpdateException dbex)
            {
                ViewData["ErrorMessage"] = "DB issue - " + dbex.Message;
                return View("Error");
            }
            catch(Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Message;
                return View("Error");
            }
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
                try
                {
                    var currentlySignedInUser = await _userManager.GetUserAsync(User);
                    questionViewModel.UserId = currentlySignedInUser.Id;
                    string uniqueFileName = null;
                    if (questionViewModel.Image != null && _fileOperations.ValidateImageType(questionViewModel.Image.FileName) == true)
                    {
                        // for more advanced projects add a composite file provider - for now wwwroot
                        // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/file-providers?view=aspnetcore-5.0#compositefileprovider
                        string serverImagesDirectory = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                        uniqueFileName = _fileOperations.AssembleQuestionUploadedFileName(questionViewModel.UserId, questionViewModel.Image.FileName);
                        string filePath = Path.Combine(serverImagesDirectory, uniqueFileName);
                        await questionViewModel.Image.CopyToAsync(new FileStream(filePath, FileMode.Create));
                    }
                    var question = _mapper.Map<QuestionViewModel, Question>(questionViewModel);
                    question.ImageNamePath = uniqueFileName;
                    var resultQuestion = await _repository.AddQuestionAsync(question);
                    return RedirectToAction("Details", new { questionId = resultQuestion.Id });
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
            return View();
        }

        // GET: QuestionsController/5/Edit
        [HttpGet]
        [Route("questions/{questionId}/edit")]
        public async Task<IActionResult> EditQuestion(int questionId)
        {
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
            var questionViewModel = _mapper.Map<Question, QuestionViewModel>(question);
            //ViewData["ValidationErrorMessage"] = validationErrorMessage;
            return View(questionViewModel);
        }

        // POST: QuestionsController/5/Edit
        [HttpPost]
        [Route("questions/{questionId}/edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditQuestion(QuestionViewModel questionViewModel)
        {
            var question = await _repository.GetQuestionByIdWithoutDetailsAsync(questionViewModel.Id);
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
            if (ModelState.IsValid)
            {
                try
                {
                    var currentlySignedInUser = await _userManager.GetUserAsync(User);
                    questionViewModel.UserId = currentlySignedInUser.Id;
                    string uniqueFileName = null;
                    if (questionViewModel.Image != null)
                    {
                        // for more advanced projects add a composite file provider - for now wwwroot
                        // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/file-providers?view=aspnetcore-5.0#compositefileprovider
                        string serverImagesDirectory = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                        uniqueFileName = _fileOperations.AssembleQuestionUploadedFileName(questionViewModel.UserId, questionViewModel.Image.FileName);
                        string filePath = Path.Combine(serverImagesDirectory, uniqueFileName);
                        await questionViewModel.Image.CopyToAsync(new FileStream(filePath, FileMode.Create));
                    }
                    question = _mapper.Map<QuestionViewModel, Question>(questionViewModel);
                    question.ImageNamePath = uniqueFileName;
                    await _repository.EditQuestionAsync(question);
                    return RedirectToAction("Details", new { questionId = questionViewModel.Id });
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
            // simply returning View somehow sends a Post request and parameter type error shows up as it expects a QVM
            // did some research, stackoverflow old 6 and 3 yr questions - no answers
            // also set up a custom error message via ViewData but this is more "expected behavior" as it sneds a Get, probably
            // answers controller has no issues with this
            return await EditQuestion(questionViewModel.Id);
        }

        // Get: QuestionsController/5/Remove
        [HttpGet]
        [Route("questions/remove/{questionId}")]
        public async Task<IActionResult> RemoveQuestion(int questionId)
        {
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
                await _repository.RemoveQuestionById(questionId);
                return RedirectToAction("Index", "List");
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

        // Get: QuestionsController/5/RemoveImage
        [HttpGet]
        [Route("questions/removeimage/{questionId}")]
        public async Task<IActionResult> RemoveImage(int questionId)
        {
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
                await _repository.RemoveQuestionImageByQuestionId(questionId);
                return RedirectToAction("Details", new { questionId = questionId });
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

        // Get: QuestionsController/5/VoteUp
        [HttpGet]
        [Route("questions/{questionId}/voteup")]
        public async Task<IActionResult> VoteUpQuestion(int questionId, string redirection= "redirectToAllQuestions")
        {
            var question = await _repository.GetQuestionByIdWithoutDetailsAsync(questionId);
            if (question == null)
            {
                Response.StatusCode = 404;
                ViewData["ErrorMessage"] = "404 Resource not found.";
                return View("Error");
            }
            try
            {
                await _repository.VoteUpQuestionById(questionId);
                if (String.Equals("redirectToDetails", redirection))
                {
                    return RedirectToAction("Details", new { questionId = questionId });
                }
                else if (String.Equals("redirectToHome", redirection))
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return RedirectToAction("AllQuestions", "Home");
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
            
        }

        // Get: QuestionsController/5/VoteDown
        [HttpGet]
        [Route("questions/{questionId}/votedown")]
        public async Task<IActionResult> VoteDownQuestion(int questionId, string redirection="redirectToAllQuestions")
        {
            var question = await _repository.GetQuestionByIdWithoutDetailsAsync(questionId);
            if (question == null)
            {
                Response.StatusCode = 404;
                ViewData["ErrorMessage"] = "404 Resource not found.";
                return View("Error");
            }
            try
            {
                await _repository.VoteDownQuestionById(questionId);
                if (String.Equals("redirectToDetails", redirection))
                {
                    return RedirectToAction("Details", new { questionId = questionId });
                }
                else if (String.Equals("redirectToHome", redirection))
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return RedirectToAction("AllQuestions", "Home");
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
        }
    }
}
