﻿using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Tests.Shared;
using Web.AutomapperProfiles;
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

        [Fact]
        public async Task EditUsersInRoleGet_RedirectToListRoles()
        {
            // Arrange

            // mocking RoleManager
            var mockRoleManager = MockHelpers.MockRoleManager<IdentityRole>();
            IdentityRole newRole = new IdentityRole { Name = "Test Role" };
            mockRoleManager.Setup(rm => rm.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(newRole).Verifiable();

            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            mockRepo.Setup(repo => repo.GetAllUsers())
                .ReturnsAsync(GetAllUsersForMock());

            // adding a real mapper
            var myProfile = new AskMateProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var realMapper = new Mapper(configuration);

            var controller = new AdministrationController(mockRoleManager.Object, userManager, mockRepo.Object, realMapper);

            // Act
            var result = await controller.EditUsersInRole("abcdefg");

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("EditUsersInRole", requestResult.ViewName);
            mockRoleManager.Verify(x => x.FindByIdAsync(It.IsAny<string>()), Times.Once);
            mockRepo.Verify(repo => repo.GetAllUsers(), Times.Once);
        }

        [Fact]
        public async Task EditUsersInRoleGet_ErrorViewOnRoleNull()
        {
            // Arrange

            // mocking RoleManager
            var mockRoleManager = MockHelpers.MockRoleManager<IdentityRole>();
            //IdentityRole newRole = new IdentityRole { Name = "Test Role" };
            IdentityRole newRole = null;
            mockRoleManager.Setup(rm => rm.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(newRole).Verifiable();

            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            mockRepo.Setup(repo => repo.GetAllUsers())
                .ReturnsAsync(GetAllUsersForMock());

            // adding a real mapper
            var myProfile = new AskMateProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var realMapper = new Mapper(configuration);

            // mocking Response.StatusCode = 404 setter
            var mockHttpContext = new Mock<HttpContext>();
            var response = new Mock<HttpResponse>();
            mockHttpContext.SetupGet(x => x.Response).Returns(response.Object);

            //creates an instance of an asp.net mvc controller
            var controller = new AdministrationController(mockRoleManager.Object, userManager, mockRepo.Object, realMapper)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };

            // Act
            var result = await controller.EditUsersInRole("abcdefg");

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", requestResult.ViewName);
            mockRoleManager.Verify(x => x.FindByIdAsync(It.IsAny<string>()), Times.Once);
            mockRepo.Verify(repo => repo.GetAllUsers(), Times.Never);
        }

        [Fact]
        public async Task EditUsersInRoleGet_ErrorViewOnException()
        {
            // Arrange

            // mocking RoleManager
            var mockRoleManager = MockHelpers.MockRoleManager<IdentityRole>();
            //IdentityRole newRole = new IdentityRole { Name = "Test Role" };
            IdentityRole newRole = null;
            mockRoleManager.Setup(rm => rm.FindByIdAsync(It.IsAny<string>())).Throws(new Exception()).Verifiable();

            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            mockRepo.Setup(repo => repo.GetAllUsers())
                .ReturnsAsync(GetAllUsersForMock());

            // adding a real mapper
            var myProfile = new AskMateProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var realMapper = new Mapper(configuration);

            // mocking Response.StatusCode = 404 setter
            var mockHttpContext = new Mock<HttpContext>();
            var response = new Mock<HttpResponse>();
            mockHttpContext.SetupGet(x => x.Response).Returns(response.Object);

            //creates an instance of an asp.net mvc controller
            var controller = new AdministrationController(mockRoleManager.Object, userManager, mockRepo.Object, realMapper)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            }; ;

            // Act
            var result = await controller.EditUsersInRole("abcdefg");

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", requestResult.ViewName);
            mockRoleManager.Verify(x => x.FindByIdAsync(It.IsAny<string>()), Times.Once);
            mockRepo.Verify(repo => repo.GetAllUsers(), Times.Never);
        }

        [Fact]
        public async Task AddUserToRolePost_RedirectToActionListRolesSuccess()
        {
            // Arrange

            // mocking UserManager            
            var mockUserManager = MockHelpers.MockUserManager<ApplicationUser>();
            ApplicationUser tempUser = new ApplicationUser { Email = "test@email.com" };
            mockUserManager.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(tempUser).Verifiable();
            mockUserManager.Setup(um => um.IsInRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(false).Verifiable();
            mockUserManager.Setup(um => um.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success).Verifiable();

            // mocking RoleManager
            var mockRoleManager = MockHelpers.MockRoleManager<IdentityRole>();
            IdentityRole tempRole = new IdentityRole() { Name = "Test Role"};
            mockRoleManager.Setup(rm => rm.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(tempRole).Verifiable();

            // mocking Response.StatusCode = 404 setter
            var mockHttpContext = new Mock<HttpContext>();
            var response = new Mock<HttpResponse>();
            mockHttpContext.SetupGet(x => x.Response).Returns(response.Object);

            //creates an instance of an asp.net mvc controller
            var controller = new AdministrationController(mockRoleManager.Object, mockUserManager.Object, repository, mapper)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };
            UserRoleViewModel tempUserRoleViewModel = new UserRoleViewModel
            {
                RoleId = "abcd",
                UserId = "efgh"
            };

            // Act
            var result = await controller.AddUserToRole(tempUserRoleViewModel);

            // Assert
            var requestResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ListRoles", requestResult.ActionName);
            Assert.Equal("Administration", requestResult.ControllerName);
            mockRoleManager.Verify(rm => rm.FindByIdAsync(It.IsAny<string>()), Times.Once);
            mockUserManager.Verify(um => um.FindByIdAsync(It.IsAny<string>()), Times.Once);
            mockUserManager.Verify(um => um.IsInRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Once);
            mockUserManager.Verify(um => um.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Once);            
        }

        [Fact]
        public async Task AddUserToRolePost_RedirectToActionOnModelInvalid()
        {
            // Arrange

            // mocking UserManager            
            var mockUserManager = MockHelpers.MockUserManager<ApplicationUser>();
            ApplicationUser tempUser = new ApplicationUser { Email = "test@email.com" };
            mockUserManager.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(tempUser).Verifiable();
            mockUserManager.Setup(um => um.IsInRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(false).Verifiable();
            mockUserManager.Setup(um => um.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success).Verifiable();

            // mocking RoleManager
            var mockRoleManager = MockHelpers.MockRoleManager<IdentityRole>();
            IdentityRole tempRole = new IdentityRole() { Name = "Test Role" };
            mockRoleManager.Setup(rm => rm.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(tempRole).Verifiable();

            // mocking Response.StatusCode = 404 setter
            var mockHttpContext = new Mock<HttpContext>();
            var response = new Mock<HttpResponse>();
            mockHttpContext.SetupGet(x => x.Response).Returns(response.Object);

            //creates an instance of an asp.net mvc controller
            var controller = new AdministrationController(mockRoleManager.Object, mockUserManager.Object, repository, mapper)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };
            UserRoleViewModel tempUserRoleViewModel = new UserRoleViewModel
            {
                RoleId = "abcd",
                UserId = "efgh"
            };
            controller.ModelState.AddModelError("RoleId", "Required");

            // Act
            var result = await controller.AddUserToRole(tempUserRoleViewModel);

            // Assert
            var requestResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ListRoles", requestResult.ActionName);
            Assert.Equal("Administration", requestResult.ControllerName);
            mockRoleManager.Verify(rm => rm.FindByIdAsync(It.IsAny<string>()), Times.Never);
            mockUserManager.Verify(um => um.FindByIdAsync(It.IsAny<string>()), Times.Never);
            mockUserManager.Verify(um => um.IsInRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);
            mockUserManager.Verify(um => um.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task AddUserToRolePost_EditUsersInRoleViewOnAlreadyInRole()
        {
            // Arrange

            // mocking UserManager            
            var mockUserManager = MockHelpers.MockUserManager<ApplicationUser>();
            ApplicationUser tempUser = new ApplicationUser { Email = "test@email.com" };
            mockUserManager.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(tempUser).Verifiable();
            mockUserManager.Setup(um => um.IsInRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(true).Verifiable();
            mockUserManager.Setup(um => um.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success).Verifiable();

            // mocking RoleManager
            var mockRoleManager = MockHelpers.MockRoleManager<IdentityRole>();
            IdentityRole tempRole = new IdentityRole() { Name = "Test Role" };
            mockRoleManager.Setup(rm => rm.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(tempRole).Verifiable();

            // mocking Response.StatusCode = 404 setter
            var mockHttpContext = new Mock<HttpContext>();
            var response = new Mock<HttpResponse>();
            mockHttpContext.SetupGet(x => x.Response).Returns(response.Object);

            //creates an instance of an asp.net mvc controller
            var controller = new AdministrationController(mockRoleManager.Object, mockUserManager.Object, repository, mapper)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };
            UserRoleViewModel tempUserRoleViewModel = new UserRoleViewModel
            {
                RoleId = "abcd",
                UserId = "efgh"
            };

            // Act
            var result = await controller.AddUserToRole(tempUserRoleViewModel);

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("EditUsersInRole", requestResult.ViewName);
            mockRoleManager.Verify(rm => rm.FindByIdAsync(It.IsAny<string>()), Times.Once);
            mockUserManager.Verify(um => um.FindByIdAsync(It.IsAny<string>()), Times.Once);
            mockUserManager.Verify(um => um.IsInRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Once);
            mockUserManager.Verify(um => um.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task AddUserToRolePost_ErrorViewOnUserNotFound()
        {
            // Arrange

            // mocking UserManager            
            var mockUserManager = MockHelpers.MockUserManager<ApplicationUser>();
            ApplicationUser tempUser = null;
            mockUserManager.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(tempUser).Verifiable();
            mockUserManager.Setup(um => um.IsInRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(false).Verifiable();
            mockUserManager.Setup(um => um.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success).Verifiable();

            // mocking RoleManager
            var mockRoleManager = MockHelpers.MockRoleManager<IdentityRole>();
            IdentityRole tempRole = new IdentityRole() { Name = "Test Role" };
            mockRoleManager.Setup(rm => rm.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(tempRole).Verifiable();

            // mocking Response.StatusCode = 404 setter
            var mockHttpContext = new Mock<HttpContext>();
            var response = new Mock<HttpResponse>();
            mockHttpContext.SetupGet(x => x.Response).Returns(response.Object);

            //creates an instance of an asp.net mvc controller
            var controller = new AdministrationController(mockRoleManager.Object, mockUserManager.Object, repository, mapper)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };
            UserRoleViewModel tempUserRoleViewModel = new UserRoleViewModel
            {
                RoleId = "abcd",
                UserId = "efgh"
            };

            // Act
            var result = await controller.AddUserToRole(tempUserRoleViewModel);

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", requestResult.ViewName);
            mockRoleManager.Verify(rm => rm.FindByIdAsync(It.IsAny<string>()), Times.Never);
            mockUserManager.Verify(um => um.FindByIdAsync(It.IsAny<string>()), Times.Once);
            mockUserManager.Verify(um => um.IsInRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);
            mockUserManager.Verify(um => um.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task AddUserToRolePost_ErrorViewOnRoleNotFound()
        {
            // Arrange

            // mocking UserManager            
            var mockUserManager = MockHelpers.MockUserManager<ApplicationUser>();
            ApplicationUser tempUser = new ApplicationUser { Email = "test@email.com" };
            mockUserManager.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(tempUser).Verifiable();
            mockUserManager.Setup(um => um.IsInRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(false).Verifiable();
            mockUserManager.Setup(um => um.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success).Verifiable();

            // mocking RoleManager
            var mockRoleManager = MockHelpers.MockRoleManager<IdentityRole>();
            IdentityRole tempRole = null;
            mockRoleManager.Setup(rm => rm.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(tempRole).Verifiable();

            // mocking Response.StatusCode = 404 setter
            var mockHttpContext = new Mock<HttpContext>();
            var response = new Mock<HttpResponse>();
            mockHttpContext.SetupGet(x => x.Response).Returns(response.Object);

            //creates an instance of an asp.net mvc controller
            var controller = new AdministrationController(mockRoleManager.Object, mockUserManager.Object, repository, mapper)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };
            UserRoleViewModel tempUserRoleViewModel = new UserRoleViewModel
            {
                RoleId = "abcd",
                UserId = "efgh"
            };

            // Act
            var result = await controller.AddUserToRole(tempUserRoleViewModel);

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", requestResult.ViewName);
            mockRoleManager.Verify(rm => rm.FindByIdAsync(It.IsAny<string>()), Times.Once);
            mockUserManager.Verify(um => um.FindByIdAsync(It.IsAny<string>()), Times.Once);
            mockUserManager.Verify(um => um.IsInRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);
            mockUserManager.Verify(um => um.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task AddUserToRolePost_ErrorViewOnException()
        {
            // Arrange

            // mocking UserManager            
            var mockUserManager = MockHelpers.MockUserManager<ApplicationUser>();
            ApplicationUser tempUser = new ApplicationUser { Email = "test@email.com" };
            mockUserManager.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(tempUser).Verifiable();
            mockUserManager.Setup(um => um.IsInRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(false).Verifiable();
            mockUserManager.Setup(um => um.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).Throws(new Exception()).Verifiable();

            // mocking RoleManager
            var mockRoleManager = MockHelpers.MockRoleManager<IdentityRole>();
            IdentityRole tempRole = new IdentityRole() { Name = "Test Role" };
            mockRoleManager.Setup(rm => rm.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(tempRole).Verifiable();

            // mocking Response.StatusCode = 404 setter
            var mockHttpContext = new Mock<HttpContext>();
            var response = new Mock<HttpResponse>();
            mockHttpContext.SetupGet(x => x.Response).Returns(response.Object);

            //creates an instance of an asp.net mvc controller
            var controller = new AdministrationController(mockRoleManager.Object, mockUserManager.Object, repository, mapper)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };
            UserRoleViewModel tempUserRoleViewModel = new UserRoleViewModel
            {
                RoleId = "abcd",
                UserId = "efgh"
            };

            // Act
            var result = await controller.AddUserToRole(tempUserRoleViewModel);

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", requestResult.ViewName);
            mockRoleManager.Verify(rm => rm.FindByIdAsync(It.IsAny<string>()), Times.Once);
            mockUserManager.Verify(um => um.FindByIdAsync(It.IsAny<string>()), Times.Once);
            mockUserManager.Verify(um => um.IsInRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Once);
            mockUserManager.Verify(um => um.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task RemoveUserFromRoleGet_RedirectToActionListRolesSuccess()
        {
            // Arrange

            // mocking UserManager            
            var mockUserManager = MockHelpers.MockUserManager<ApplicationUser>();
            ApplicationUser tempUser = new ApplicationUser { Email = "test@email.com" };
            mockUserManager.Setup(um => um.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(tempUser).Verifiable();
            mockUserManager.Setup(um => um.IsInRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(true).Verifiable();
            mockUserManager.Setup(um => um.RemoveFromRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success).Verifiable();

            // mocking RoleManager
            var mockRoleManager = MockHelpers.MockRoleManager<IdentityRole>();
            IdentityRole tempRole = new IdentityRole() { Name = "Test Role" };
            mockRoleManager.Setup(rm => rm.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(tempRole).Verifiable();

            // mocking Response.StatusCode = 404 setter
            var mockHttpContext = new Mock<HttpContext>();
            var response = new Mock<HttpResponse>();
            mockHttpContext.SetupGet(x => x.Response).Returns(response.Object);

            //creates an instance of an asp.net mvc controller
            var controller = new AdministrationController(mockRoleManager.Object, mockUserManager.Object, repository, mapper)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };

            // Act
            var result = await controller.RemoveUserFromRole("test@email.com", "abcd");

            // Assert
            var requestResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ListRoles", requestResult.ActionName);
            Assert.Equal("Administration", requestResult.ControllerName);
            mockRoleManager.Verify(rm => rm.FindByIdAsync(It.IsAny<string>()), Times.Once);
            mockUserManager.Verify(um => um.FindByEmailAsync(It.IsAny<string>()), Times.Once);
            mockUserManager.Verify(um => um.IsInRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Once);
            mockUserManager.Verify(um => um.RemoveFromRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task RemoveUserFromRoleGet_EditUsersInRoleUserNotInRole()
        {
            // Arrange

            // mocking UserManager            
            var mockUserManager = MockHelpers.MockUserManager<ApplicationUser>();
            ApplicationUser tempUser = new ApplicationUser { Email = "test@email.com" };
            mockUserManager.Setup(um => um.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(tempUser).Verifiable();
            mockUserManager.Setup(um => um.IsInRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(false).Verifiable();
            mockUserManager.Setup(um => um.RemoveFromRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success).Verifiable();

            // mocking RoleManager
            var mockRoleManager = MockHelpers.MockRoleManager<IdentityRole>();
            IdentityRole tempRole = new IdentityRole() { Name = "Test Role" };
            mockRoleManager.Setup(rm => rm.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(tempRole).Verifiable();

            // mocking Response.StatusCode = 404 setter
            var mockHttpContext = new Mock<HttpContext>();
            var response = new Mock<HttpResponse>();
            mockHttpContext.SetupGet(x => x.Response).Returns(response.Object);

            //creates an instance of an asp.net mvc controller
            var controller = new AdministrationController(mockRoleManager.Object, mockUserManager.Object, repository, mapper)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };

            // Act
            var result = await controller.RemoveUserFromRole("test@email.com", "abcd");

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("EditUsersInRole", requestResult.ViewName);
            mockRoleManager.Verify(rm => rm.FindByIdAsync(It.IsAny<string>()), Times.Once);
            mockUserManager.Verify(um => um.FindByEmailAsync(It.IsAny<string>()), Times.Once);
            mockUserManager.Verify(um => um.IsInRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Once);
            mockUserManager.Verify(um => um.RemoveFromRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task RemoveUserFromRoleGet_ErrorViewOnException()
        {
            // Arrange

            // mocking UserManager            
            var mockUserManager = MockHelpers.MockUserManager<ApplicationUser>();
            ApplicationUser tempUser = new ApplicationUser { Email = "test@email.com" };
            mockUserManager.Setup(um => um.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(tempUser).Verifiable();
            mockUserManager.Setup(um => um.IsInRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(true).Verifiable();
            mockUserManager.Setup(um => um.RemoveFromRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success).Verifiable();

            // mocking RoleManager
            var mockRoleManager = MockHelpers.MockRoleManager<IdentityRole>();
            IdentityRole tempRole = new IdentityRole() { Name = "Test Role" };
            mockRoleManager.Setup(rm => rm.FindByIdAsync(It.IsAny<string>())).Throws(new Exception()).Verifiable();

            // mocking Response.StatusCode = 404 setter
            var mockHttpContext = new Mock<HttpContext>();
            var response = new Mock<HttpResponse>();
            mockHttpContext.SetupGet(x => x.Response).Returns(response.Object);

            //creates an instance of an asp.net mvc controller
            var controller = new AdministrationController(mockRoleManager.Object, mockUserManager.Object, repository, mapper)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };

            // Act
            var result = await controller.RemoveUserFromRole("test@email.com", "abcd");

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", requestResult.ViewName);
            mockRoleManager.Verify(rm => rm.FindByIdAsync(It.IsAny<string>()), Times.Once);
            mockUserManager.Verify(um => um.FindByEmailAsync(It.IsAny<string>()), Times.Once);
            mockUserManager.Verify(um => um.IsInRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);
            mockUserManager.Verify(um => um.RemoveFromRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task ManageUserClaimsGet_ManageUserClaimsViewOnSuccess()
        {
            // Arrange

            // mocking UserManager            
            var mockUserManager = MockHelpers.MockUserManager<ApplicationUser>();
            ApplicationUser tempUser = new ApplicationUser { Email = "test@email.com" };
            mockUserManager.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(tempUser).Verifiable();
            mockUserManager.Setup(um => um.GetClaimsAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(GetClaimsListForMock()).Verifiable();

            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            mockRepo.Setup(repo => repo.GetAllUserClaims()).ReturnsAsync(GetApplicationClaimsListForMock()).Verifiable();

            // mocking Response.StatusCode = 404 setter
            var mockHttpContext = new Mock<HttpContext>();
            var response = new Mock<HttpResponse>();
            mockHttpContext.SetupGet(x => x.Response).Returns(response.Object);

            // adding a real mapper
            var myProfile = new AskMateProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var realMapper = new Mapper(configuration);

            //creates an instance of an asp.net mvc controller
            var controller = new AdministrationController(roleManager, mockUserManager.Object, mockRepo.Object, realMapper)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };

            // Act
            var result = await controller.ManageUserClaims("abcd");

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("ManageUserClaims", requestResult.ViewName);
            mockUserManager.Verify(um => um.FindByIdAsync(It.IsAny<string>()), Times.Once);
            mockUserManager.Verify(um => um.GetClaimsAsync(It.IsAny<ApplicationUser>()), Times.Once);
            mockRepo.Verify(um => um.GetAllUserClaims(), Times.Once);
        }

        [Fact]
        public async Task ManageUserClaimsGet_ErrorViewOnNullUser()
        {
            // Arrange

            // mocking UserManager            
            var mockUserManager = MockHelpers.MockUserManager<ApplicationUser>();
            ApplicationUser tempUser = null;
            mockUserManager.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(tempUser).Verifiable();
            mockUserManager.Setup(um => um.GetClaimsAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(GetClaimsListForMock()).Verifiable();

            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            mockRepo.Setup(repo => repo.GetAllUserClaims()).ReturnsAsync(GetApplicationClaimsListForMock()).Verifiable();

            // mocking Response.StatusCode = 404 setter
            var mockHttpContext = new Mock<HttpContext>();
            var response = new Mock<HttpResponse>();
            mockHttpContext.SetupGet(x => x.Response).Returns(response.Object);

            // adding a real mapper
            var myProfile = new AskMateProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var realMapper = new Mapper(configuration);

            //creates an instance of an asp.net mvc controller
            var controller = new AdministrationController(roleManager, mockUserManager.Object, mockRepo.Object, realMapper)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };

            // Act
            var result = await controller.ManageUserClaims("abcd");

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", requestResult.ViewName);
            mockUserManager.Verify(um => um.FindByIdAsync(It.IsAny<string>()), Times.Once);
            mockUserManager.Verify(um => um.GetClaimsAsync(It.IsAny<ApplicationUser>()), Times.Never);
            mockRepo.Verify(um => um.GetAllUserClaims(), Times.Never);
        }

        [Fact]
        public async Task ManageUserClaimsGet_ErrorViewOnException()
        {
            // Arrange

            // mocking UserManager            
            var mockUserManager = MockHelpers.MockUserManager<ApplicationUser>();
            ApplicationUser tempUser = new ApplicationUser { Email = "test@email.com" };
            mockUserManager.Setup(um => um.FindByIdAsync(It.IsAny<string>())).Throws(new Exception()).Verifiable();
            mockUserManager.Setup(um => um.GetClaimsAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(GetClaimsListForMock()).Verifiable();

            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            mockRepo.Setup(repo => repo.GetAllUserClaims()).ReturnsAsync(GetApplicationClaimsListForMock()).Verifiable();

            // mocking Response.StatusCode = 404 setter
            var mockHttpContext = new Mock<HttpContext>();
            var response = new Mock<HttpResponse>();
            mockHttpContext.SetupGet(x => x.Response).Returns(response.Object);

            // adding a real mapper
            var myProfile = new AskMateProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var realMapper = new Mapper(configuration);

            //creates an instance of an asp.net mvc controller
            var controller = new AdministrationController(roleManager, mockUserManager.Object, mockRepo.Object, realMapper)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };

            // Act
            var result = await controller.ManageUserClaims("abcd");

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", requestResult.ViewName);
            mockUserManager.Verify(um => um.FindByIdAsync(It.IsAny<string>()), Times.Once);
            mockUserManager.Verify(um => um.GetClaimsAsync(It.IsAny<ApplicationUser>()), Times.Never);
            mockRepo.Verify(um => um.GetAllUserClaims(), Times.Never);
        }

        [Fact]
        public async Task AddClaimToUserPost_ManageUserClaimsRedirectToActionOnSuccess()
        {
            // Arrange

            // mocking UserManager            
            var mockUserManager = MockHelpers.MockUserManager<ApplicationUser>();
            ApplicationUser tempUser = new ApplicationUser { Email = "test@email.com" };
            mockUserManager.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(tempUser).Verifiable();
            mockUserManager.Setup(um => um.AddClaimAsync(It.IsAny<ApplicationUser>(), It.IsAny<Claim>())).ReturnsAsync(IdentityResult.Success).Verifiable();

            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            ApplicationClaim tempClaim = new ApplicationClaim() { ClaimType = ClaimTypes.Name, ClaimValue = "Test name" };
            mockRepo.Setup(repo => repo.GetApplicationClaimById(It.IsAny<int>())).ReturnsAsync(tempClaim).Verifiable();

            // mocking Response.StatusCode = 404 setter
            var mockHttpContext = new Mock<HttpContext>();
            var response = new Mock<HttpResponse>();
            mockHttpContext.SetupGet(x => x.Response).Returns(response.Object);

            //creates an instance of an asp.net mvc controller
            var controller = new AdministrationController(roleManager, mockUserManager.Object, mockRepo.Object, mapper)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };
            ClaimModificationViewModel tempClaimModificationVM = new ClaimModificationViewModel { ClaimId = 123, UserId = "abcd" };

            // Act
            var result = await controller.AddClaimToUser(tempClaimModificationVM);

            // Assert
            var requestResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ManageUserClaims", requestResult.ActionName);
            mockUserManager.Verify(um => um.FindByIdAsync(It.IsAny<string>()), Times.Once);
            mockUserManager.Verify(um => um.AddClaimAsync(It.IsAny<ApplicationUser>(), It.IsAny<Claim>()), Times.Once);
            mockRepo.Verify(um => um.GetApplicationClaimById(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task AddClaimToUserPost_RedirectToActionManageUserClaimsOnInvalidModel()
        {
            // Arrange

            // mocking UserManager            
            var mockUserManager = MockHelpers.MockUserManager<ApplicationUser>();
            ApplicationUser tempUser = new ApplicationUser { Email = "test@email.com" };
            mockUserManager.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(tempUser).Verifiable();
            mockUserManager.Setup(um => um.AddClaimAsync(It.IsAny<ApplicationUser>(), It.IsAny<Claim>())).ReturnsAsync(IdentityResult.Success).Verifiable();

            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            ApplicationClaim tempClaim = new ApplicationClaim() { ClaimType = ClaimTypes.Name, ClaimValue = "Test name" };
            mockRepo.Setup(repo => repo.GetApplicationClaimById(It.IsAny<int>())).ReturnsAsync(tempClaim).Verifiable();

            // mocking Response.StatusCode = 404 setter
            var mockHttpContext = new Mock<HttpContext>();
            var response = new Mock<HttpResponse>();
            mockHttpContext.SetupGet(x => x.Response).Returns(response.Object);

            //creates an instance of an asp.net mvc controller
            var controller = new AdministrationController(roleManager, mockUserManager.Object, mockRepo.Object, mapper)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };
            controller.ModelState.AddModelError("ClaimId", "Required");
            ClaimModificationViewModel tempClaimModificationVM = new ClaimModificationViewModel { ClaimId = 123, UserId = "abcd" };

            // Act
            var result = await controller.AddClaimToUser(tempClaimModificationVM);

            // Assert
            var requestResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ManageUserClaims", requestResult.ActionName);
            mockUserManager.Verify(um => um.FindByIdAsync(It.IsAny<string>()), Times.Never);
            mockUserManager.Verify(um => um.AddClaimAsync(It.IsAny<ApplicationUser>(), It.IsAny<Claim>()), Times.Never);
            mockRepo.Verify(um => um.GetApplicationClaimById(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task AddClaimToUserPost_ErrorViewOnException()
        {
            // Arrange

            // mocking UserManager            
            var mockUserManager = MockHelpers.MockUserManager<ApplicationUser>();
            ApplicationUser tempUser = new ApplicationUser { Email = "test@email.com" };
            mockUserManager.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(tempUser).Verifiable();
            mockUserManager.Setup(um => um.AddClaimAsync(It.IsAny<ApplicationUser>(), It.IsAny<Claim>())).ReturnsAsync(IdentityResult.Failed()).Verifiable();

            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            ApplicationClaim tempClaim = new ApplicationClaim() { ClaimType = ClaimTypes.Name, ClaimValue = "Test name" };
            mockRepo.Setup(repo => repo.GetApplicationClaimById(It.IsAny<int>())).ReturnsAsync(tempClaim).Verifiable();

            // mocking Response.StatusCode = 404 setter
            var mockHttpContext = new Mock<HttpContext>();
            var response = new Mock<HttpResponse>();
            mockHttpContext.SetupGet(x => x.Response).Returns(response.Object);

            //creates an instance of an asp.net mvc controller
            var controller = new AdministrationController(roleManager, mockUserManager.Object, mockRepo.Object, mapper)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };
            ClaimModificationViewModel tempClaimModificationVM = new ClaimModificationViewModel { ClaimId = 123, UserId = "abcd" };

            // Act
            var result = await controller.AddClaimToUser(tempClaimModificationVM);

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", requestResult.ViewName);
            mockUserManager.Verify(um => um.FindByIdAsync(It.IsAny<string>()), Times.Once);
            mockUserManager.Verify(um => um.AddClaimAsync(It.IsAny<ApplicationUser>(), It.IsAny<Claim>()), Times.Once);
            mockRepo.Verify(um => um.GetApplicationClaimById(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task RemoveClaimFromUserPost_RedirectToActionManageUserClaimsOnSuccess()
        {
            // Arrange

            // mocking UserManager            
            var mockUserManager = MockHelpers.MockUserManager<ApplicationUser>();
            ApplicationUser tempUser = new ApplicationUser { Email = "test@email.com" };
            mockUserManager.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(tempUser).Verifiable();
            mockUserManager.Setup(um => um.RemoveClaimAsync(It.IsAny<ApplicationUser>(), It.IsAny<Claim>())).ReturnsAsync(IdentityResult.Success).Verifiable();

            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            ApplicationClaim tempClaim = new ApplicationClaim() { ClaimType = ClaimTypes.Name, ClaimValue = "Test name" };
            mockRepo.Setup(repo => repo.GetApplicationClaimById(It.IsAny<int>())).ReturnsAsync(tempClaim).Verifiable();

            // mocking Response.StatusCode = 404 setter
            var mockHttpContext = new Mock<HttpContext>();
            var response = new Mock<HttpResponse>();
            mockHttpContext.SetupGet(x => x.Response).Returns(response.Object);

            //creates an instance of an asp.net mvc controller
            var controller = new AdministrationController(roleManager, mockUserManager.Object, mockRepo.Object, mapper)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };
            ClaimModificationViewModel tempClaimModificationVM = new ClaimModificationViewModel { ClaimId = 123, UserId = "abcd" };

            // Act
            var result = await controller.RemoveClaimFromUser(tempClaimModificationVM);

            // Assert
            var requestResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ManageUserClaims", requestResult.ActionName);
            mockUserManager.Verify(um => um.FindByIdAsync(It.IsAny<string>()), Times.Once);
            mockUserManager.Verify(um => um.RemoveClaimAsync(It.IsAny<ApplicationUser>(), It.IsAny<Claim>()), Times.Once);
            mockRepo.Verify(um => um.GetApplicationClaimById(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task RemoveClaimFromUserPost_RedirectToActionManageUserClaimsOnInvalidModel()
        {
            // Arrange

            // mocking UserManager            
            var mockUserManager = MockHelpers.MockUserManager<ApplicationUser>();
            ApplicationUser tempUser = new ApplicationUser { Email = "test@email.com" };
            mockUserManager.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(tempUser).Verifiable();
            mockUserManager.Setup(um => um.RemoveClaimAsync(It.IsAny<ApplicationUser>(), It.IsAny<Claim>())).ReturnsAsync(IdentityResult.Success).Verifiable();

            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            ApplicationClaim tempClaim = new ApplicationClaim() { ClaimType = ClaimTypes.Name, ClaimValue = "Test name" };
            mockRepo.Setup(repo => repo.GetApplicationClaimById(It.IsAny<int>())).ReturnsAsync(tempClaim).Verifiable();

            // mocking Response.StatusCode = 404 setter
            var mockHttpContext = new Mock<HttpContext>();
            var response = new Mock<HttpResponse>();
            mockHttpContext.SetupGet(x => x.Response).Returns(response.Object);

            //creates an instance of an asp.net mvc controller
            var controller = new AdministrationController(roleManager, mockUserManager.Object, mockRepo.Object, mapper)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };
            controller.ModelState.AddModelError("ClaimId", "Required");
            ClaimModificationViewModel tempClaimModificationVM = new ClaimModificationViewModel { ClaimId = 123, UserId = "abcd" };

            // Act
            var result = await controller.RemoveClaimFromUser(tempClaimModificationVM);

            // Assert
            var requestResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ManageUserClaims", requestResult.ActionName);
            mockUserManager.Verify(um => um.FindByIdAsync(It.IsAny<string>()), Times.Never);
            mockUserManager.Verify(um => um.RemoveClaimAsync(It.IsAny<ApplicationUser>(), It.IsAny<Claim>()), Times.Never);
            mockRepo.Verify(um => um.GetApplicationClaimById(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task RemoveClaimFromUserPost_ErrorViewOnException()
        {
            // Arrange

            // mocking UserManager            
            var mockUserManager = MockHelpers.MockUserManager<ApplicationUser>();
            ApplicationUser tempUser = new ApplicationUser { Email = "test@email.com" };
            mockUserManager.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(tempUser).Verifiable();
            mockUserManager.Setup(um => um.RemoveClaimAsync(It.IsAny<ApplicationUser>(), It.IsAny<Claim>())).ReturnsAsync(IdentityResult.Failed()).Verifiable();

            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            ApplicationClaim tempClaim = new ApplicationClaim() { ClaimType = ClaimTypes.Name, ClaimValue = "Test name" };
            mockRepo.Setup(repo => repo.GetApplicationClaimById(It.IsAny<int>())).ReturnsAsync(tempClaim).Verifiable();

            // mocking Response.StatusCode = 404 setter
            var mockHttpContext = new Mock<HttpContext>();
            var response = new Mock<HttpResponse>();
            mockHttpContext.SetupGet(x => x.Response).Returns(response.Object);

            //creates an instance of an asp.net mvc controller
            var controller = new AdministrationController(roleManager, mockUserManager.Object, mockRepo.Object, mapper)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };
            ClaimModificationViewModel tempClaimModificationVM = new ClaimModificationViewModel { ClaimId = 123, UserId = "abcd" };

            // Act
            var result = await controller.RemoveClaimFromUser(tempClaimModificationVM);

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", requestResult.ViewName);
            mockUserManager.Verify(um => um.FindByIdAsync(It.IsAny<string>()), Times.Once);
            mockUserManager.Verify(um => um.RemoveClaimAsync(It.IsAny<ApplicationUser>(), It.IsAny<Claim>()), Times.Once);
            mockRepo.Verify(um => um.GetApplicationClaimById(It.IsAny<int>()), Times.Once);
        }


        // helper methods

        private List<Claim> GetClaimsListForMock()
        {
            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name, "Brock"));
            claims.Add(new Claim(ClaimTypes.Email, "brockallen@gmail.com"));
            return claims;
        }

        private List<ApplicationClaim> GetApplicationClaimsListForMock()
        {
            var applicationClaims = new List<ApplicationClaim>();
            applicationClaims.Add(new ApplicationClaim() { ClaimType = ClaimTypes.Name, ClaimValue = "Test name" });
            applicationClaims.Add(new ApplicationClaim() { ClaimType = ClaimTypes.Email, ClaimValue = "test@email.com" });
            return applicationClaims;
        }

        private List<ApplicationUser> GetAllUsersForMock()
        {
            ApplicationUser tempUser = new ApplicationUser
            {
                Id = "abcd",
                Email = "test@email.com"
            };
            List<ApplicationUser> tempList = new List<ApplicationUser>();
            tempList.Add(tempUser);
            return tempList;
        }

    }
}
