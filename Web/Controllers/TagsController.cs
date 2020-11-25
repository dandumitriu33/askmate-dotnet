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
        public async Task<IActionResult> AttachTag(int questionId)
        {
            List<Tag> tagsFromDb = await _repository.GetAllTags();
            List<TagViewModel> tagsViewModel = _mapper.Map<List<Tag>, List<TagViewModel>>(tagsFromDb);
            ViewData["questionId"] = questionId.ToString();
            return View(tagsViewModel);
        }

        // GET: TagsController/addQuestionTag
        public async Task AddQuestionTag(int questionId, int tagId)
        {
            QuestionTag newQuestionTag = new QuestionTag
            {
                QuestionId = questionId,
                TagId = tagId
            };
            await _repository.AddQuestionTagAsync(newQuestionTag);
        }

        // GET: TagsController
        public ActionResult Index()
        {
            return View();
        }

        // GET: TagsController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: TagsController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TagsController/Create
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

        // GET: TagsController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: TagsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
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

        // GET: TagsController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: TagsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
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
    }
}
