using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ApplicationCore.Entities
{
    public class ApplicationRole : IdentityRole
    {
        [Required]
        [MaxLength(100)]
        public string RoleName { get; set; }
    }
}
