using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class Comment : BaseEntity
    {
        private DateTime dateAdded;

        public string Body { get; set; }
        public DateTime DateAdded
        {
            get { return this.dateAdded; }
            set { this.dateAdded = DateTime.Now; }
        }
        public bool IsRemoved { get; set; } = false;
    }
}
