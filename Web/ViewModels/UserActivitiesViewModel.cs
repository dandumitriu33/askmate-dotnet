using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.ViewModels
{
    public class UserActivitiesViewModel
    {
        public List<QuestionViewModel> Questions { get; set; }
        public List<AnswerViewModel> Answers { get; set; }
        public List<QuestionCommentViewModel> QuestionComments { get; set; }
        public List<AnswerCommentViewModel> AnswerComments { get; set; }
    }
}
