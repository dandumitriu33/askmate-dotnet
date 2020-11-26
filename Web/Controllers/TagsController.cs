using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.ViewModels;

namespace Web.Controllers
{
    public class TagsController : Controller
    {
        private readonly IAsyncRepository _repository;
        private readonly IMapper _mapper;

        public TagsController(IAsyncRepository repository,
                              IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        // GET: TagsController/tags
        [HttpGet]
        public async Task<IActionResult> AttachTag(int questionId)
        {
            List<Tag> tagsFromDb = await _repository.GetAllTagsNoDuplicates(questionId);
            List<TagViewModel> tagsViewModel = _mapper.Map<List<Tag>, List<TagViewModel>>(tagsFromDb);
            ViewData["questionId"] = questionId.ToString();
            return View(tagsViewModel);
        }

        // POST: TagsController/tags
        [HttpPost]
        public async Task<IActionResult> AttachTag(int questionId, string tagName)
        {
            // create new tag assuming it doesn't exist, will check in refactor/validation round
            Tag newTag = new Tag
            {
                Name = tagName
            };
            await _repository.AddTagAsync(newTag);

            // attach the new tag to q
            await AddQuestionTag(questionId, newTag.Id);
            return RedirectToAction("Details", "Questions", new { questionId = questionId });
        }

        // GET: TagsController/detach
        [HttpGet]
        [Route("detach")]
        public async Task<IActionResult> DetachTag(int tagId, int questionId)
        {
            QuestionTag newQuestionTag = new QuestionTag
            {
                QuestionId = questionId,
                TagId = tagId
            };
            await _repository.DetachTag(newQuestionTag);
            return RedirectToAction("Details", "Questions", new { questionId = questionId });
        }

        // GET: TagsController/addQuestionTag
        public async Task<IActionResult> AddQuestionTag(int questionId, int tagId)
        {
            QuestionTag newQuestionTag = new QuestionTag
            {
                QuestionId = questionId,
                TagId = tagId
            };
            await _repository.AddQuestionTagAsync(newQuestionTag);
            return RedirectToAction("Details", "Questions", new { questionId = questionId });
        }

        // GET: TagsController/info
        [Route("info")]
        public async Task<IActionResult> TagInfo()
        {
            Dictionary<int, int> tagInfo = await _repository.GetTagInfo();
            List<Tag> allTags = await _repository.GetAllTags();
            Dictionary<string, int> result = new Dictionary<string, int>();
            foreach (var item in tagInfo)
            {
                var tempTag = allTags.Where(t => t.Id == item.Key).FirstOrDefault();
                result.Add(tempTag.Name, item.Value);
            }
            return View(result);
        }
        
    }
}
