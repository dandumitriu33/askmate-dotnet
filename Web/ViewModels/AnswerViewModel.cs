using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.ViewModels
{
    public class AnswerViewModel
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(1000, ErrorMessage ="Answer length must be less than 1000 characters.")]
        public string Body { get; set; }
        public DateTime DateAdded { get; set; }
        public int QuestionId { get; set; }
        public int Votes { get; set; }
        public List<AnswerCommentViewModel> AnswerComments { get; set; }
        public IFormFile Image { get; set; }
        [MaxLength(400)]
        public string ImageNamePath { get; set; }
        [MaxLength(64)]
        public string UserId { get; set; }
        public bool IsAccepted { get; set; } = false;
    }
}
