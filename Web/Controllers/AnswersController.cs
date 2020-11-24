using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
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
        private readonly IFileTypeChecker _fileTypeChecker;

        public AnswersController(IAsyncRepository repository,
                                 IMapper mapper,
                                 IWebHostEnvironment webHostEnvironment,
                                 IFileTypeChecker fileTypeChecker)
        {
            _repository = repository;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
            _fileTypeChecker = fileTypeChecker;
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
            if (ModelState.IsValid && _fileTypeChecker.ValidateImageType(answerViewModel.Image.FileName) == true)
            {
                string uniqueFileName = null;
                if (answerViewModel.Image != null)
                {
                    // for more advanced projects add a composite file provider - for now wwwroot
                    // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/file-providers?view=aspnetcore-5.0#compositefileprovider
                    string serverImagesDirectory = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                    uniqueFileName = "AQID_"
                                    + answerViewModel.QuestionId + "_"
                                    + DateTime.Now.Year.ToString() + "_"
                                    + DateTime.Now.Month.ToString() + "_"
                                    + DateTime.Now.Day.ToString() + "_"
                                    + DateTime.Now.Hour.ToString() + "_"
                                    + DateTime.Now.Minute.ToString() + "_"
                                    + DateTime.Now.Second.ToString() + "_"
                                    + Guid.NewGuid().ToString() + "_" + answerViewModel.Image.FileName;
                    string filePath = Path.Combine(serverImagesDirectory, uniqueFileName);
                    await answerViewModel.Image.CopyToAsync(new FileStream(filePath, FileMode.Create));
                }
                var answer = _mapper.Map<AnswerViewModel, Answer>(answerViewModel);
                answer.ImageNamePath = uniqueFileName;
                await _repository.AddAnswerAsync(answer);
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
    }
}
