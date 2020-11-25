﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ApplicationCore.Entities
{
    public class Comment : BaseEntity
    {
        [Column(TypeName = ("varchar(1000)"))]
        public string Body { get; set; }
        public int QuestionId { get; set; }
        public int AnswerId { get; set; }
        public DateTime DateAdded { get; set; }
        public bool IsRemoved { get; set; } = false;
    }
}
