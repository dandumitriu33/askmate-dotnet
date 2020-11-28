using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.ViewModels;

namespace Web.Controllers
{
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
        public IActionResult ListRoles()
        {
            var roles = _roleManager.Roles;
            return View(roles);
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
    }
}
