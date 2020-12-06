using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Tests.Shared;
using Web.AutomapperProfiles;
using Web.Controllers;
using Xunit;

namespace Tests.Controller
{
    public class UserControllerShould
    {
        private IAsyncRepository repository { get; }
        private IMapper mapper { get; }
        private UserManager<ApplicationUser> userManager { get; }

        [Fact]
        public async Task AllUsers_ReturnAllUsersViewOnSuccess()
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            mockRepo.Setup(repo => repo.GetAllUsers()).ReturnsAsync(new List<ApplicationUser>()).Verifiable();
            
            // adding a real mapper
            var myProfile = new AskMateProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var realMapper = new Mapper(configuration);

            var controller = new UserController(mockRepo.Object, realMapper, userManager);

            // Act

            var result = await controller.AllUsers();

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("AllUsers", requestResult.ViewName);
            mockRepo.Verify(x => x.GetAllUsers(), Times.Once);
        }

        [Fact]
        public async Task AllUsers_ReturnErrorViewOnException()
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            mockRepo.Setup(repo => repo.GetAllUsers()).Throws(new Exception()).Verifiable();

            // adding a real mapper
            var myProfile = new AskMateProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var realMapper = new Mapper(configuration);

            var controller = new UserController(mockRepo.Object, realMapper, userManager);

            // Act

            var result = await controller.AllUsers();

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", requestResult.ViewName);
            mockRepo.Verify(x => x.GetAllUsers(), Times.Once);
        }

        [Fact]
        public async Task UserActivity_ReturnUserAcctivityViewOnSuccess()
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            mockRepo.Setup(repo => repo.GetUserQuestions(It.IsAny<string>())).ReturnsAsync(new List<Question>()).Verifiable();
            mockRepo.Setup(repo => repo.GetUserAnswers(It.IsAny<string>())).ReturnsAsync(new List<Answer>()).Verifiable();
            mockRepo.Setup(repo => repo.GetUserQuestionComments(It.IsAny<string>())).ReturnsAsync(new List<QuestionComment>()).Verifiable();
            mockRepo.Setup(repo => repo.GetUserAnswerComments(It.IsAny<string>())).ReturnsAsync(new List<AnswerComment>()).Verifiable();

            // mocking UserManager            
            var mockUserManager = MockHelpers.MockUserManager<ApplicationUser>();
            ApplicationUser tempUser = new ApplicationUser { Id = "abcd", Email = "test@email.com" };
            mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(tempUser).Verifiable();

            // adding a real mapper
            var myProfile = new AskMateProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var realMapper = new Mapper(configuration);

            var controller = new UserController(mockRepo.Object, realMapper, mockUserManager.Object);

            // Act

            var result = await controller.UserActivity();

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("UserActivity", requestResult.ViewName);
            mockRepo.Verify(x => x.GetUserQuestions(It.IsAny<string>()), Times.Once);
            mockRepo.Verify(x => x.GetUserAnswers(It.IsAny<string>()), Times.Once);
            mockRepo.Verify(x => x.GetUserQuestionComments(It.IsAny<string>()), Times.Once);
            mockRepo.Verify(x => x.GetUserAnswerComments(It.IsAny<string>()), Times.Once);
        }





    }
}
