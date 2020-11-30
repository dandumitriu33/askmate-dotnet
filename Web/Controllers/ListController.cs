using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using AutoMapper;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.ViewModels;

namespace Web.Controllers
{
    public class ListController : Controller
    {
        private readonly IAsyncRepository _repository;
        private readonly IMapper _mapper;

        public ListController(IAsyncRepository repository,
                              IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        // GET: ListController
        public async Task<IActionResult> Index(string orderBy="DateAdded", string direction="Descending")
        {
            var questions = await _repository.ListAllAsync(orderBy, direction);
            var questionsViewModel = _mapper.Map<List<Question>, List<QuestionViewModel>>(questions);
            return View(questionsViewModel);
        }
    }
}
