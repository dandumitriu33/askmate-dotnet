using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class Answer : BaseEntity
    {
        public string Body { get; set; }
        public bool IsRemoved { get; set; }
        public List<Comment> Comments { get; set; }
    }
}
