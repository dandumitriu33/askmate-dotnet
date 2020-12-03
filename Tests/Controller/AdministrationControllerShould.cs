using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Shared;
using Web.Controllers;
using Web.ViewModels;
using Xunit;

namespace Tests.Controller
{
    public class AdministrationControllerShould
    {
        private UserManager<ApplicationUser> userManager { get; }
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
        public async Task CreateRolePost_RedirectToListRoles()
        {
            // Arrange
            RegisterViewModel newRegisterVM = new RegisterViewModel();

            // mocking RoleManager
            var mockRoleManager = MockHelpers.MockRoleManager<IdentityRole>();
            mockRoleManager.Setup(rm => rm.CreateAsync(It.IsAny<IdentityRole>())).ReturnsAsync(IdentityResult.Success).Verifiable();

            var controller = new AdministrationController(mockRoleManager.Object, userManager, repository, mapper);
            RoleViewModel tempRole = new RoleViewModel() { RoleName = "Test role name" };

            // Act
            var result = await controller.CreateRole(tempRole);

            // Assert
            var requestResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ListRoles", requestResult.ActionName);
            Assert.Equal("Administration", requestResult.ControllerName);
            mockRoleManager.Verify(x => x.CreateAsync(It.IsAny<IdentityRole>()), Times.Once);
        }

        [Fact]
        public async Task CreateRolePost_ReturnViewOnCreateRoleFail()
        {
            // Arrange
            RegisterViewModel newRegisterVM = new RegisterViewModel();

            // mocking RoleManager
            var mockRoleManager = MockHelpers.MockRoleManager<IdentityRole>();
            mockRoleManager.Setup(rm => rm.CreateAsync(It.IsAny<IdentityRole>())).ReturnsAsync(IdentityResult.Failed());

            var controller = new AdministrationController(mockRoleManager.Object, userManager, repository, mapper);
            RoleViewModel tempRole = new RoleViewModel() { RoleName = "Test role name" };

            // Act
            var result = await controller.CreateRole(tempRole);

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("CreateRole", requestResult.ViewName);
            mockRoleManager.Verify(x => x.CreateAsync(It.IsAny<IdentityRole>()), Times.Once);
        }

        [Fact]
        public async Task CreateRolePost_ReturnViewOnCreateRoleException()
        {
            // Arrange
            RegisterViewModel newRegisterVM = new RegisterViewModel();

            // mocking RoleManager
            var mockRoleManager = MockHelpers.MockRoleManager<IdentityRole>();
            mockRoleManager.Setup(rm => rm.CreateAsync(It.IsAny<IdentityRole>())).Throws(new Exception());

            var controller = new AdministrationController(mockRoleManager.Object, userManager, repository, mapper);
            RoleViewModel tempRole = new RoleViewModel() { RoleName = "Test role name" };

            // Act
            var result = await controller.CreateRole(tempRole);

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", requestResult.ViewName);
            mockRoleManager.Verify(x => x.CreateAsync(It.IsAny<IdentityRole>()), Times.Once);
        }

        [Fact]
        public async Task CreateRolePost_ReturnCreateRoleViewOnModelInvalid()
        {
            // Arrange
            RegisterViewModel newRegisterVM = new RegisterViewModel();

            // mocking RoleManager
            var mockRoleManager = MockHelpers.MockRoleManager<IdentityRole>();
            mockRoleManager.Setup(rm => rm.CreateAsync(It.IsAny<IdentityRole>())).ReturnsAsync(IdentityResult.Success).Verifiable();

            var controller = new AdministrationController(mockRoleManager.Object, userManager, repository, mapper);
            RoleViewModel tempRole = new RoleViewModel() { RoleName = "Test role name" };
            controller.ModelState.AddModelError("RoleName", "Required");

            // Act
            var result = await controller.CreateRole(tempRole);


            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("CreateRole", requestResult.ViewName);
            mockRoleManager.Verify(x => x.CreateAsync(It.IsAny<IdentityRole>()), Times.Never);
        }

        [Fact]
        public async Task ListRolesGet_RedirectToListRoles()
        {
            // Arrange
            RegisterViewModel newRegisterVM = new RegisterViewModel();

            // mocking UserManager
            var mockUserManager = MockHelpers.MockUserManager<ApplicationUser>();
            List<ApplicationUser> usersInRole = new List<ApplicationUser>();
            usersInRole.Add(new ApplicationUser { Email = "test@email.com" });
            mockUserManager.Setup(um => um.GetUsersInRoleAsync(It.IsAny<string>())).ReturnsAsync(usersInRole);

            // mocking RoleManager
            var mockRoleManager = MockHelpers.MockRoleManager<IdentityRole>();
            List<IdentityRole> returnedRoles = new List<IdentityRole>();
            IdentityRole newRole = new IdentityRole { Name = "Test Role" };
            returnedRoles.Add(newRole);
            mockRoleManager.Setup(rm => rm.Roles).Returns(returnedRoles.AsQueryable());

            var controller = new AdministrationController(mockRoleManager.Object, mockUserManager.Object, repository, mapper);

            // Act
            var result = await controller.ListRoles();

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("ListRoles", requestResult.ViewName);
            mockUserManager.Verify(x => x.GetUsersInRoleAsync(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task ListRolesGet_ReturnsErrorViewOnException()
        {
            // Arrange
            RegisterViewModel newRegisterVM = new RegisterViewModel();

            // mocking UserManager
            var mockUserManager = MockHelpers.MockUserManager<ApplicationUser>();
            List<ApplicationUser> usersInRole = new List<ApplicationUser>();
            usersInRole.Add(new ApplicationUser { Email = "test@email.com" });
            mockUserManager.Setup(um => um.GetUsersInRoleAsync(It.IsAny<string>())).ReturnsAsync(usersInRole);

            // mocking RoleManager
            var mockRoleManager = MockHelpers.MockRoleManager<IdentityRole>();
            mockRoleManager.Setup(rm => rm.Roles).Throws(new Exception());

            var controller = new AdministrationController(mockRoleManager.Object, mockUserManager.Object, repository, mapper);

            // Act
            var result = await controller.ListRoles();

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", requestResult.ViewName);
            mockUserManager.Verify(x => x.GetUsersInRoleAsync(It.IsAny<string>()), Times.Never);
        }



        //[Fact]
        //public async Task CreateRolePost_ReturnsViewResult_WhenModelStateIsInvalid()
        //{
        //    // Arrange
        //    var controller = new AdministrationController(roleManager, userManager, repository, mapper);
        //    controller.ModelState.AddModelError("RoleName", "Required");
        //    var newRoleViewModel = new RoleViewModel();

        //    // Act
        //    var result = await controller.CreateRole(newRoleViewModel);

        //    // Assert
        //    var badRequestResult = Assert.IsType<ViewResult>(result);
        //    var model = Assert.IsAssignableFrom<RoleViewModel>(badRequestResult.ViewData.Model);
        //    Assert.NotEqual("Error", badRequestResult.ViewName);
        //}

        //[Fact]
        //public async Task CreateRolePost_ReturnsViewResult_WhenModelStateIsValidCreateFails()
        //{
        //    // Arrange
        //    var controller = new AdministrationController(roleManager, userManager, repository, mapper);
        //    var newRoleViewModel = new RoleViewModel();

        //    // Act
        //    var result = await controller.CreateRole(newRoleViewModel);

        //    // Assert
        //    var badRequestResult = Assert.IsType<ViewResult>(result);
        //    Assert.Equal("Error", badRequestResult.ViewName);
        //}

        //[Fact]
        //public async Task ListRolesGet_ReturnAResult()
        //{
        //    // Arrange
        //    var controller = new AdministrationController(roleManager, userManager, repository, mapper);

        //    // Act
        //    var result = await controller.ListRoles();

        //    // Assert
        //    var badRequestResult = Assert.IsType<ViewResult>(result);
        //    Assert.Equal("Error", badRequestResult.ViewName);
        //}

    }
}
