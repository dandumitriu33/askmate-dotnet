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
        private readonly DateTime dateAdded;
        public QuestionViewModel()
        {
            this.dateAdded = DateTime.Now;
        }
        public int Id { get; set; }
        [MaxLength(100)]
        public string Title { get; set; }
        [MaxLength(1000)]
        public string Body { get; set; }
        public DateTime DateAdded 
        {
            get { return dateAdded; }
            set { } 
        }
        public List<CommentViewModel> Comments { get; set; }
        public List<AnswerViewModel> Answers { get; set; }
    }
}
