using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Web.ViewModels
{
    public class TagViewModel
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(100, ErrorMessage ="The tag name must be under 100 characters in length.")]
        public string Name { get; set; }
    }
}
