using Microsoft.AspNetCore.Identity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.ViewModels
{
    public class ListRolesDisplayObject
    {
        public IEnumerable<IdentityRole> Roles { get; set; }
        public Dictionary<string, List<string>> UserLists { get; set; }
    }
}
