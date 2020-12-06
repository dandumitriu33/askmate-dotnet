using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
using Web.ViewModels;
using Xunit;

namespace Tests.Controller
{
    public class QuestionsControllerShould
    {
        private IAsyncRepository repository { get; }
        private IMapper mapper { get; }
        private IWebHostEnvironment webHostEnvironment { get; }
        private IFileOperations fileOperations { get; }
        private SignInManager<ApplicationUser> signInManager { get; }
        private UserManager<ApplicationUser> userManager { get; }

        [Fact]
        public async Task Details_ReturnDetailsViewOnSuccess()
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            Question tempQuestion = new Question { Id = 1, Title = "Test Title" };
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempQuestion).Verifiable();
            mockRepo.Setup(repo => repo.GetQuestionByIdAsync(It.IsAny<int>())).ReturnsAsync(tempQuestion).Verifiable();
            mockRepo.Setup(repo => repo.GetTagIdsForQuestionId(It.IsAny<int>())).ReturnsAsync(new List<int> { 1, 2, 3}).Verifiable();
            mockRepo.Setup(repo => repo.GetTagsFromListFromDb(It.IsAny<List<int>>())).ReturnsAsync(new List<Tag> { new Tag(), new Tag()}).Verifiable();


            // adding a real mapper
            var myProfile = new AskMateProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var realMapper = new Mapper(configuration);

            // mocking Response.StatusCode = 404 setter
            var mockHttpContext = new Mock<HttpContext>();
            var response = new Mock<HttpResponse>();
            mockHttpContext.SetupGet(x => x.Response).Returns(response.Object);

            //creates an instance of an asp.net mvc controller
            var controller = new QuestionsController(mockRepo.Object, realMapper, webHostEnvironment, fileOperations, signInManager, userManager)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };

            // Act
            var result = await controller.Details(1);

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Details", requestResult.ViewName);
            mockRepo.Verify(x => x.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(x => x.GetQuestionByIdAsync(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(x => x.GetTagIdsForQuestionId(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(x => x.GetTagsFromListFromDb(It.IsAny<List<int>>()), Times.Once);
        }

        [Fact]
        public async Task Details_ReturnErrorViewOnNullSimpleQuestion()
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            Question tempQuestion = null;
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempQuestion).Verifiable();
            mockRepo.Setup(repo => repo.GetQuestionByIdAsync(It.IsAny<int>())).ReturnsAsync(tempQuestion).Verifiable();
            mockRepo.Setup(repo => repo.GetTagIdsForQuestionId(It.IsAny<int>())).ReturnsAsync(new List<int> { 1, 2, 3 }).Verifiable();
            mockRepo.Setup(repo => repo.GetTagsFromListFromDb(It.IsAny<List<int>>())).ReturnsAsync(new List<Tag> { new Tag(), new Tag() }).Verifiable();


            // adding a real mapper
            var myProfile = new AskMateProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var realMapper = new Mapper(configuration);

            // mocking Response.StatusCode = 404 setter
            var mockHttpContext = new Mock<HttpContext>();
            var response = new Mock<HttpResponse>();
            mockHttpContext.SetupGet(x => x.Response).Returns(response.Object);

            //creates an instance of an asp.net mvc controller
            var controller = new QuestionsController(mockRepo.Object, realMapper, webHostEnvironment, fileOperations, signInManager, userManager)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };

            // Act
            var result = await controller.Details(1);

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", requestResult.ViewName);
            mockRepo.Verify(x => x.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(x => x.GetQuestionByIdAsync(It.IsAny<int>()), Times.Never);
            mockRepo.Verify(x => x.GetTagIdsForQuestionId(It.IsAny<int>()), Times.Never);
            mockRepo.Verify(x => x.GetTagsFromListFromDb(It.IsAny<List<int>>()), Times.Never);
        }

        [Fact]
        public async Task Details_ReturnErrorViewOnException()
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            Question tempQuestion = new Question { Id = 1, Title = "Test Title" };
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempQuestion).Verifiable();
            mockRepo.Setup(repo => repo.GetQuestionByIdAsync(It.IsAny<int>())).ReturnsAsync(tempQuestion).Verifiable();
            mockRepo.Setup(repo => repo.GetTagIdsForQuestionId(It.IsAny<int>())).ReturnsAsync(new List<int> { 1, 2, 3 }).Verifiable();
            mockRepo.Setup(repo => repo.GetTagsFromListFromDb(It.IsAny<List<int>>())).Throws(new Exception()).Verifiable();


            // adding a real mapper
            var myProfile = new AskMateProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var realMapper = new Mapper(configuration);

            // mocking Response.StatusCode = 404 setter
            var mockHttpContext = new Mock<HttpContext>();
            var response = new Mock<HttpResponse>();
            mockHttpContext.SetupGet(x => x.Response).Returns(response.Object);

            //creates an instance of an asp.net mvc controller
            var controller = new QuestionsController(mockRepo.Object, realMapper, webHostEnvironment, fileOperations, signInManager, userManager)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };

            // Act
            var result = await controller.Details(1);

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", requestResult.ViewName);
            mockRepo.Verify(x => x.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(x => x.GetQuestionByIdAsync(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(x => x.GetTagIdsForQuestionId(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(x => x.GetTagsFromListFromDb(It.IsAny<List<int>>()), Times.Once);
        }

        [Fact]
        public void AddQuestionGet_ReturnViewOnSuccess()
        {
            // Arrange
            var controller = new QuestionsController(repository, mapper, webHostEnvironment, fileOperations, signInManager, userManager);

            // Act
            var result = controller.AddQuestion();

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("AddQuestion", requestResult.ViewName);
        }

        [Fact]
        public async Task AddQuestionPost_RedirectToDetailsActionOnSuccess() // Not including IMAGE, too advanced for me at this time
        {
            // Arrange
            // mocking UserManager
            var mockUserManager = MockHelpers.MockUserManager<ApplicationUser>();
            var userFromDb = new ApplicationUser { Email = "test@email.com" };
            mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(userFromDb);

            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            Question tempQuestion = new Question { Id = 1, Title = "Test Title" };
            mockRepo.Setup(repo => repo.AddQuestionAsync(It.IsAny<Question>())).ReturnsAsync(tempQuestion).Verifiable();

            // adding a real mapper
            var myProfile = new AskMateProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var realMapper = new Mapper(configuration);

            // mocking Response.StatusCode = 404 setter
            var mockHttpContext = new Mock<HttpContext>();
            var response = new Mock<HttpResponse>();
            mockHttpContext.SetupGet(x => x.Response).Returns(response.Object);

            //creates an instance of an asp.net mvc controller
            var controller = new QuestionsController(mockRepo.Object, realMapper, webHostEnvironment, fileOperations, signInManager, mockUserManager.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };
            QuestionViewModel tempQuestionVM = new QuestionViewModel() { Title = "Test Title", Body = "Test Body" };

            // Act
            var result = await controller.AddQuestion(tempQuestionVM);

            // Assert
            var requestResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", requestResult.ActionName);
            mockUserManager.Verify(mu => mu.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
            mockRepo.Verify(x => x.AddQuestionAsync(It.IsAny<Question>()), Times.Once);
        }

        [Fact]
        public async Task AddQuestionPost_ReturnAddQuestionViewOnInvalidModel() // Not including IMAGE, too advanced for me at this time
        {
            // Arrange
            // mocking UserManager
            var mockUserManager = MockHelpers.MockUserManager<ApplicationUser>();
            var userFromDb = new ApplicationUser { Email = "test@email.com" };
            mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(userFromDb);

            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            Question tempQuestion = new Question { Id = 1, Title = "Test Title" };
            mockRepo.Setup(repo => repo.AddQuestionAsync(It.IsAny<Question>())).ReturnsAsync(tempQuestion).Verifiable();

            // adding a real mapper
            var myProfile = new AskMateProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var realMapper = new Mapper(configuration);

            // mocking Response.StatusCode = 404 setter
            var mockHttpContext = new Mock<HttpContext>();
            var response = new Mock<HttpResponse>();
            mockHttpContext.SetupGet(x => x.Response).Returns(response.Object);

            //creates an instance of an asp.net mvc controller
            var controller = new QuestionsController(mockRepo.Object, realMapper, webHostEnvironment, fileOperations, signInManager, mockUserManager.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };
            QuestionViewModel tempQuestionVM = new QuestionViewModel() { Title = "Test Title", Body = "Test Body" };
            controller.ModelState.AddModelError("UserId", "Required");

            // Act
            var result = await controller.AddQuestion(tempQuestionVM);

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("AddQuestion", requestResult.ViewName);
            mockUserManager.Verify(mu => mu.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Never);
            mockRepo.Verify(x => x.AddQuestionAsync(It.IsAny<Question>()), Times.Never);
        }



    }
}
