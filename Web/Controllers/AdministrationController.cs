using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Web.ViewModels;

namespace Web.Controllers
{

    [Authorize(Policy = "AdminAccess")]
    public class AdministrationController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAsyncRepository _repository;
        private readonly IMapper _mapper;

        public AdministrationController(RoleManager<IdentityRole> roleManager,
                                        UserManager<ApplicationUser> userManager,
                                        IAsyncRepository repository,
                                        IMapper mapper)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel createRoleViewModel)
        {
            if (ModelState.IsValid)
            {
                IdentityRole identityRole = new IdentityRole
                {
                    Name = createRoleViewModel.RoleName
                };

                IdentityResult result = await _roleManager.CreateAsync(identityRole);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles", "Administration");
                }
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(createRoleViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> ListRoles()
        {
            var roles = _roleManager.Roles;
            List<IdentityRole> inMemoryRoles = new List<IdentityRole>();
            foreach (var role in roles)
            {
                inMemoryRoles.Add(role);
            }
            Dictionary<string, List<string>> roleUsers = new Dictionary<string, List<string>>();
            foreach (var role in inMemoryRoles)
            {
                var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name);
                var emailsInRole = usersInRole.Select(u => u.Email).ToList();
                roleUsers.Add(role.Name, emailsInRole);
            }
            ListRolesDisplayObject rolesAndMembers = new ListRolesDisplayObject
            {
                Roles = roles,
                UserLists = roleUsers
            };
            return View(rolesAndMembers);
        }

        [HttpGet]
        public async Task<IActionResult> EditUsersInRole(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            ViewData["roleId"] = roleId;
            ViewData["roleName"] = role.Name;

            if (role == null)
            {
                // implement error page with message, for now redirect to ListRoles
                return RedirectToAction("ListRoles", "Administration");
            }

            List<ApplicationUser> allUsersFromDb = await _repository.GetAllUsers();
            var allUsersViewModel = _mapper.Map<List<ApplicationUser>, List<ApplicationUserViewModel>>(allUsersFromDb);

            return View(allUsersViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddUserToRole(UserRoleViewModel userRoleViewModel)
        {
            var user = await _userManager.FindByIdAsync(userRoleViewModel.UserId);
            var role = await _roleManager.FindByIdAsync(userRoleViewModel.RoleId);

            IdentityResult result = null;
            if ((await _userManager.IsInRoleAsync(user, role.Name)) == false)
            {
                result = await _userManager.AddToRoleAsync(user, role.Name);
            }
            if (result.Succeeded)
            {
                return RedirectToAction("ListRoles", "Administration");
            }
            return View("EditUsersInRole", new { roleId = userRoleViewModel.RoleId });

            
        }

        [HttpPost]
        public async Task<IActionResult> RemoveUserFromRole(UserRoleViewModel userRoleViewModel)
        {
            var user = await _userManager.FindByIdAsync(userRoleViewModel.UserId);
            var role = await _roleManager.FindByIdAsync(userRoleViewModel.RoleId);

            IdentityResult result = null;
            if ((await _userManager.IsInRoleAsync(user, role.Name)) == true)
            {
                result = await _userManager.RemoveFromRoleAsync(user, role.Name);
            }
            if (result.Succeeded)
            {
                return RedirectToAction("ListRoles", "Administration");
            }
            return View("EditUsersInRole", new { roleId = userRoleViewModel.RoleId });
        }

        [HttpGet]
        public async Task<IActionResult> ManageUserClaims(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewData["ErrorMessage"] = "The user cannot be found.";
                return View("Error");
            }
            var existingUserClaims = await _userManager.GetClaimsAsync(user);
            var allClaims = await _repository.GetAllUserClaims();
            var allClaimsViewModel = _mapper.Map<List<ApplicationClaim>, List<ApplicationClaimViewModel>>(allClaims);

            ManageUserClaimsViewModel allInfo = new ManageUserClaimsViewModel()
            {
                UserId = userId,
                UserEmail = user.Email,
                ExistingUserClaims = existingUserClaims,
                AllClaims = allClaimsViewModel
            };
            return View(allInfo);
        }

        [HttpPost]
        public async Task<IActionResult> AddClaimToUser(ClaimModificationViewModel claimModificationViewModel)
        {
            var claimFromDb = await _repository.GetApplicationClaimById(claimModificationViewModel.ClaimId);
            string claimType = claimFromDb.ClaimType;
            string claimValue = claimFromDb.ClaimValue;
            var user = await _userManager.FindByIdAsync(claimModificationViewModel.UserId);
            
            Claim newClaim = new Claim(claimType, claimValue);
            var result = await _userManager.AddClaimAsync(user, newClaim);
            if (result.Succeeded)
            {
                return RedirectToAction("ManageUserClaims", new { userId = user.Id });
            }
            // error message UX 
            return RedirectToAction("ManageUserClaims", new { userId = user.Id });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveClaimFromUser(ClaimModificationViewModel claimModificationViewModel)
        {
            var claimFromDb = await _repository.GetApplicationClaimById(claimModificationViewModel.ClaimId);
            string claimType = claimFromDb.ClaimType;
            string claimValue = claimFromDb.ClaimValue;
            var user = await _userManager.FindByIdAsync(claimModificationViewModel.UserId);

            Claim newClaim = new Claim(claimType, claimValue);
            var result = await _userManager.RemoveClaimAsync(user, newClaim);
            if (result.Succeeded)
            {
                return RedirectToAction("ManageUserClaims", new { userId = user.Id });
            }
            // error message UX 
            return RedirectToAction("ManageUserClaims", new { userId = user.Id });
        }
    }
}
