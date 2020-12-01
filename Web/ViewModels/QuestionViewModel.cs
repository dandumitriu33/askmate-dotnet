using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.ViewModels
{
    public class QuestionViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="The question title is required.")]
        [MaxLength(100)]
        public string Title { get; set; }
        [MaxLength(1000)]
        public string Body { get; set; }
        public DateTime DateAdded { get; set; }
        public int Views { get; set; }
        public int Votes { get; set; }
        public List<QuestionCommentViewModel> QuestionComments { get; set; }
        public List<AnswerViewModel> Answers { get; set; }
        public List<TagViewModel> Tags { get; set; }
        public IFormFile Image { get; set; }
        [MaxLength(400)]
        public string ImageNamePath { get; set; }
        [MaxLength(64)]
        public string UserId { get; set; }
    }
}
