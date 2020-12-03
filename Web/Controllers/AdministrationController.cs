using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public async Task<IActionResult> CreateRole(RoleViewModel roleViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    IdentityRole identityRole = new IdentityRole
                    {
                        Name = roleViewModel.RoleName
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
                catch (DbUpdateException dbex)
                {
                    ViewData["ErrorMessage"] = "DB issue - " + dbex.Message;
                    return View("Error");
                }
                catch (Exception ex)
                {
                    ViewData["ErrorMessage"] = ex.Message;
                    return View("Error");
                }
            }
            return View("CreateRole", roleViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> ListRoles()
        {
            try
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
                return View("ListRoles", rolesAndMembers);
            }
            catch (DbUpdateException dbex)
            {
                ViewData["ErrorMessage"] = "DB issue - " + dbex.Message;
                return View("Error");
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Message;
                return View("Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditUsersInRole(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                Response.StatusCode = 404;
                ViewData["ErrorMessage"] = "404 Resource not found.";
                return View("Error");
            }
            try
            {
                ViewData["roleId"] = roleId;
                ViewData["roleName"] = role.Name;

                List<ApplicationUser> allUsersFromDb = await _repository.GetAllUsers();
                var allUsersViewModel = _mapper.Map<List<ApplicationUser>, List<ApplicationUserViewModel>>(allUsersFromDb);

                return View(allUsersViewModel);
            }
            catch (DbUpdateException dbex)
            {
                ViewData["ErrorMessage"] = "DB issue - " + dbex.Message;
                return View("Error");
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Message;
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddUserToRole(UserRoleViewModel userRoleViewModel)
        {
            var user = await _userManager.FindByIdAsync(userRoleViewModel.UserId);
            if (user == null)
            {
                Response.StatusCode = 404;
                ViewData["ErrorMessage"] = "404 Resource not found.";
                return View("Error");
            }
            var role = await _roleManager.FindByIdAsync(userRoleViewModel.RoleId);
            if (role == null)
            {
                Response.StatusCode = 404;
                ViewData["ErrorMessage"] = "404 Resource not found.";
                return View("Error");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    if ((await _userManager.IsInRoleAsync(user, role.Name)) == false)
                    {
                        IdentityResult result = await _userManager.AddToRoleAsync(user, role.Name);
                        if (result.Succeeded)
                        {
                            return RedirectToAction("ListRoles", "Administration");
                        }
                    }
                    return View("EditUsersInRole", new { roleId = userRoleViewModel.RoleId });
                }
                catch (DbUpdateException dbex)
                {
                    ViewData["ErrorMessage"] = "DB issue - " + dbex.Message;
                    return View("Error");
                }
                catch (Exception ex)
                {
                    ViewData["ErrorMessage"] = ex.Message;
                    return View("Error");
                }
            }
            return RedirectToAction("ListRoles", "Administration");
        }

        [HttpGet]
        [Route("administration/{roleId}/remove/{userEmail}")]
        public async Task<IActionResult> RemoveUserFromRole(string userEmail, string roleId)
        {
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null)
            {
                Response.StatusCode = 404;
                ViewData["ErrorMessage"] = "404 Resource not found.";
                return View("Error");
            }
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                Response.StatusCode = 404;
                ViewData["ErrorMessage"] = "404 Resource not found.";
                return View("Error");
            }
            try
            {
                if ((await _userManager.IsInRoleAsync(user, role.Name)) == true)
                {
                    IdentityResult result = await _userManager.RemoveFromRoleAsync(user, role.Name);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("ListRoles", "Administration");
                    }
                }
                return View("EditUsersInRole", new { roleId = roleId });
            }
            catch (DbUpdateException dbex)
            {
                ViewData["ErrorMessage"] = "DB issue - " + dbex.Message;
                return View("Error");
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Message;
                return View("Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> ManageUserClaims(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                Response.StatusCode = 404;
                ViewData["ErrorMessage"] = "404 Resource not found.";
                return View("Error");
            }
            try
            {
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
            catch (DbUpdateException dbex)
            {
                ViewData["ErrorMessage"] = "DB issue - " + dbex.Message;
                return View("Error");
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Message;
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddClaimToUser(ClaimModificationViewModel claimModificationViewModel)
        {
            var claimFromDb = await _repository.GetApplicationClaimById(claimModificationViewModel.ClaimId);
            if (claimFromDb == null)
            {
                Response.StatusCode = 404;
                ViewData["ErrorMessage"] = "404 Resource not found.";
                return View("Error");
            }
            var user = await _userManager.FindByIdAsync(claimModificationViewModel.UserId);
            if (user == null)
            {
                Response.StatusCode = 404;
                ViewData["ErrorMessage"] = "404 Resource not found.";
                return View("Error");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    string claimType = claimFromDb.ClaimType;
                    string claimValue = claimFromDb.ClaimValue;

                    Claim newClaim = new Claim(claimType, claimValue);
                    var result = await _userManager.AddClaimAsync(user, newClaim);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("ManageUserClaims", new { userId = user.Id });
                    }
                    else
                    {
                        throw new Exception("Claim attachment error.");
                    }
                }
                catch (DbUpdateException dbex)
                {
                    ViewData["ErrorMessage"] = "DB issue - " + dbex.Message;
                    return View("Error");
                }
                catch (Exception ex)
                {
                    ViewData["ErrorMessage"] = ex.Message;
                    return View("Error");
                }
            }
            return RedirectToAction("ManageUserClaims", new { userId = user.Id });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveClaimFromUser(ClaimModificationViewModel claimModificationViewModel)
        {
            var claimFromDb = await _repository.GetApplicationClaimById(claimModificationViewModel.ClaimId);
            if (claimFromDb == null)
            {
                Response.StatusCode = 404;
                ViewData["ErrorMessage"] = "404 Resource not found.";
                return View("Error");
            }
            var user = await _userManager.FindByIdAsync(claimModificationViewModel.UserId);
            if (user == null)
            {
                Response.StatusCode = 404;
                ViewData["ErrorMessage"] = "404 Resource not found.";
                return View("Error");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    string claimType = claimFromDb.ClaimType;
                    string claimValue = claimFromDb.ClaimValue;

                    Claim newClaim = new Claim(claimType, claimValue);
                    var result = await _userManager.RemoveClaimAsync(user, newClaim);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("ManageUserClaims", new { userId = user.Id });
                    }
                    else
                    {
                        throw new Exception("Claim attachment error.");
                    }
                }
                catch (DbUpdateException dbex)
                {
                    ViewData["ErrorMessage"] = "DB issue - " + dbex.Message;
                    return View("Error");
                }
                catch (Exception ex)
                {
                    ViewData["ErrorMessage"] = ex.Message;
                    return View("Error");
                }
            }
            return RedirectToAction("ManageUserClaims", new { userId = user.Id });
        }
    }
}
