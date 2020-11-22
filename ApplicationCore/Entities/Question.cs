using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ApplicationCore.Entities
{
    public class Question : BaseEntity
    {
        [Required]
        [Column(TypeName = ("varchar(100)"))]
        public string Title { get; set; }
        public string Body { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime DateAdded { get; set; }
        public bool IsRemoved { get; set; } = false;
        public List<Comment> Comments { get; set; }
        public List<Answer> Answers { get; set; }
    }
}
