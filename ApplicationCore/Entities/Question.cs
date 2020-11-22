using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class Question : BaseEntity
    {
        private DateTime dateAdded;

        public string Title { get; set; }
        public string Body { get; set; }
        
        public DateTime DateAdded {
            get { return this.dateAdded; } 
            set { this.dateAdded = DateTime.Now; } 
        }
        public bool IsRemoved { get; set; } = false;
        public List<Comment> Comments { get; set; }
        public List<Answer> Answers { get; set; }
    }
}
