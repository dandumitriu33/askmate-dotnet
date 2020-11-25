﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.ViewModels
{
    public class QuestionCommentViewModel
    {
        public int Id { get; set; }
        [Column(TypeName = ("varchar(1000)"))]
        public string Body { get; set; }
        public int QuestionId { get; set; }
        public DateTime DateAdded { get; set; }
        public bool IsEdited { get; set; }
    }
}