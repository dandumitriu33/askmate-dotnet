using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            try
            {
                var latestQuestions = await _repository.GetLatestQuestions(numberOfQuestions);
                var latestQuestionsViewModel = _mapper.Map<List<Question>, List<QuestionViewModel>>(latestQuestions);
                return View(latestQuestionsViewModel);
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

        // Get: HomeController/{searchPhrase}
        [HttpGet]
        [Route("home/{searchPhrase}")]
        public async Task<IActionResult> Search([FromQuery] string searchPhrase)
        {
            try
            {
                var searchResults = await _repository.GetSearchResults(searchPhrase);
                var searchResultsViewModel = _mapper.Map<List<Question>, List<QuestionViewModel>>(searchResults);
                ViewData["searchPhrase"] = searchPhrase;
                return View(searchResultsViewModel);
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
