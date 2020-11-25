using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<QuestionTag> QuestionTags { get; set; }
        public bool IsRemoved { get; set; } = false;
    }
}
