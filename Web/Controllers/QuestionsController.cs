using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Services;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Web.ViewModels;

namespace Web.Controllers
{
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
            if (ModelState.IsValid && _fileOperations.ValidateImageType(questionViewModel.Image.FileName) == true)
            {
                string uniqueFileName = null;
                if (questionViewModel.Image != null)
                {
                    // for more advanced projects add a composite file provider - for now wwwroot
                    // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/file-providers?view=aspnetcore-5.0#compositefileprovider
                    string serverImagesDirectory = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                    uniqueFileName = _fileOperations.AssembleQuestionUploadedFileName(questionViewModel.Title, questionViewModel.Image.FileName);
                    string filePath = Path.Combine(serverImagesDirectory, uniqueFileName);
                    await questionViewModel.Image.CopyToAsync(new FileStream(filePath, FileMode.Create));
                }
                var question = _mapper.Map<QuestionViewModel, Question>(questionViewModel);
                question.ImageNamePath = uniqueFileName;
                var currentlySignedInUser = await _userManager.GetUserAsync(User);
                question.UserId = currentlySignedInUser.Id;
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

        // GET: QuestionsController/5/Edit
        [HttpGet]
        [Route("questions/{questionId}/edit")]
        public async Task<IActionResult> Edit(int questionId)
        {
            var question = await _repository.GetQuestionByIdWithoutDetailsAsync(questionId);
            var questionViewModel = _mapper.Map<Question, QuestionViewModel>(question);
            return View(questionViewModel);
        }

        // POST: QuestionsController/5/Edit
        [HttpPost]
        [Route("questions/{questionId}/edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(QuestionViewModel questionViewModel)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = null;
                if (questionViewModel.Image != null)
                {
                    // for more advanced projects add a composite file provider - for now wwwroot
                    // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/file-providers?view=aspnetcore-5.0#compositefileprovider
                    string serverImagesDirectory = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                    uniqueFileName = _fileOperations.AssembleQuestionUploadedFileName(questionViewModel.Title, questionViewModel.Image.FileName);
                    string filePath = Path.Combine(serverImagesDirectory, uniqueFileName);
                    await questionViewModel.Image.CopyToAsync(new FileStream(filePath, FileMode.Create));
                }
                var question = _mapper.Map<QuestionViewModel, Question>(questionViewModel);
                question.ImageNamePath = uniqueFileName;
                await _repository.EditQuestionAsync(question);
                return RedirectToAction("Details", new { questionId = questionViewModel.Id });
            }
            return View(questionViewModel.Id);
        }

        // Get: QuestionsController/5/Remove
        [HttpGet]
        [Route("questions/remove/{questionId}")]
        public async Task<IActionResult> Remove(int questionId)
        {
            await _repository.RemoveQuestionById(questionId);
            
            return RedirectToAction("Index", "List");
        }

        // Get: QuestionsController/5/VoteUp
        [HttpGet]
        [Route("questions/{questionId}/voteup")]
        public async Task<IActionResult> VoteUpQuestion(int questionId)
        {
            await _repository.VoteUpQuestionById(questionId);

            return RedirectToAction("Index", "List");
        }

        // Get: QuestionsController/5/VoteDown
        [HttpGet]
        [Route("questions/{questionId}/votedown")]
        public async Task<IActionResult> VoteDownQuestion(int questionId)
        {
            await _repository.VoteDownQuestionById(questionId);

            return RedirectToAction("Index", "List");
        }
    }
}
