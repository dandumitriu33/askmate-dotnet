﻿using System;
using System.Collections.Generic;
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
        public bool IsRemoved { get; set; }
        public int Votes { get; set; }
        public List<AnswerComment> AnswerComments { get; set; }
        public string ImageNamePath { get; set; }
        public string UserId { get; set; }
    }
}
