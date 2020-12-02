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
using Web.Controllers;
using Web.ViewModels;
using Xunit;

namespace Tests.Controller
{
    public class AccountControllerShould
    {
        private UserManager<ApplicationUser> userManager { get; }
        private SignInManager<ApplicationUser> signInManager { get; }

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
        public async Task RegisterPost_ReturnsViewResult_WhenModelStateIsInvalid()
        {
            // Arrange
            var controller = new AccountController(userManager, signInManager);
            controller.ModelState.AddModelError("Email", "Required");
            var newRegisterViewModel = new RegisterViewModel();

            // Act
            var result = await controller.Register(newRegisterViewModel);

            // Assert
            var badRequestResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<RegisterViewModel>(badRequestResult.ViewData.Model);
        }

        [Fact]
        public async Task RegisterPost_ReturnsViewResultError_WhenModelStateIsValidAndCreateFails()
        {
            // Arrange
            var controller = new AccountController(userManager, signInManager);
            var newRegisterViewModel = new RegisterViewModel();

            // Act
            var result = await controller.Register(newRegisterViewModel);

            // Assert
            var badRequestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", badRequestResult.ViewName);
        }

        [Fact]
        public void LoginGet_ReturnAViewResult()
        {
            // Arrange
            var controller = new AccountController(userManager, signInManager);

            // Act
            var result = controller.LogIn();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
        }


    }
}
