using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.ViewModels;

namespace Web.AutomapperProfiles
{
    public class AskMateProfiles : Profile
    {
        public AskMateProfiles()
        {
            CreateMap<Question, QuestionViewModel>()
                .ReverseMap();
            CreateMap<Answer, AnswerViewModel>()
                .ReverseMap();
            CreateMap<QuestionComment, QuestionCommentViewModel>()
                .ReverseMap();
            CreateMap<AnswerComment, AnswerCommentViewModel>()
                .ReverseMap();
            CreateMap<Tag, TagViewModel>()
                .ReverseMap();
        }
    }
}
