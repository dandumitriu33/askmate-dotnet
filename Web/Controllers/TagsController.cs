﻿using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        public async Task<IActionResult> AddTag(int questionId)
        {
            try
            {
                var question = await _repository.GetQuestionByIdWithoutDetailsAsync(questionId);
                if (question == null)
                {
                    Response.StatusCode = 404;
                    ViewData["ErrorMessage"] = "404 Resource not found.";
                    return View("Error");
                }
                List<Tag> tagsFromDb = await _repository.GetAllTagsNoDuplicates(questionId);
                List<TagViewModel> tagsViewModel = _mapper.Map<List<Tag>, List<TagViewModel>>(tagsFromDb);
                ViewData["questionId"] = questionId.ToString();
                return View("AddTag", tagsViewModel);
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

        // POST: TagsController/tags
        [HttpPost]
        public async Task<IActionResult> AddTag(TagViewModel tagViewModel, int questionId)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var question = await _repository.GetQuestionByIdWithoutDetailsAsync(questionId);
                    if (question == null)
                    {
                        Response.StatusCode = 404;
                        ViewData["ErrorMessage"] = "404 Resource not found.";
                        return View("Error");
                    }
                    var tag = await _repository.GetTagByIdAsync(tagViewModel.Id);
                    if (tag == null)
                    {
                        Tag newTag = _mapper.Map<TagViewModel, Tag>(tagViewModel);
                        tag = await _repository.AddTagAsync(newTag);
                    }
                    await AttachQuestionTag(tag.Id, questionId);
                    return RedirectToAction("Details", "Questions", new { questionId = questionId });
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
            ViewData["questionId"] = questionId.ToString();
            return View("AddTag", tagViewModel);
        }

        // GET: TagsController/detach
        [HttpGet]
        [Route("detach")]
        public async Task<IActionResult> DetachQuestionTag(int tagId, int questionId)
        {
            try
            {
                var question = await _repository.GetQuestionByIdWithoutDetailsAsync(questionId);
                if (question == null)
                {
                    Response.StatusCode = 404;
                    ViewData["ErrorMessage"] = "404 Resource not found.";
                    return View("Error");
                }
                var tag = await _repository.GetTagByIdAsync(tagId);
                if (tag == null)
                {
                    Response.StatusCode = 404;
                    ViewData["ErrorMessage"] = "404 Resource not found.";
                    return View("Error");
                }
                QuestionTag newQuestionTag = new QuestionTag
                {
                    QuestionId = questionId,
                    TagId = tagId
                };
                await _repository.DetachTag(newQuestionTag);
                return RedirectToAction("Details", "Questions", new { questionId = questionId });
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

        // GET: TagsController/AttachQuestionTag
        public async Task<IActionResult> AttachQuestionTag(int tagId, int questionId)
        {
            try
            {
                var question = await _repository.GetQuestionByIdWithoutDetailsAsync(questionId);
                if (question == null)
                {
                    Response.StatusCode = 404;
                    ViewData["ErrorMessage"] = "404 Resource not found.";
                    return View("Error");
                }
                var tag = await _repository.GetTagByIdAsync(tagId);
                if (tag == null)
                {
                    Response.StatusCode = 404;
                    ViewData["ErrorMessage"] = "404 Resource not found.";
                    return View("Error");
                }
                QuestionTag newQuestionTag = new QuestionTag
                {
                    QuestionId = questionId,
                    TagId = tagId
                };
                await _repository.AttachQuestionTagAsync(newQuestionTag);
                return RedirectToAction("Details", "Questions", new { questionId = questionId });
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

        // GET: TagsController/info
        [Route("info")]
        public async Task<IActionResult> TagInfo()
        {
            try
            {
                Dictionary<int, int> tagInfo = await _repository.GetTagInfo();
                List<Tag> allTags = await _repository.GetAllTags();
                Dictionary<string, int> result = new Dictionary<string, int>();
                foreach (var item in tagInfo)
                {
                    var tempTag = allTags.Where(t => t.Id == item.Key).FirstOrDefault();
                    result.Add(tempTag.Name, item.Value);
                }
                return View("TagInfo", result);
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
