using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ApplicationCore.Entities
{
    public class AnswerComment
    {
        public int Id { get; set; }
        [Column(TypeName = ("varchar(1000)"))]
        public string Body { get; set; }
        public DateTime DateAdded { get; set; }
        public bool IsRemoved { get; set; } = false;
        public bool IsEdited { get; set; } = false;
        public int QuestionId { get; set; }
        public int AnswerId { get; set; }
        public string UserId { get; set; }
    }
}
