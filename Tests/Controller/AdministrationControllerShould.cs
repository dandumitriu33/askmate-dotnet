using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Web.Controllers;
using Web.ViewModels;
using Xunit;

namespace Tests.Controller
{
    public class AdministrationControllerShould
    {
        private UserManager<ApplicationUser> userManager { get; }
        private SignInManager<ApplicationUser> signInManager { get; }
        private RoleManager<IdentityRole> roleManager { get; }
        private IAsyncRepository repository { get; }
        private IMapper mapper { get; }

        [Fact]
        public void CreateRoleGet_ReturnAViewResult()
        {
            // Arrange
            var controller = new AdministrationController(roleManager, userManager, repository, mapper);

            // Act
            var result = controller.CreateRole();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task CreateRolePost_ReturnsViewResult_WhenModelStateIsInvalid()
        {
            // Arrange
            var controller = new AdministrationController(roleManager, userManager, repository, mapper);
            controller.ModelState.AddModelError("RoleName", "Required");
            var newRoleViewModel = new RoleViewModel();

            // Act
            var result = await controller.CreateRole(newRoleViewModel);

            // Assert
            var badRequestResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<RoleViewModel>(badRequestResult.ViewData.Model);
        }
    }
}
