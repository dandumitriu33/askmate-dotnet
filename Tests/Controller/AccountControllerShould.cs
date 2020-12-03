using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tests.Shared;
using Web.Controllers;
using Web.ViewModels;
using Xunit;
using Xunit.Abstractions;

namespace Tests.Controller
{
    public class AccountControllerShould
    {
        private readonly ITestOutputHelper _output;

        private UserManager<ApplicationUser> userManager { get; }
        private SignInManager<ApplicationUser> signInManager { get; }

        public AccountControllerShould(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void RegisterGet_ReturnAViewResult()
        {
            // Arrange
            var controller = new AccountController(userManager, signInManager);

            // Act
            var result = controller.Register();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task RegisterPost_ReturnRedirectToActionHomeIndex()
        {
            // Arrange
            RegisterViewModel newRegisterVM = new RegisterViewModel();
            
            // mocking UserManager
            var mockUserManager = MockHelpers.MockUserManager<ApplicationUser>();
            mockUserManager.Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success).Verifiable();

            // mocking SignInManager - no need to set up the method, just needs SignInManager to not be null
            var contextAccessor = new Mock<IHttpContextAccessor>();
            var userPrincipalFactory = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
            var mockSignInManager = new Mock<SignInManager<ApplicationUser>>(mockUserManager.Object,
                contextAccessor.Object, userPrincipalFactory.Object, null, null, null, null);

            var controller = new AccountController(mockUserManager.Object, mockSignInManager.Object);

            // Act
            var result = await controller.Register(newRegisterVM);

            // Assert
            var requestResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", requestResult.ActionName);
            Assert.Equal("Home", requestResult.ControllerName);
            mockUserManager.Verify(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task RegisterPost_ReturnErrorViewOnTryCatchFail()
        {
            // Arrange
            RegisterViewModel newRegisterVM = new RegisterViewModel();

            // mocking UserManager
            var mockUserManager = MockHelpers.MockUserManager<ApplicationUser>();
            // IdentityResult returned without Success
            mockUserManager.Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).Throws(new Exception());

            // mocking SignInManager - no need to set up the method, just needs SignInManager to not be null
            var contextAccessor = new Mock<IHttpContextAccessor>();
            var userPrincipalFactory = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
            var mockSignInManager = new Mock<SignInManager<ApplicationUser>>(mockUserManager.Object,
                contextAccessor.Object, userPrincipalFactory.Object, null, null, null, null);

            var controller = new AccountController(mockUserManager.Object, mockSignInManager.Object);

            // Act
            var result = await controller.Register(newRegisterVM);

            // Assert
            var badRequestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", badRequestResult.ViewName);
        }

        [Fact]
        public async Task RegisterPost_ReturnsRegisterViewOnModelIsValidFalse()
        {
            // Arrange
            RegisterViewModel newRegisterVM = new RegisterViewModel();

            // mocking UserManager
            var mockUserManager = MockHelpers.MockUserManager<ApplicationUser>();
            mockUserManager.Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success).Verifiable();

            // mocking SignInManager - no need to set up the method, just needs SignInManager to not be null
            var contextAccessor = new Mock<IHttpContextAccessor>();
            var userPrincipalFactory = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
            var mockSignInManager = new Mock<SignInManager<ApplicationUser>>(mockUserManager.Object,
                contextAccessor.Object, userPrincipalFactory.Object, null, null, null, null);

            var controller = new AccountController(mockUserManager.Object, mockSignInManager.Object);
            controller.ModelState.AddModelError("Email", "Required");

            // Act
            var result = await controller.Register(newRegisterVM);

            // Assert
            var badRequestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Register", badRequestResult.ViewName);
        }





        //[Fact]
        //public async Task RegisterPost_ReturnsViewResult_WhenModelStateIsInvalid()
        //{
        //    // Arrange
        //    var controller = new AccountController(userManager, signInManager);
        //    controller.ModelState.AddModelError("Email", "Required");
        //    var newRegisterViewModel = new RegisterViewModel();

        //    // Act
        //    var result = await controller.Register(newRegisterViewModel);

        //    // Assert
        //    var badRequestResult = Assert.IsType<ViewResult>(result);
        //    var model = Assert.IsAssignableFrom<RegisterViewModel>(badRequestResult.ViewData.Model);
        //}

        //[Fact]
        //public async Task RegisterPost_ReturnsViewResultError_WhenModelStateIsValidAndCreateFails()
        //{
        //    // CreateAsync and SignInAsync are not mocked and should fail
        //    // Arrange
        //    var controller = new AccountController(userManager, signInManager);
        //    var newRegisterViewModel = new RegisterViewModel();

        //    // Act
        //    var result = await controller.Register(newRegisterViewModel);

        //    // Assert
        //    var badRequestResult = Assert.IsType<ViewResult>(result);
        //    Assert.Equal("Error", badRequestResult.ViewName);
        //}

        //[Fact]
        //public void LoginGet_ReturnAViewResult()
        //{
        //    // Arrange
        //    var controller = new AccountController(userManager, signInManager);

        //    // Act
        //    var result = controller.LogIn();

        //    // Assert
        //    var viewResult = Assert.IsType<ViewResult>(result);
        //}

        //[Fact]
        //public async Task LogIPost_ReturnsViewResult_WhenModelStateIsInvalid()
        //{
        //    // Arrange
        //    var controller = new AccountController(userManager, signInManager);
        //    controller.ModelState.AddModelError("Email", "Required");
        //    var newLogInViewModel = new LogInViewModel();

        //    // Act
        //    var result = await controller.LogIn(newLogInViewModel);

        //    // Assert
        //    var badRequestResult = Assert.IsType<ViewResult>(result);
        //    Assert.IsAssignableFrom<LogInViewModel>(badRequestResult.ViewData.Model);
        //}

        //[Fact]
        //public async Task LogInPost_ReturnsViewResultError_WhenModelStateIsValidAndSignInFails()
        //{
        //    // Arrange
        //    // SignInAsync is not mocked and should fail
        //    var controller = new AccountController(userManager, signInManager);
        //    var newLogInViewModel = new LogInViewModel();

        //    // Act
        //    var result = await controller.LogIn(newLogInViewModel);

        //    // Assert
        //    var badRequestResult = Assert.IsType<ViewResult>(result);
        //    Assert.Equal("Error", badRequestResult.ViewName);
        //}

        //[Fact]
        //public async Task LogOutPost_ReturnViewResultErrorOnFail()
        //{
        //    // Arrange
        //    // SignOutAsync is not mocked and should fail
        //    var controller = new AccountController(userManager, signInManager);

        //    // Act
        //    var result = await controller.LogOut();

        //    // Assert
        //    var viewResult = Assert.IsType<ViewResult>(result);
        //    Assert.Equal("Error", viewResult.ViewName);
        //}

        //[Fact]
        //public void AccessDeniedGet_ReturnAViewResult()
        //{
        //    // Arrange
        //    var controller = new AccountController(userManager, signInManager);

        //    // Act
        //    var result = controller.AccessDenied();

        //    // Assert
        //    var viewResult = Assert.IsType<ViewResult>(result);
        //}

    }
}
