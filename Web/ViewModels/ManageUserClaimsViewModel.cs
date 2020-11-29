using ApplicationCore.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Web.ViewModels
{
    public class ManageUserClaimsViewModel
    {
        public string UserId { get; set; }
        public string UserEmail { get; set; }
        public IList<Claim> ExistingUserClaims { get; set; }
        public IList<ApplicationClaimViewModel> AllClaims { get; set; }
    }
}
