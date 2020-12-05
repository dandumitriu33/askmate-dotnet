using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using AutoMapper;
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
    public class CommentsControllerShould
    {
        private UserManager<ApplicationUser> userManager { get; }
        private IAsyncRepository repository { get; }
        private IMapper mapper { get; }

        [Fact]
        public async Task AddQuestionCommentGet_ReturnViewAddQuestionCommentOnSuccess()
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            Question tempQuestion = new Question { Id = 1, Title = "Test Title" };
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempQuestion).Verifiable();

            // mocking Response.StatusCode = 404 setter
            var mockHttpContext = new Mock<HttpContext>();
            var response = new Mock<HttpResponse>();
            mockHttpContext.SetupGet(x => x.Response).Returns(response.Object);

            //creates an instance of an asp.net mvc controller
            var controller = new CommentsController(mapper, mockRepo.Object, userManager)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };

            // Act
            var result = await controller.AddQuestionComment(1);

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("AddQuestionComment", requestResult.ViewName);
            mockRepo.Verify(x => x.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task AddQuestionCommentGet_ReturnViewErrorOnNullQuestion()
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            Question tempQuestion = null;
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempQuestion).Verifiable();

            // mocking Response.StatusCode = 404 setter
            var mockHttpContext = new Mock<HttpContext>();
            var response = new Mock<HttpResponse>();
            mockHttpContext.SetupGet(x => x.Response).Returns(response.Object);

            //creates an instance of an asp.net mvc controller
            var controller = new CommentsController(mapper, mockRepo.Object, userManager)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };

            // Act
            var result = await controller.AddQuestionComment(1);

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", requestResult.ViewName);
            mockRepo.Verify(x => x.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task AddQuestionCommentGet_ReturnViewErrorOnException()
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            Question tempQuestion = new Question { Id = 1, Title = "Test Title" };
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>())).Throws(new Exception()).Verifiable();

            // mocking Response.StatusCode = 404 setter
            var mockHttpContext = new Mock<HttpContext>();
            var response = new Mock<HttpResponse>();
            mockHttpContext.SetupGet(x => x.Response).Returns(response.Object);

            //creates an instance of an asp.net mvc controller
            var controller = new CommentsController(mapper, mockRepo.Object, userManager)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };

            // Act
            var result = await controller.AddQuestionComment(1);

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", requestResult.ViewName);
            mockRepo.Verify(x => x.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task AddAnswerCommentGet_ReturnViewAddAnswerCommentOnSuccess()
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            Answer tempAnswer = new Answer { Id = 1, Body = "Test Body", UserId = "abcd" };
            mockRepo.Setup(repo => repo.GetAnswerByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempAnswer).Verifiable();
            Question tempQuestion = new Question { Id = 1, Title = "Test Title" };
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempQuestion).Verifiable();

            // mocking Response.StatusCode = 404 setter
            var mockHttpContext = new Mock<HttpContext>();
            var response = new Mock<HttpResponse>();
            mockHttpContext.SetupGet(x => x.Response).Returns(response.Object);

            //creates an instance of an asp.net mvc controller
            var controller = new CommentsController(mapper, mockRepo.Object, userManager)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };

            // Act
            var result = await controller.AddAnswerComment(1, 1);

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("AddAnswerComment", requestResult.ViewName);
            mockRepo.Verify(x => x.GetAnswerByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(x => x.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task AddAnswerCommentGet_ReturnViewErrorOnNullAnswer()
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            Answer tempAnswer = null;
            mockRepo.Setup(repo => repo.GetAnswerByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempAnswer).Verifiable();
            Question tempQuestion = new Question { Id = 1, Title = "Test Title" };
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempQuestion).Verifiable();

            // mocking Response.StatusCode = 404 setter
            var mockHttpContext = new Mock<HttpContext>();
            var response = new Mock<HttpResponse>();
            mockHttpContext.SetupGet(x => x.Response).Returns(response.Object);

            //creates an instance of an asp.net mvc controller
            var controller = new CommentsController(mapper, mockRepo.Object, userManager)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };

            // Act
            var result = await controller.AddAnswerComment(1, 1);

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", requestResult.ViewName);
            mockRepo.Verify(x => x.GetAnswerByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(x => x.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task AddAnswerCommentGet_ReturnViewErrorOnNullQuestion()
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            Answer tempAnswer = new Answer { Id = 1, Body = "Test Body", UserId = "abcd" };
            mockRepo.Setup(repo => repo.GetAnswerByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempAnswer).Verifiable();
            Question tempQuestion = null;
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempQuestion).Verifiable();

            // mocking Response.StatusCode = 404 setter
            var mockHttpContext = new Mock<HttpContext>();
            var response = new Mock<HttpResponse>();
            mockHttpContext.SetupGet(x => x.Response).Returns(response.Object);

            //creates an instance of an asp.net mvc controller
            var controller = new CommentsController(mapper, mockRepo.Object, userManager)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };

            // Act
            var result = await controller.AddAnswerComment(1, 1);

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", requestResult.ViewName);
            mockRepo.Verify(x => x.GetAnswerByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(x => x.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task AddAnswerCommentGet_ReturnViewErrorOnException()
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            Answer tempAnswer = new Answer { Id = 1, Body = "Test Body", UserId = "abcd" };
            mockRepo.Setup(repo => repo.GetAnswerByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempAnswer).Verifiable();
            Question tempQuestion = new Question { Id = 1, Title = "Test Title" };
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>())).Throws(new Exception()).Verifiable();

            // mocking Response.StatusCode = 404 setter
            var mockHttpContext = new Mock<HttpContext>();
            var response = new Mock<HttpResponse>();
            mockHttpContext.SetupGet(x => x.Response).Returns(response.Object);

            //creates an instance of an asp.net mvc controller
            var controller = new CommentsController(mapper, mockRepo.Object, userManager)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };

            // Act
            var result = await controller.AddAnswerComment(1, 1);

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", requestResult.ViewName);
            mockRepo.Verify(x => x.GetAnswerByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(x => x.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task AddQuestionCommentPost_RedirectDetailsOnSuccess()
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            Question tempQuestion = new Question { Id = 1, Title = "Test Title" };
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempQuestion).Verifiable();
            mockRepo.Setup(repo => repo.AddQuestionCommentAsync(It.IsAny<QuestionComment>())).ReturnsAsync(new QuestionComment()).Verifiable();

            // mocking UserManager            
            var mockUserManager = MockHelpers.MockUserManager<ApplicationUser>();
            ApplicationUser tempUser = new ApplicationUser { Id = "abcd", Email = "test@email.com" };
            mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(tempUser).Verifiable();

            // adding a real mapper
            var myProfile = new AskMateProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var realMapper = new Mapper(configuration);

            // mocking Response.StatusCode = 404 setter
            var mockHttpContext = new Mock<HttpContext>();
            var response = new Mock<HttpResponse>();
            mockHttpContext.SetupGet(x => x.Response).Returns(response.Object);

            //creates an instance of an asp.net mvc controller
            var controller = new CommentsController(realMapper, mockRepo.Object, mockUserManager.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };
            QuestionCommentViewModel tempQuestionCommentVM = new QuestionCommentViewModel() { QuestionId = 1, Body = "Test Body" };

            // Act
            var result = await controller.AddQuestionComment(tempQuestionCommentVM);

            // Assert
            var requestResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", requestResult.ActionName);
            Assert.Equal("Questions", requestResult.ControllerName);
            mockRepo.Verify(x => x.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
            mockUserManager.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
            mockRepo.Verify(x => x.AddQuestionCommentAsync(It.IsAny<QuestionComment>()), Times.Once);
        }

        [Fact]
        public async Task AddQuestionCommentPost_ReturnsAddQuestionCommentViewOnInvalidModel()
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            Question tempQuestion = new Question { Id = 1, Title = "Test Title" };
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempQuestion).Verifiable();
            mockRepo.Setup(repo => repo.AddQuestionCommentAsync(It.IsAny<QuestionComment>())).ReturnsAsync(new QuestionComment()).Verifiable();

            // mocking UserManager            
            var mockUserManager = MockHelpers.MockUserManager<ApplicationUser>();
            ApplicationUser tempUser = new ApplicationUser { Id = "abcd", Email = "test@email.com" };
            mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(tempUser).Verifiable();

            // adding a real mapper
            var myProfile = new AskMateProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var realMapper = new Mapper(configuration);

            // mocking Response.StatusCode = 404 setter
            var mockHttpContext = new Mock<HttpContext>();
            var response = new Mock<HttpResponse>();
            mockHttpContext.SetupGet(x => x.Response).Returns(response.Object);

            //creates an instance of an asp.net mvc controller
            var controller = new CommentsController(realMapper, mockRepo.Object, mockUserManager.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };
            QuestionCommentViewModel tempQuestionCommentVM = new QuestionCommentViewModel() { QuestionId = 1, Body = "Test Body" };
            controller.ModelState.AddModelError("Body", "Required");

            // Act
            var result = await controller.AddQuestionComment(tempQuestionCommentVM);

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("AddQuestionComment", requestResult.ViewName);
            mockRepo.Verify(x => x.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Never);
            mockUserManager.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Never);
            mockRepo.Verify(x => x.AddQuestionCommentAsync(It.IsAny<QuestionComment>()), Times.Never);
        }

        [Fact]
        public async Task AddQuestionCommentPost_ReturnsErrorViewOnNullQuestion()
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            Question tempQuestion = null;
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempQuestion).Verifiable();
            mockRepo.Setup(repo => repo.AddQuestionCommentAsync(It.IsAny<QuestionComment>())).ReturnsAsync(new QuestionComment()).Verifiable();

            // mocking UserManager            
            var mockUserManager = MockHelpers.MockUserManager<ApplicationUser>();
            ApplicationUser tempUser = new ApplicationUser { Id = "abcd", Email = "test@email.com" };
            mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(tempUser).Verifiable();

            // adding a real mapper
            var myProfile = new AskMateProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var realMapper = new Mapper(configuration);

            // mocking Response.StatusCode = 404 setter
            var mockHttpContext = new Mock<HttpContext>();
            var response = new Mock<HttpResponse>();
            mockHttpContext.SetupGet(x => x.Response).Returns(response.Object);

            //creates an instance of an asp.net mvc controller
            var controller = new CommentsController(realMapper, mockRepo.Object, mockUserManager.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };
            QuestionCommentViewModel tempQuestionCommentVM = new QuestionCommentViewModel() { QuestionId = 1, Body = "Test Body" };
            
            // Act
            var result = await controller.AddQuestionComment(tempQuestionCommentVM);

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", requestResult.ViewName);
            mockRepo.Verify(x => x.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
            mockUserManager.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Never);
            mockRepo.Verify(x => x.AddQuestionCommentAsync(It.IsAny<QuestionComment>()), Times.Never);
        }

        [Fact]
        public async Task AddQuestionCommentPost_ReturnsErrorViewOnException()
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            Question tempQuestion = new Question { Id = 1, Title = "Test Title" };
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempQuestion).Verifiable();
            mockRepo.Setup(repo => repo.AddQuestionCommentAsync(It.IsAny<QuestionComment>())).Throws(new Exception()).Verifiable();

            // mocking UserManager            
            var mockUserManager = MockHelpers.MockUserManager<ApplicationUser>();
            ApplicationUser tempUser = new ApplicationUser { Id = "abcd", Email = "test@email.com" };
            mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(tempUser).Verifiable();

            // adding a real mapper
            var myProfile = new AskMateProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var realMapper = new Mapper(configuration);

            // mocking Response.StatusCode = 404 setter
            var mockHttpContext = new Mock<HttpContext>();
            var response = new Mock<HttpResponse>();
            mockHttpContext.SetupGet(x => x.Response).Returns(response.Object);

            //creates an instance of an asp.net mvc controller
            var controller = new CommentsController(realMapper, mockRepo.Object, mockUserManager.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };
            QuestionCommentViewModel tempQuestionCommentVM = new QuestionCommentViewModel() { QuestionId = 1, Body = "Test Body" };

            // Act
            var result = await controller.AddQuestionComment(tempQuestionCommentVM);

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", requestResult.ViewName);
            mockRepo.Verify(x => x.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
            mockUserManager.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
            mockRepo.Verify(x => x.AddQuestionCommentAsync(It.IsAny<QuestionComment>()), Times.Once);
        }

        [Fact]
        public async Task AddAnswerCommentPost_RedirectDetailsOnSuccess()
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            Answer tempAnswer = new Answer { Id = 1, Body = "Test Body", UserId = "abcd" };
            mockRepo.Setup(repo => repo.GetAnswerByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempAnswer).Verifiable();
            Question tempQuestion = new Question { Id = 1, Title = "Test Title" };
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempQuestion).Verifiable();
            mockRepo.Setup(repo => repo.AddAnswerCommentAsync(It.IsAny<AnswerComment>())).ReturnsAsync(new AnswerComment()).Verifiable();

            // mocking UserManager            
            var mockUserManager = MockHelpers.MockUserManager<ApplicationUser>();
            ApplicationUser tempUser = new ApplicationUser { Id = "abcd", Email = "test@email.com" };
            mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(tempUser).Verifiable();

            // adding a real mapper
            var myProfile = new AskMateProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var realMapper = new Mapper(configuration);

            // mocking Response.StatusCode = 404 setter
            var mockHttpContext = new Mock<HttpContext>();
            var response = new Mock<HttpResponse>();
            mockHttpContext.SetupGet(x => x.Response).Returns(response.Object);

            //creates an instance of an asp.net mvc controller
            var controller = new CommentsController(realMapper, mockRepo.Object, mockUserManager.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };
            AnswerCommentViewModel tempAnswerCommentVM = new AnswerCommentViewModel() { AnswerId = 1, Body = "Test Body" };

            // Act
            var result = await controller.AddAnswerComment(tempAnswerCommentVM);

            // Assert
            var requestResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", requestResult.ActionName);
            Assert.Equal("Questions", requestResult.ControllerName);
            mockRepo.Verify(x => x.GetAnswerByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(x => x.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
            mockUserManager.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
            mockRepo.Verify(x => x.AddAnswerCommentAsync(It.IsAny<AnswerComment>()), Times.Once);
        }

        [Fact]
        public async Task AddAnswerCommentPost_ReturnAddAnswerCommentViewOnInvalidModel()
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            Answer tempAnswer = new Answer { Id = 1, Body = "Test Body", UserId = "abcd" };
            mockRepo.Setup(repo => repo.GetAnswerByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempAnswer).Verifiable();
            Question tempQuestion = new Question { Id = 1, Title = "Test Title" };
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempQuestion).Verifiable();
            mockRepo.Setup(repo => repo.AddAnswerCommentAsync(It.IsAny<AnswerComment>())).ReturnsAsync(new AnswerComment()).Verifiable();

            // mocking UserManager            
            var mockUserManager = MockHelpers.MockUserManager<ApplicationUser>();
            ApplicationUser tempUser = new ApplicationUser { Id = "abcd", Email = "test@email.com" };
            mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(tempUser).Verifiable();

            // adding a real mapper
            var myProfile = new AskMateProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var realMapper = new Mapper(configuration);

            // mocking Response.StatusCode = 404 setter
            var mockHttpContext = new Mock<HttpContext>();
            var response = new Mock<HttpResponse>();
            mockHttpContext.SetupGet(x => x.Response).Returns(response.Object);

            //creates an instance of an asp.net mvc controller
            var controller = new CommentsController(realMapper, mockRepo.Object, mockUserManager.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };
            AnswerCommentViewModel tempAnswerCommentVM = new AnswerCommentViewModel() { AnswerId = 1, Body = "Test Body" };
            controller.ModelState.AddModelError("AnswerId", "Required");

            // Act
            var result = await controller.AddAnswerComment(tempAnswerCommentVM);

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("AddAnswerComment", requestResult.ViewName);
            mockRepo.Verify(x => x.GetAnswerByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Never);
            mockRepo.Verify(x => x.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Never);
            mockUserManager.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Never);
            mockRepo.Verify(x => x.AddAnswerCommentAsync(It.IsAny<AnswerComment>()), Times.Never);
        }

        [Fact]
        public async Task AddAnswerCommentPost_ReturnErrorOnNullAnswer()
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            Answer tempAnswer = null;
            mockRepo.Setup(repo => repo.GetAnswerByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempAnswer).Verifiable();
            Question tempQuestion = new Question { Id = 1, Title = "Test Title" };
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempQuestion).Verifiable();
            mockRepo.Setup(repo => repo.AddAnswerCommentAsync(It.IsAny<AnswerComment>())).ReturnsAsync(new AnswerComment()).Verifiable();

            // mocking UserManager            
            var mockUserManager = MockHelpers.MockUserManager<ApplicationUser>();
            ApplicationUser tempUser = new ApplicationUser { Id = "abcd", Email = "test@email.com" };
            mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(tempUser).Verifiable();

            // adding a real mapper
            var myProfile = new AskMateProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var realMapper = new Mapper(configuration);

            // mocking Response.StatusCode = 404 setter
            var mockHttpContext = new Mock<HttpContext>();
            var response = new Mock<HttpResponse>();
            mockHttpContext.SetupGet(x => x.Response).Returns(response.Object);

            //creates an instance of an asp.net mvc controller
            var controller = new CommentsController(realMapper, mockRepo.Object, mockUserManager.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };
            AnswerCommentViewModel tempAnswerCommentVM = new AnswerCommentViewModel() { AnswerId = 1, Body = "Test Body" };

            // Act
            var result = await controller.AddAnswerComment(tempAnswerCommentVM);

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", requestResult.ViewName);
            mockRepo.Verify(x => x.GetAnswerByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(x => x.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Never);
            mockUserManager.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Never);
            mockRepo.Verify(x => x.AddAnswerCommentAsync(It.IsAny<AnswerComment>()), Times.Never);
        }

        [Fact]
        public async Task AddAnswerCommentPost_ReturnErrorOnNullQuestion()
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            Answer tempAnswer = new Answer { Id = 1, Body = "Test Body", UserId = "abcd" };
            mockRepo.Setup(repo => repo.GetAnswerByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempAnswer).Verifiable();
            Question tempQuestion = null;
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempQuestion).Verifiable();
            mockRepo.Setup(repo => repo.AddAnswerCommentAsync(It.IsAny<AnswerComment>())).ReturnsAsync(new AnswerComment()).Verifiable();

            // mocking UserManager            
            var mockUserManager = MockHelpers.MockUserManager<ApplicationUser>();
            ApplicationUser tempUser = new ApplicationUser { Id = "abcd", Email = "test@email.com" };
            mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(tempUser).Verifiable();

            // adding a real mapper
            var myProfile = new AskMateProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var realMapper = new Mapper(configuration);

            // mocking Response.StatusCode = 404 setter
            var mockHttpContext = new Mock<HttpContext>();
            var response = new Mock<HttpResponse>();
            mockHttpContext.SetupGet(x => x.Response).Returns(response.Object);

            //creates an instance of an asp.net mvc controller
            var controller = new CommentsController(realMapper, mockRepo.Object, mockUserManager.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };
            AnswerCommentViewModel tempAnswerCommentVM = new AnswerCommentViewModel() { AnswerId = 1, Body = "Test Body" };

            // Act
            var result = await controller.AddAnswerComment(tempAnswerCommentVM);

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", requestResult.ViewName);
            mockRepo.Verify(x => x.GetAnswerByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(x => x.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
            mockUserManager.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Never);
            mockRepo.Verify(x => x.AddAnswerCommentAsync(It.IsAny<AnswerComment>()), Times.Never);
        }

        [Fact]
        public async Task AddAnswerCommentPost_ReturnErrorOnException()
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            Answer tempAnswer = new Answer { Id = 1, Body = "Test Body", UserId = "abcd" };
            mockRepo.Setup(repo => repo.GetAnswerByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempAnswer).Verifiable();
            Question tempQuestion = new Question { Id = 1, Title = "Test Title" };
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempQuestion).Verifiable();
            mockRepo.Setup(repo => repo.AddAnswerCommentAsync(It.IsAny<AnswerComment>())).Throws(new Exception()).Verifiable();

            // mocking UserManager            
            var mockUserManager = MockHelpers.MockUserManager<ApplicationUser>();
            ApplicationUser tempUser = new ApplicationUser { Id = "abcd", Email = "test@email.com" };
            mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(tempUser).Verifiable();

            // adding a real mapper
            var myProfile = new AskMateProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var realMapper = new Mapper(configuration);

            // mocking Response.StatusCode = 404 setter
            var mockHttpContext = new Mock<HttpContext>();
            var response = new Mock<HttpResponse>();
            mockHttpContext.SetupGet(x => x.Response).Returns(response.Object);

            //creates an instance of an asp.net mvc controller
            var controller = new CommentsController(realMapper, mockRepo.Object, mockUserManager.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };
            AnswerCommentViewModel tempAnswerCommentVM = new AnswerCommentViewModel() { AnswerId = 1, Body = "Test Body" };

            // Act
            var result = await controller.AddAnswerComment(tempAnswerCommentVM);

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", requestResult.ViewName);
            mockRepo.Verify(x => x.GetAnswerByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(x => x.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
            mockUserManager.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
            mockRepo.Verify(x => x.AddAnswerCommentAsync(It.IsAny<AnswerComment>()), Times.Once);
        }

        [Fact]
        public async Task EditQuestionCommentGet_ReturnEditQuestionCommentViewOnSuccess()
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            QuestionComment tempQuestionComment = new QuestionComment { Id = 1, Body = "Test Body", UserId = "abcd" };
            mockRepo.Setup(repo => repo.GetQuestionCommentById(It.IsAny<int>())).ReturnsAsync(tempQuestionComment).Verifiable();
            Question tempQuestion = new Question { Id = 1, Title = "Test Title" };
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempQuestion).Verifiable();

            // mock ClaimsPrincipal
            // https://stackoverflow.com/questions/38557942/mocking-iprincipal-in-asp-net-core
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "example name"),
                new Claim(ClaimTypes.NameIdentifier, "abcd"),
                new Claim("custom-claim", "example claim value"),
            }, "mock"));

            // mocking Response.StatusCode = 404 setter
            var mockHttpContext = new Mock<HttpContext>();
            var response = new Mock<HttpResponse>();
            mockHttpContext.SetupGet(x => x.Response).Returns(response.Object);

            // adding a real mapper
            var myProfile = new AskMateProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var realMapper = new Mapper(configuration);

            //creates an instance of an asp.net mvc controller
            var controller = new CommentsController(realMapper, mockRepo.Object, userManager)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            // Act
            var result = await controller.EditQuestionComment(1);

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("EditQuestionComment", requestResult.ViewName);
            mockRepo.Verify(x => x.GetQuestionCommentById(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(x => x.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task EditQuestionCommentGet_ReturnErrorViewOnNullQuestionComment()
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            QuestionComment tempQuestionComment = null;
            mockRepo.Setup(repo => repo.GetQuestionCommentById(It.IsAny<int>())).ReturnsAsync(tempQuestionComment).Verifiable();
            Question tempQuestion = new Question { Id = 1, Title = "Test Title" };
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempQuestion).Verifiable();

            // mock ClaimsPrincipal
            // https://stackoverflow.com/questions/38557942/mocking-iprincipal-in-asp-net-core
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "example name"),
                new Claim(ClaimTypes.NameIdentifier, "abcd"),
                new Claim("custom-claim", "example claim value"),
            }, "mock"));

            // mocking Response.StatusCode = 404 setter
            var mockHttpContext = new Mock<HttpContext>();
            var response = new Mock<HttpResponse>();
            mockHttpContext.SetupGet(x => x.Response).Returns(response.Object);

            // adding a real mapper
            var myProfile = new AskMateProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var realMapper = new Mapper(configuration);

            //creates an instance of an asp.net mvc controller
            var controller = new CommentsController(realMapper, mockRepo.Object, userManager)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            // Act
            var result = await controller.EditQuestionComment(1);

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", requestResult.ViewName);
            mockRepo.Verify(x => x.GetQuestionCommentById(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(x => x.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task EditQuestionCommentGet_ReturnErrorViewOnNullQuestion()
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            QuestionComment tempQuestionComment = new QuestionComment { Id = 1, Body = "Test Body", UserId = "abcd" };
            mockRepo.Setup(repo => repo.GetQuestionCommentById(It.IsAny<int>())).ReturnsAsync(tempQuestionComment).Verifiable();
            Question tempQuestion = null;
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempQuestion).Verifiable();

            // mock ClaimsPrincipal
            // https://stackoverflow.com/questions/38557942/mocking-iprincipal-in-asp-net-core
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "example name"),
                new Claim(ClaimTypes.NameIdentifier, "abcd"),
                new Claim("custom-claim", "example claim value"),
            }, "mock"));

            // mocking Response.StatusCode = 404 setter
            var mockHttpContext = new Mock<HttpContext>();
            var response = new Mock<HttpResponse>();
            mockHttpContext.SetupGet(x => x.Response).Returns(response.Object);

            // adding a real mapper
            var myProfile = new AskMateProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var realMapper = new Mapper(configuration);

            //creates an instance of an asp.net mvc controller
            var controller = new CommentsController(realMapper, mockRepo.Object, userManager)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            // Act
            var result = await controller.EditQuestionComment(1);

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", requestResult.ViewName);
            mockRepo.Verify(x => x.GetQuestionCommentById(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(x => x.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task EditQuestionCommentGet_ReturnErrorViewOnException()
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            QuestionComment tempQuestionComment = new QuestionComment { Id = 1, Body = "Test Body", UserId = "abcd" };
            mockRepo.Setup(repo => repo.GetQuestionCommentById(It.IsAny<int>())).ReturnsAsync(tempQuestionComment).Verifiable();
            Question tempQuestion = new Question { Id = 1, Title = "Test Title" };
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>())).Throws(new Exception()).Verifiable();

            // mock ClaimsPrincipal
            // https://stackoverflow.com/questions/38557942/mocking-iprincipal-in-asp-net-core
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "example name"),
                new Claim(ClaimTypes.NameIdentifier, "abcd"),
                new Claim("custom-claim", "example claim value"),
            }, "mock"));

            // mocking Response.StatusCode = 404 setter
            var mockHttpContext = new Mock<HttpContext>();
            var response = new Mock<HttpResponse>();
            mockHttpContext.SetupGet(x => x.Response).Returns(response.Object);

            // adding a real mapper
            var myProfile = new AskMateProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var realMapper = new Mapper(configuration);

            //creates an instance of an asp.net mvc controller
            var controller = new CommentsController(realMapper, mockRepo.Object, userManager)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            // Act
            var result = await controller.EditQuestionComment(1);

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", requestResult.ViewName);
            mockRepo.Verify(x => x.GetQuestionCommentById(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(x => x.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task EditAnswerCommentGet_ReturnEditAnswerCommentViewOnSuccess()
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            AnswerComment tempAnswerComment = new AnswerComment { Id = 1, Body = "Test Body", UserId = "abcd" };
            mockRepo.Setup(repo => repo.GetAnswerCommentById(It.IsAny<int>())).ReturnsAsync(tempAnswerComment).Verifiable();
            Question tempQuestion = new Question { Id = 1, Title = "Test Title" };
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempQuestion).Verifiable();
            Answer tempAnswer = new Answer { Id = 1, Body = "Test Body", UserId = "abcd" };
            mockRepo.Setup(repo => repo.GetAnswerByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempAnswer).Verifiable();

            // mock ClaimsPrincipal
            // https://stackoverflow.com/questions/38557942/mocking-iprincipal-in-asp-net-core
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "example name"),
                new Claim(ClaimTypes.NameIdentifier, "abcd"),
                new Claim("custom-claim", "example claim value"),
            }, "mock"));

            // mocking Response.StatusCode = 404 setter
            var mockHttpContext = new Mock<HttpContext>();
            var response = new Mock<HttpResponse>();
            mockHttpContext.SetupGet(x => x.Response).Returns(response.Object);

            // adding a real mapper
            var myProfile = new AskMateProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var realMapper = new Mapper(configuration);

            //creates an instance of an asp.net mvc controller
            var controller = new CommentsController(realMapper, mockRepo.Object, userManager)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            // Act
            var result = await controller.EditAnswerComment(1);

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("EditAnswerComment", requestResult.ViewName);
            mockRepo.Verify(x => x.GetAnswerCommentById(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(x => x.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(x => x.GetAnswerByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task EditAnswerCommentGet_ReturnErrorViewOnNullAnswerComment()
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            AnswerComment tempAnswerComment = null;
            mockRepo.Setup(repo => repo.GetAnswerCommentById(It.IsAny<int>())).ReturnsAsync(tempAnswerComment).Verifiable();
            Question tempQuestion = new Question { Id = 1, Title = "Test Title" };
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempQuestion).Verifiable();
            Answer tempAnswer = new Answer { Id = 1, Body = "Test Body", UserId = "abcd" };
            mockRepo.Setup(repo => repo.GetAnswerByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempAnswer).Verifiable();

            // mock ClaimsPrincipal
            // https://stackoverflow.com/questions/38557942/mocking-iprincipal-in-asp-net-core
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "example name"),
                new Claim(ClaimTypes.NameIdentifier, "abcd"),
                new Claim("custom-claim", "example claim value"),
            }, "mock"));

            // mocking Response.StatusCode = 404 setter
            var mockHttpContext = new Mock<HttpContext>();
            var response = new Mock<HttpResponse>();
            mockHttpContext.SetupGet(x => x.Response).Returns(response.Object);

            // adding a real mapper
            var myProfile = new AskMateProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var realMapper = new Mapper(configuration);

            //creates an instance of an asp.net mvc controller
            var controller = new CommentsController(realMapper, mockRepo.Object, userManager)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            // Act
            var result = await controller.EditAnswerComment(1);

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", requestResult.ViewName);
            mockRepo.Verify(x => x.GetAnswerCommentById(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(x => x.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Never);
            mockRepo.Verify(x => x.GetAnswerByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task EditAnswerCommentGet_ReturnErrorViewOnNullQuestion()
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            AnswerComment tempAnswerComment = new AnswerComment { Id = 1, Body = "Test Body", UserId = "abcd" };
            mockRepo.Setup(repo => repo.GetAnswerCommentById(It.IsAny<int>())).ReturnsAsync(tempAnswerComment).Verifiable();
            Question tempQuestion = null;
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempQuestion).Verifiable();
            Answer tempAnswer = new Answer { Id = 1, Body = "Test Body", UserId = "abcd" };
            mockRepo.Setup(repo => repo.GetAnswerByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempAnswer).Verifiable();

            // mock ClaimsPrincipal
            // https://stackoverflow.com/questions/38557942/mocking-iprincipal-in-asp-net-core
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "example name"),
                new Claim(ClaimTypes.NameIdentifier, "abcd"),
                new Claim("custom-claim", "example claim value"),
            }, "mock"));

            // mocking Response.StatusCode = 404 setter
            var mockHttpContext = new Mock<HttpContext>();
            var response = new Mock<HttpResponse>();
            mockHttpContext.SetupGet(x => x.Response).Returns(response.Object);

            // adding a real mapper
            var myProfile = new AskMateProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var realMapper = new Mapper(configuration);

            //creates an instance of an asp.net mvc controller
            var controller = new CommentsController(realMapper, mockRepo.Object, userManager)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            // Act
            var result = await controller.EditAnswerComment(1);

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", requestResult.ViewName);
            mockRepo.Verify(x => x.GetAnswerCommentById(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(x => x.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(x => x.GetAnswerByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task EditAnswerCommentGet_ReturnErrorViewOnNullAnswer()
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            AnswerComment tempAnswerComment = new AnswerComment { Id = 1, Body = "Test Body", UserId = "abcd" };
            mockRepo.Setup(repo => repo.GetAnswerCommentById(It.IsAny<int>())).ReturnsAsync(tempAnswerComment).Verifiable();
            Question tempQuestion = new Question { Id = 1, Title = "Test Title" };
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempQuestion).Verifiable();
            Answer tempAnswer = null;
            mockRepo.Setup(repo => repo.GetAnswerByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempAnswer).Verifiable();

            // mock ClaimsPrincipal
            // https://stackoverflow.com/questions/38557942/mocking-iprincipal-in-asp-net-core
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "example name"),
                new Claim(ClaimTypes.NameIdentifier, "abcd"),
                new Claim("custom-claim", "example claim value"),
            }, "mock"));

            // mocking Response.StatusCode = 404 setter
            var mockHttpContext = new Mock<HttpContext>();
            var response = new Mock<HttpResponse>();
            mockHttpContext.SetupGet(x => x.Response).Returns(response.Object);

            // adding a real mapper
            var myProfile = new AskMateProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var realMapper = new Mapper(configuration);

            //creates an instance of an asp.net mvc controller
            var controller = new CommentsController(realMapper, mockRepo.Object, userManager)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            // Act
            var result = await controller.EditAnswerComment(1);

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", requestResult.ViewName);
            mockRepo.Verify(x => x.GetAnswerCommentById(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(x => x.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(x => x.GetAnswerByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task EditAnswerCommentGet_ReturnErrorViewOnException()
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            AnswerComment tempAnswerComment = new AnswerComment { Id = 1, Body = "Test Body", UserId = "abcd" };
            mockRepo.Setup(repo => repo.GetAnswerCommentById(It.IsAny<int>())).ReturnsAsync(tempAnswerComment).Verifiable();
            Question tempQuestion = new Question { Id = 1, Title = "Test Title" };
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempQuestion).Verifiable();
            Answer tempAnswer = new Answer { Id = 1, Body = "Test Body", UserId = "abcd" };
            mockRepo.Setup(repo => repo.GetAnswerByIdWithoutDetailsAsync(It.IsAny<int>())).Throws(new Exception()).Verifiable();

            // mock ClaimsPrincipal
            // https://stackoverflow.com/questions/38557942/mocking-iprincipal-in-asp-net-core
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "example name"),
                new Claim(ClaimTypes.NameIdentifier, "abcd"),
                new Claim("custom-claim", "example claim value"),
            }, "mock"));

            // mocking Response.StatusCode = 404 setter
            var mockHttpContext = new Mock<HttpContext>();
            var response = new Mock<HttpResponse>();
            mockHttpContext.SetupGet(x => x.Response).Returns(response.Object);

            // adding a real mapper
            var myProfile = new AskMateProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var realMapper = new Mapper(configuration);

            //creates an instance of an asp.net mvc controller
            var controller = new CommentsController(realMapper, mockRepo.Object, userManager)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            // Act
            var result = await controller.EditAnswerComment(1);

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", requestResult.ViewName);
            mockRepo.Verify(x => x.GetAnswerCommentById(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(x => x.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(x => x.GetAnswerByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task EditQuestionCommentPost_RedirectDetailsOnSuccess()
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            QuestionComment tempQuestionComment = new QuestionComment { Id = 1, Body = "Test Body", UserId = "abcd" };
            mockRepo.Setup(repo => repo.GetQuestionCommentById(It.IsAny<int>())).ReturnsAsync(tempQuestionComment).Verifiable();
            Question tempQuestion = new Question { Id = 1, Title = "Test Title" };
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempQuestion).Verifiable();
            mockRepo.Setup(repo => repo.EditQuestionCommentAsync(It.IsAny<QuestionComment>())).Verifiable();

            // mock ClaimsPrincipal
            // https://stackoverflow.com/questions/38557942/mocking-iprincipal-in-asp-net-core
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "example name"),
                new Claim(ClaimTypes.NameIdentifier, "abcd"),
                new Claim("custom-claim", "example claim value"),
            }, "mock"));

            // mocking Response.StatusCode = 404 setter
            var mockHttpContext = new Mock<HttpContext>();
            var response = new Mock<HttpResponse>();
            mockHttpContext.SetupGet(x => x.Response).Returns(response.Object);

            // adding a real mapper
            var myProfile = new AskMateProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var realMapper = new Mapper(configuration);

            //creates an instance of an asp.net mvc controller
            var controller = new CommentsController(realMapper, mockRepo.Object, userManager)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
            QuestionCommentViewModel questionCommentVM = new QuestionCommentViewModel() { Id = 1, QuestionId = 1, UserId = "abcd"};

            // Act
            var result = await controller.EditQuestionComment(questionCommentVM);

            // Assert
            var requestResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", requestResult.ActionName);
            Assert.Equal("Questions", requestResult.ControllerName);
            mockRepo.Verify(x => x.GetQuestionCommentById(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(x => x.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(x => x.EditQuestionCommentAsync(It.IsAny<QuestionComment>()), Times.Once);
        }

        [Fact]
        public async Task EditQuestionCommentPost_ReturnEditQuestionCommentViewOnInvalidModel()
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            QuestionComment tempQuestionComment = new QuestionComment { Id = 1, Body = "Test Body", UserId = "abcd" };
            mockRepo.Setup(repo => repo.GetQuestionCommentById(It.IsAny<int>())).ReturnsAsync(tempQuestionComment).Verifiable();
            Question tempQuestion = new Question { Id = 1, Title = "Test Title" };
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempQuestion).Verifiable();
            mockRepo.Setup(repo => repo.EditQuestionCommentAsync(It.IsAny<QuestionComment>())).Verifiable();

            // mock ClaimsPrincipal
            // https://stackoverflow.com/questions/38557942/mocking-iprincipal-in-asp-net-core
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "example name"),
                new Claim(ClaimTypes.NameIdentifier, "abcd"),
                new Claim("custom-claim", "example claim value"),
            }, "mock"));

            // mocking Response.StatusCode = 404 setter
            var mockHttpContext = new Mock<HttpContext>();
            var response = new Mock<HttpResponse>();
            mockHttpContext.SetupGet(x => x.Response).Returns(response.Object);

            // adding a real mapper
            var myProfile = new AskMateProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var realMapper = new Mapper(configuration);

            //creates an instance of an asp.net mvc controller
            var controller = new CommentsController(realMapper, mockRepo.Object, userManager)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
            QuestionCommentViewModel questionCommentVM = new QuestionCommentViewModel() { Id = 1, QuestionId = 1, UserId = "abcd" };
            controller.ModelState.AddModelError("UserId", "Required");

            // Act
            var result = await controller.EditQuestionComment(questionCommentVM);

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("EditQuestionComment", requestResult.ViewName);
            mockRepo.Verify(x => x.GetQuestionCommentById(It.IsAny<int>()), Times.Never);
            mockRepo.Verify(x => x.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Never);
            mockRepo.Verify(x => x.EditQuestionCommentAsync(It.IsAny<QuestionComment>()), Times.Never);
        }

        [Fact]
        public async Task EditQuestionCommentPost_ReturnErrorViewOnNullQuestionComment()
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            QuestionComment tempQuestionComment = null;
            mockRepo.Setup(repo => repo.GetQuestionCommentById(It.IsAny<int>())).ReturnsAsync(tempQuestionComment).Verifiable();
            Question tempQuestion = new Question { Id = 1, Title = "Test Title" };
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempQuestion).Verifiable();
            mockRepo.Setup(repo => repo.EditQuestionCommentAsync(It.IsAny<QuestionComment>())).Verifiable();

            // mock ClaimsPrincipal
            // https://stackoverflow.com/questions/38557942/mocking-iprincipal-in-asp-net-core
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "example name"),
                new Claim(ClaimTypes.NameIdentifier, "abcd"),
                new Claim("custom-claim", "example claim value"),
            }, "mock"));

            // mocking Response.StatusCode = 404 setter
            var mockHttpContext = new Mock<HttpContext>();
            var response = new Mock<HttpResponse>();
            mockHttpContext.SetupGet(x => x.Response).Returns(response.Object);

            // adding a real mapper
            var myProfile = new AskMateProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var realMapper = new Mapper(configuration);

            //creates an instance of an asp.net mvc controller
            var controller = new CommentsController(realMapper, mockRepo.Object, userManager)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
            QuestionCommentViewModel questionCommentVM = new QuestionCommentViewModel() { Id = 1, QuestionId = 1, UserId = "abcd" };

            // Act
            var result = await controller.EditQuestionComment(questionCommentVM);

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", requestResult.ViewName);
            mockRepo.Verify(x => x.GetQuestionCommentById(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(x => x.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Never);
            mockRepo.Verify(x => x.EditQuestionCommentAsync(It.IsAny<QuestionComment>()), Times.Never);
        }

        [Fact]
        public async Task EditQuestionCommentPost_ReturnErrorViewOnNullQuestion()
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            QuestionComment tempQuestionComment = new QuestionComment { Id = 1, Body = "Test Body", UserId = "abcd" };
            mockRepo.Setup(repo => repo.GetQuestionCommentById(It.IsAny<int>())).ReturnsAsync(tempQuestionComment).Verifiable();
            Question tempQuestion = null;
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempQuestion).Verifiable();
            mockRepo.Setup(repo => repo.EditQuestionCommentAsync(It.IsAny<QuestionComment>())).Verifiable();

            // mock ClaimsPrincipal
            // https://stackoverflow.com/questions/38557942/mocking-iprincipal-in-asp-net-core
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "example name"),
                new Claim(ClaimTypes.NameIdentifier, "abcd"),
                new Claim("custom-claim", "example claim value"),
            }, "mock"));

            // mocking Response.StatusCode = 404 setter
            var mockHttpContext = new Mock<HttpContext>();
            var response = new Mock<HttpResponse>();
            mockHttpContext.SetupGet(x => x.Response).Returns(response.Object);

            // adding a real mapper
            var myProfile = new AskMateProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var realMapper = new Mapper(configuration);

            //creates an instance of an asp.net mvc controller
            var controller = new CommentsController(realMapper, mockRepo.Object, userManager)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
            QuestionCommentViewModel questionCommentVM = new QuestionCommentViewModel() { Id = 1, QuestionId = 1, UserId = "abcd" };

            // Act
            var result = await controller.EditQuestionComment(questionCommentVM);

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", requestResult.ViewName);
            mockRepo.Verify(x => x.GetQuestionCommentById(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(x => x.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(x => x.EditQuestionCommentAsync(It.IsAny<QuestionComment>()), Times.Never);
        }

        [Fact]
        public async Task EditQuestionCommentPost_ReturnErrorViewOnException()
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            QuestionComment tempQuestionComment = new QuestionComment { Id = 1, Body = "Test Body", UserId = "abcd" };
            mockRepo.Setup(repo => repo.GetQuestionCommentById(It.IsAny<int>())).ReturnsAsync(tempQuestionComment).Verifiable();
            Question tempQuestion = new Question { Id = 1, Title = "Test Title" };
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempQuestion).Verifiable();
            mockRepo.Setup(repo => repo.EditQuestionCommentAsync(It.IsAny<QuestionComment>())).Throws(new Exception()).Verifiable();

            // mock ClaimsPrincipal
            // https://stackoverflow.com/questions/38557942/mocking-iprincipal-in-asp-net-core
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "example name"),
                new Claim(ClaimTypes.NameIdentifier, "abcd"),
                new Claim("custom-claim", "example claim value"),
            }, "mock"));

            // mocking Response.StatusCode = 404 setter
            var mockHttpContext = new Mock<HttpContext>();
            var response = new Mock<HttpResponse>();
            mockHttpContext.SetupGet(x => x.Response).Returns(response.Object);

            // adding a real mapper
            var myProfile = new AskMateProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var realMapper = new Mapper(configuration);

            //creates an instance of an asp.net mvc controller
            var controller = new CommentsController(realMapper, mockRepo.Object, userManager)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
            QuestionCommentViewModel questionCommentVM = new QuestionCommentViewModel() { Id = 1, QuestionId = 1, UserId = "abcd" };

            // Act
            var result = await controller.EditQuestionComment(questionCommentVM);

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", requestResult.ViewName);
            mockRepo.Verify(x => x.GetQuestionCommentById(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(x => x.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(x => x.EditQuestionCommentAsync(It.IsAny<QuestionComment>()), Times.Once);
        }

        [Fact]
        public async Task EditAnswerCommentPost_RedirectDetailsOnSuccess()
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            AnswerComment tempAnswerComment = new AnswerComment { Id = 1, Body = "Test Body", UserId = "abcd" };
            mockRepo.Setup(repo => repo.GetAnswerCommentById(It.IsAny<int>())).ReturnsAsync(tempAnswerComment).Verifiable();
            Question tempQuestion = new Question { Id = 1, Title = "Test Title" };
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempQuestion).Verifiable();
            Answer tempAnswer = new Answer { Id = 1, Body = "Test Body", UserId = "abcd" };
            mockRepo.Setup(repo => repo.GetAnswerByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempAnswer).Verifiable();
            mockRepo.Setup(repo => repo.EditAnswerCommentAsync(It.IsAny<AnswerComment>())).Verifiable();

            // mock ClaimsPrincipal
            // https://stackoverflow.com/questions/38557942/mocking-iprincipal-in-asp-net-core
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "example name"),
                new Claim(ClaimTypes.NameIdentifier, "abcd"),
                new Claim("custom-claim", "example claim value"),
            }, "mock"));

            // mocking Response.StatusCode = 404 setter
            var mockHttpContext = new Mock<HttpContext>();
            var response = new Mock<HttpResponse>();
            mockHttpContext.SetupGet(x => x.Response).Returns(response.Object);

            // adding a real mapper
            var myProfile = new AskMateProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var realMapper = new Mapper(configuration);

            //creates an instance of an asp.net mvc controller
            var controller = new CommentsController(realMapper, mockRepo.Object, userManager)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
            AnswerCommentViewModel answerCommentVM = new AnswerCommentViewModel() { Id = 1, AnswerId = 1, QuestionId = 1, UserId = "abcd" };

            // Act
            var result = await controller.EditAnswerComment(answerCommentVM);

            // Assert
            var requestResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", requestResult.ActionName);
            Assert.Equal("Questions", requestResult.ControllerName);
            mockRepo.Verify(x => x.GetAnswerCommentById(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(x => x.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(x => x.GetAnswerByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(x => x.EditAnswerCommentAsync(It.IsAny<AnswerComment>()), Times.Once);
        }

        [Fact]
        public async Task EditAnswerCommentPost_ReturnEditAnswerCommentViewOnInvalidModel()
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            AnswerComment tempAnswerComment = new AnswerComment { Id = 1, Body = "Test Body", UserId = "abcd" };
            mockRepo.Setup(repo => repo.GetAnswerCommentById(It.IsAny<int>())).ReturnsAsync(tempAnswerComment).Verifiable();
            Question tempQuestion = new Question { Id = 1, Title = "Test Title" };
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempQuestion).Verifiable();
            Answer tempAnswer = new Answer { Id = 1, Body = "Test Body", UserId = "abcd" };
            mockRepo.Setup(repo => repo.GetAnswerByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempAnswer).Verifiable();
            mockRepo.Setup(repo => repo.EditAnswerCommentAsync(It.IsAny<AnswerComment>())).Verifiable();

            // mock ClaimsPrincipal
            // https://stackoverflow.com/questions/38557942/mocking-iprincipal-in-asp-net-core
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "example name"),
                new Claim(ClaimTypes.NameIdentifier, "abcd"),
                new Claim("custom-claim", "example claim value"),
            }, "mock"));

            // mocking Response.StatusCode = 404 setter
            var mockHttpContext = new Mock<HttpContext>();
            var response = new Mock<HttpResponse>();
            mockHttpContext.SetupGet(x => x.Response).Returns(response.Object);

            // adding a real mapper
            var myProfile = new AskMateProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var realMapper = new Mapper(configuration);

            //creates an instance of an asp.net mvc controller
            var controller = new CommentsController(realMapper, mockRepo.Object, userManager)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
            AnswerCommentViewModel answerCommentVM = new AnswerCommentViewModel() { Id = 1, AnswerId = 1, QuestionId = 1, UserId = "abcd" };
            controller.ModelState.AddModelError("UserId", "Required");

            // Act
            var result = await controller.EditAnswerComment(answerCommentVM);

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("EditAnswerComment", requestResult.ViewName);
            mockRepo.Verify(x => x.GetAnswerCommentById(It.IsAny<int>()), Times.Never);
            mockRepo.Verify(x => x.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Never);
            mockRepo.Verify(x => x.GetAnswerByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Never);
            mockRepo.Verify(x => x.EditAnswerCommentAsync(It.IsAny<AnswerComment>()), Times.Never);
        }

        [Fact]
        public async Task EditAnswerCommentPost_ReturnErrorViewOnNullAnswerComment()
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            AnswerComment tempAnswerComment = null;
            mockRepo.Setup(repo => repo.GetAnswerCommentById(It.IsAny<int>())).ReturnsAsync(tempAnswerComment).Verifiable();
            Question tempQuestion = new Question { Id = 1, Title = "Test Title" };
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempQuestion).Verifiable();
            Answer tempAnswer = new Answer { Id = 1, Body = "Test Body", UserId = "abcd" };
            mockRepo.Setup(repo => repo.GetAnswerByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempAnswer).Verifiable();
            mockRepo.Setup(repo => repo.EditAnswerCommentAsync(It.IsAny<AnswerComment>())).Verifiable();

            // mock ClaimsPrincipal
            // https://stackoverflow.com/questions/38557942/mocking-iprincipal-in-asp-net-core
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "example name"),
                new Claim(ClaimTypes.NameIdentifier, "abcd"),
                new Claim("custom-claim", "example claim value"),
            }, "mock"));

            // mocking Response.StatusCode = 404 setter
            var mockHttpContext = new Mock<HttpContext>();
            var response = new Mock<HttpResponse>();
            mockHttpContext.SetupGet(x => x.Response).Returns(response.Object);

            // adding a real mapper
            var myProfile = new AskMateProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var realMapper = new Mapper(configuration);

            //creates an instance of an asp.net mvc controller
            var controller = new CommentsController(realMapper, mockRepo.Object, userManager)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
            AnswerCommentViewModel answerCommentVM = new AnswerCommentViewModel() { Id = 1, AnswerId = 1, QuestionId = 1, UserId = "abcd" };

            // Act
            var result = await controller.EditAnswerComment(answerCommentVM);

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", requestResult.ViewName);
            mockRepo.Verify(x => x.GetAnswerCommentById(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(x => x.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Never);
            mockRepo.Verify(x => x.GetAnswerByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Never);
            mockRepo.Verify(x => x.EditAnswerCommentAsync(It.IsAny<AnswerComment>()), Times.Never);
        }

        [Fact]
        public async Task EditAnswerCommentPost_ReturnErrorViewOnNullQuestion()
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            AnswerComment tempAnswerComment = new AnswerComment { Id = 1, Body = "Test Body", UserId = "abcd" };
            mockRepo.Setup(repo => repo.GetAnswerCommentById(It.IsAny<int>())).ReturnsAsync(tempAnswerComment).Verifiable();
            Question tempQuestion = null;
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempQuestion).Verifiable();
            Answer tempAnswer = new Answer { Id = 1, Body = "Test Body", UserId = "abcd" };
            mockRepo.Setup(repo => repo.GetAnswerByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempAnswer).Verifiable();
            mockRepo.Setup(repo => repo.EditAnswerCommentAsync(It.IsAny<AnswerComment>())).Verifiable();

            // mock ClaimsPrincipal
            // https://stackoverflow.com/questions/38557942/mocking-iprincipal-in-asp-net-core
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "example name"),
                new Claim(ClaimTypes.NameIdentifier, "abcd"),
                new Claim("custom-claim", "example claim value"),
            }, "mock"));

            // mocking Response.StatusCode = 404 setter
            var mockHttpContext = new Mock<HttpContext>();
            var response = new Mock<HttpResponse>();
            mockHttpContext.SetupGet(x => x.Response).Returns(response.Object);

            // adding a real mapper
            var myProfile = new AskMateProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var realMapper = new Mapper(configuration);

            //creates an instance of an asp.net mvc controller
            var controller = new CommentsController(realMapper, mockRepo.Object, userManager)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
            AnswerCommentViewModel answerCommentVM = new AnswerCommentViewModel() { Id = 1, AnswerId = 1, QuestionId = 1, UserId = "abcd" };

            // Act
            var result = await controller.EditAnswerComment(answerCommentVM);

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", requestResult.ViewName);
            mockRepo.Verify(x => x.GetAnswerCommentById(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(x => x.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(x => x.GetAnswerByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Never);
            mockRepo.Verify(x => x.EditAnswerCommentAsync(It.IsAny<AnswerComment>()), Times.Never);
        }

        [Fact]
        public async Task EditAnswerCommentPost_ReturnErrorViewOnNullAnswer()
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            AnswerComment tempAnswerComment = new AnswerComment { Id = 1, Body = "Test Body", UserId = "abcd" };
            mockRepo.Setup(repo => repo.GetAnswerCommentById(It.IsAny<int>())).ReturnsAsync(tempAnswerComment).Verifiable();
            Question tempQuestion = new Question { Id = 1, Title = "Test Title" };
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempQuestion).Verifiable();
            Answer tempAnswer = null;
            mockRepo.Setup(repo => repo.GetAnswerByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempAnswer).Verifiable();
            mockRepo.Setup(repo => repo.EditAnswerCommentAsync(It.IsAny<AnswerComment>())).Verifiable();

            // mock ClaimsPrincipal
            // https://stackoverflow.com/questions/38557942/mocking-iprincipal-in-asp-net-core
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "example name"),
                new Claim(ClaimTypes.NameIdentifier, "abcd"),
                new Claim("custom-claim", "example claim value"),
            }, "mock"));

            // mocking Response.StatusCode = 404 setter
            var mockHttpContext = new Mock<HttpContext>();
            var response = new Mock<HttpResponse>();
            mockHttpContext.SetupGet(x => x.Response).Returns(response.Object);

            // adding a real mapper
            var myProfile = new AskMateProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var realMapper = new Mapper(configuration);

            //creates an instance of an asp.net mvc controller
            var controller = new CommentsController(realMapper, mockRepo.Object, userManager)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
            AnswerCommentViewModel answerCommentVM = new AnswerCommentViewModel() { Id = 1, AnswerId = 1, QuestionId = 1, UserId = "abcd" };

            // Act
            var result = await controller.EditAnswerComment(answerCommentVM);

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", requestResult.ViewName);
            mockRepo.Verify(x => x.GetAnswerCommentById(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(x => x.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(x => x.GetAnswerByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(x => x.EditAnswerCommentAsync(It.IsAny<AnswerComment>()), Times.Never);
        }

        [Fact]
        public async Task EditAnswerCommentPost_ReturnErrorViewOnException()
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            AnswerComment tempAnswerComment = new AnswerComment { Id = 1, Body = "Test Body", UserId = "abcd" };
            mockRepo.Setup(repo => repo.GetAnswerCommentById(It.IsAny<int>())).ReturnsAsync(tempAnswerComment).Verifiable();
            Question tempQuestion = new Question { Id = 1, Title = "Test Title" };
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempQuestion).Verifiable();
            Answer tempAnswer = new Answer { Id = 1, Body = "Test Body", UserId = "abcd" };
            mockRepo.Setup(repo => repo.GetAnswerByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempAnswer).Verifiable();
            mockRepo.Setup(repo => repo.EditAnswerCommentAsync(It.IsAny<AnswerComment>())).Throws(new Exception()).Verifiable();

            // mock ClaimsPrincipal
            // https://stackoverflow.com/questions/38557942/mocking-iprincipal-in-asp-net-core
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "example name"),
                new Claim(ClaimTypes.NameIdentifier, "abcd"),
                new Claim("custom-claim", "example claim value"),
            }, "mock"));

            // mocking Response.StatusCode = 404 setter
            var mockHttpContext = new Mock<HttpContext>();
            var response = new Mock<HttpResponse>();
            mockHttpContext.SetupGet(x => x.Response).Returns(response.Object);

            // adding a real mapper
            var myProfile = new AskMateProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var realMapper = new Mapper(configuration);

            //creates an instance of an asp.net mvc controller
            var controller = new CommentsController(realMapper, mockRepo.Object, userManager)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
            AnswerCommentViewModel answerCommentVM = new AnswerCommentViewModel() { Id = 1, AnswerId = 1, QuestionId = 1, UserId = "abcd" };

            // Act
            var result = await controller.EditAnswerComment(answerCommentVM);

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", requestResult.ViewName);
            mockRepo.Verify(x => x.GetAnswerCommentById(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(x => x.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(x => x.GetAnswerByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(x => x.EditAnswerCommentAsync(It.IsAny<AnswerComment>()), Times.Once);
        }

        [Fact]
        public async Task RemoveQuestionComment_RedirectDetailsOnSuccess()
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            QuestionComment tempQuestionComment = new QuestionComment { Id = 1, Body = "Test Body", UserId = "abcd" };
            mockRepo.Setup(repo => repo.GetQuestionCommentById(It.IsAny<int>())).ReturnsAsync(tempQuestionComment).Verifiable();
            Question tempQuestion = new Question { Id = 1, Title = "Test Title" };
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempQuestion).Verifiable();
            mockRepo.Setup(repo => repo.RemoveQuestionCommentById(It.IsAny<int>())).Verifiable();

            // mock ClaimsPrincipal
            // https://stackoverflow.com/questions/38557942/mocking-iprincipal-in-asp-net-core
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "example name"),
                new Claim(ClaimTypes.NameIdentifier, "abcd"),
                new Claim("custom-claim", "example claim value"),
            }, "mock"));

            // mocking Response.StatusCode = 404 setter
            var mockHttpContext = new Mock<HttpContext>();
            var response = new Mock<HttpResponse>();
            mockHttpContext.SetupGet(x => x.Response).Returns(response.Object);

            // adding a real mapper
            var myProfile = new AskMateProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var realMapper = new Mapper(configuration);

            //creates an instance of an asp.net mvc controller
            var controller = new CommentsController(realMapper, mockRepo.Object, userManager)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
            
            // Act
            var result = await controller.RemoveQuestionComment(1, 1);

            // Assert
            var requestResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", requestResult.ActionName);
            Assert.Equal("Questions", requestResult.ControllerName);
            mockRepo.Verify(x => x.GetQuestionCommentById(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(x => x.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(x => x.RemoveQuestionCommentById(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task RemoveQuestionComment_ReturnErrorViewOnNullQuestionComment()
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            QuestionComment tempQuestionComment = null;
            mockRepo.Setup(repo => repo.GetQuestionCommentById(It.IsAny<int>())).ReturnsAsync(tempQuestionComment).Verifiable();
            Question tempQuestion = new Question { Id = 1, Title = "Test Title" };
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempQuestion).Verifiable();
            mockRepo.Setup(repo => repo.RemoveQuestionCommentById(It.IsAny<int>())).Verifiable();

            // mock ClaimsPrincipal
            // https://stackoverflow.com/questions/38557942/mocking-iprincipal-in-asp-net-core
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "example name"),
                new Claim(ClaimTypes.NameIdentifier, "abcd"),
                new Claim("custom-claim", "example claim value"),
            }, "mock"));

            // mocking Response.StatusCode = 404 setter
            var mockHttpContext = new Mock<HttpContext>();
            var response = new Mock<HttpResponse>();
            mockHttpContext.SetupGet(x => x.Response).Returns(response.Object);

            // adding a real mapper
            var myProfile = new AskMateProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var realMapper = new Mapper(configuration);

            //creates an instance of an asp.net mvc controller
            var controller = new CommentsController(realMapper, mockRepo.Object, userManager)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            // Act
            var result = await controller.RemoveQuestionComment(1, 1);

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", requestResult.ViewName);
            mockRepo.Verify(x => x.GetQuestionCommentById(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(x => x.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Never);
            mockRepo.Verify(x => x.RemoveQuestionCommentById(It.IsAny<int>()), Times.Never);
        }






    }
}
