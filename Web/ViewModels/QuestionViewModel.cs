using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.ViewModels
{
    public class QuestionViewModel
    {
        public int Id { get; set; }
        [Column(TypeName = ("varchar(100)"))]
        public string Title { get; set; }
        [Column(TypeName = ("varchar(1000)"))]
        public string Body { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime DateAdded { get; set; }
        public List<CommentViewModel> Comments { get; set; }
        public List<AnswerViewModel> Answers { get; set; }
    }
}
