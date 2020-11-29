using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Web.Models;
using Web.ViewModels;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IAsyncRepository _repository;
        private readonly IMapper _mapper;

        public HomeController(ILogger<HomeController> logger,
                              IAsyncRepository repository,
                              IMapper mapper)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            int numberOfQuestions = 5;
            var latestQuestions = await _repository.GetLatestQuestions(numberOfQuestions);
            var latestQuestionsViewModel = _mapper.Map<List<Question>, List<QuestionViewModel>>(latestQuestions);
            return View(latestQuestionsViewModel);
        }

        [Authorize]
        // Get: HomeController/5/VoteUp
        [HttpGet]
        [Route("home/{questionId}/voteup")]
        public async Task<IActionResult> VoteUpQuestion(int questionId)
        {
            await _repository.VoteUpQuestionById(questionId);

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        // Get: HomeController/5/VoteDown
        [HttpGet]
        [Route("home/{questionId}/votedown")]
        public async Task<IActionResult> VoteDownQuestion(int questionId)
        {
            await _repository.VoteDownQuestionById(questionId);

            return RedirectToAction("Index", "Home");
        }

        // Get: HomeController/{searchPhrase}
        [HttpGet]
        [Route("home/{searchPhrase}")]
        public async Task<IActionResult> Search([FromQuery] string searchPhrase)
        {
            var searchResults = await _repository.GetSearchResults(searchPhrase);
            var searchResultsViewModel = _mapper.Map<List<Question>, List<QuestionViewModel>>(searchResults);
            ViewData["searchPhrase"] = searchPhrase;
            return View(searchResultsViewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
