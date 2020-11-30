using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.ViewModels
{
    public class QuestionCommentViewModel
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(1000, ErrorMessage = "Comment length must be less than 1000 characters.")]
        public string Body { get; set; }
        public int QuestionId { get; set; }
        public DateTime DateAdded { get; set; }
        public bool IsEdited { get; set; }
        public string UserId { get; set; }
    }
}
