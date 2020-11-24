using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.ViewModels;

namespace Web.Controllers
{
    public class AnswersController : Controller
    {
        private readonly IAsyncRepository _repository;
        private readonly IMapper _mapper;

        public AnswersController(IAsyncRepository repository,
                                 IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
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
                var answer = _mapper.Map<AnswerViewModel, Answer>(answerViewModel);
                await _repository.AddAnswerAsync(answer);
            }
            return RedirectToAction("Details", "Questions", new { questionId = answerViewModel.QuestionId });
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
    }
}
