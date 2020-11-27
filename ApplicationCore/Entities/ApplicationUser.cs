using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public DateTime DateAdded { get; set; }
        public int Reputation { get; set; }
    }
}
