using ApplicationCore.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.ViewModels;

namespace Web.Components
{
    public class CreateAndAddTag : ViewComponent
    {
        private readonly IAsyncRepository _repository;
        private readonly IMapper _mapper;

        public CreateAndAddTag(IAsyncRepository repository,
                               IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public IViewComponentResult Invoke()
        {
            TagViewModel newTagViewModel = new TagViewModel();
            return View(newTagViewModel);
        }
    }
}
