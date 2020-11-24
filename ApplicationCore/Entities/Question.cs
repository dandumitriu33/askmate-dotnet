using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ApplicationCore.Entities
{
    public class Question
    {
        public int Id { get; set; }
        
        [Required]
        [Column(TypeName = ("varchar(100)"))]
        public string Title { get; set; }
        [Column(TypeName = ("varchar(1000)"))]
        public string Body { get; set; }
        public DateTime DateAdded { get; set; }
        public int Views { get; set; }
        public int Votes { get; set; }
        public bool IsRemoved { get; set; } = false;
        public List<Comment> Comments { get; set; }
        public List<Answer> Answers { get; set; }
        public string ImageNamePath { get; set; }
    }
}
