using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class Comment : BaseEntity
    {
        public string Body { get; set; }
        public DateTime DateAdded { get; set; }
        public bool IsRemoved { get; set; } = false;
    }
}
