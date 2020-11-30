using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ApplicationCore.Entities
{
    public class Answer
    {
        public int Id { get; set; }
        [Column(TypeName = ("varchar(1000)"))]
        public string Body { get; set; }
        public DateTime DateAdded { get; set; }
        public int QuestionId { get; set; }
        public bool IsRemoved { get; set; } = false;
        public int Votes { get; set; }
        public List<AnswerComment> AnswerComments { get; set; }
        [MaxLength(400)]
        public string ImageNamePath { get; set; }
        [MaxLength(64)]
        public string UserId { get; set; }
        public bool IsAccepted { get; set; } = false;
    }
}
