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
using System.IO;
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
    public class AnswersControllerShould
    {
        private UserManager<ApplicationUser> userManager { get; }
        private IWebHostEnvironment webHostEnvironment { get; }
        private IFileOperations fileOperations { get; }
        private IAsyncRepository repository { get; }
        private IMapper mapper { get; }

        [Fact]
        public async Task AddAnswerGet_ReturnViewAddAnswerOnSuccess()
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
            var controller = new AnswersController(mockRepo.Object, mapper, webHostEnvironment, fileOperations, userManager)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };

            // Act
            var result = await controller.AddAnswer(1);

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("AddAnswer", requestResult.ViewName);
            mockRepo.Verify(x => x.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task AddAnswerGet_ReturnViewErrorOnNullQuestion()
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
            var controller = new AnswersController(mockRepo.Object, mapper, webHostEnvironment, fileOperations, userManager)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };

            // Act
            var result = await controller.AddAnswer(1);

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", requestResult.ViewName);
            mockRepo.Verify(x => x.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task AddAnswerGet_ReturnViewErrorOnException()
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
            var controller = new AnswersController(mockRepo.Object, mapper, webHostEnvironment, fileOperations, userManager)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };

            // Act
            var result = await controller.AddAnswer(1);

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", requestResult.ViewName);
            mockRepo.Verify(x => x.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task AddAnswerPost_RedirectToDetailsOnSuccess() // NOT TESTING WITH IMAGE - too advanced for me at this time
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            Question tempQuestion = new Question { Id = 1, Title = "Test Title" };
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempQuestion).Verifiable();
            mockRepo.Setup(repo => repo.AddAnswerAsync(It.IsAny<Answer>())).ReturnsAsync(new Answer()).Verifiable();

            // mocking UserManager            
            var mockUserManager = MockHelpers.MockUserManager<ApplicationUser>();
            ApplicationUser tempUser = new ApplicationUser { Id = "abcd", Email = "test@email.com" };
            mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(tempUser).Verifiable();

            // mock fileOperations
            var mockFileOperations = new Mock<IFileOperations>();
            mockFileOperations.Setup(fo => fo.ValidateImageType(It.IsAny<string>())).Returns(false).Verifiable();

            //// controller mock for SetPathAndUpload(answerViewModel)
            //var mockController = new Mock<AnswersController>();
            //mockController.Setup(c => c.SetPathAndUpload(It.IsAny<AnswerViewModel>())).ReturnsAsync("TestUniqueFileName").Verifiable();

            // adding a real mapper
            var myProfile = new AskMateProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var realMapper = new Mapper(configuration);

            // mocking Response.StatusCode = 404 setter
            var mockHttpContext = new Mock<HttpContext>();
            var response = new Mock<HttpResponse>();
            mockHttpContext.SetupGet(x => x.Response).Returns(response.Object);

            //creates an instance of an asp.net mvc controller
            var controller = new AnswersController(mockRepo.Object, realMapper, webHostEnvironment, mockFileOperations.Object, mockUserManager.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };
            FormFile tempFormFile = new FormFile(new MemoryStream(), 123, 123, "test", "test.xyz");
            AnswerViewModel tempAnswerViewModel = new AnswerViewModel { QuestionId = 1, UserId = "abcd", Image = tempFormFile };

            // Act
            var result = await controller.AddAnswer(tempAnswerViewModel);

            // Assert
            var requestResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", requestResult.ActionName);
            Assert.Equal("Questions", requestResult.ControllerName);
            mockRepo.Verify(r => r.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(r => r.AddAnswerAsync(It.IsAny<Answer>()), Times.Once);
            mockUserManager.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
            mockFileOperations.Verify(fo => fo.ValidateImageType(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task AddAnswerPost_AddAnswerOnModelInvalid() // NOT TESTING WITH IMAGE - too advanced for me at this time
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            Question tempQuestion = new Question { Id = 1, Title = "Test Title" };
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempQuestion).Verifiable();
            mockRepo.Setup(repo => repo.AddAnswerAsync(It.IsAny<Answer>())).ReturnsAsync(new Answer()).Verifiable();

            // mocking UserManager            
            var mockUserManager = MockHelpers.MockUserManager<ApplicationUser>();
            ApplicationUser tempUser = new ApplicationUser { Id = "abcd", Email = "test@email.com" };
            mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(tempUser).Verifiable();

            // mock fileOperations
            var mockFileOperations = new Mock<IFileOperations>();
            mockFileOperations.Setup(fo => fo.ValidateImageType(It.IsAny<string>())).Returns(false).Verifiable();

            //// controller mock for SetPathAndUpload(answerViewModel)
            //var mockController = new Mock<AnswersController>();
            //mockController.Setup(c => c.SetPathAndUpload(It.IsAny<AnswerViewModel>())).ReturnsAsync("TestUniqueFileName").Verifiable();

            // adding a real mapper
            var myProfile = new AskMateProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var realMapper = new Mapper(configuration);

            // mocking Response.StatusCode = 404 setter
            var mockHttpContext = new Mock<HttpContext>();
            var response = new Mock<HttpResponse>();
            mockHttpContext.SetupGet(x => x.Response).Returns(response.Object);

            //creates an instance of an asp.net mvc controller
            var controller = new AnswersController(mockRepo.Object, realMapper, webHostEnvironment, mockFileOperations.Object, mockUserManager.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };
            FormFile tempFormFile = new FormFile(new MemoryStream(), 123, 123, "test", "test.xyz");
            AnswerViewModel tempAnswerViewModel = new AnswerViewModel { QuestionId = 1, UserId = "abcd", Image = tempFormFile };
            controller.ModelState.AddModelError("QuestionId", "Required");

            // Act
            var result = await controller.AddAnswer(tempAnswerViewModel);

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("AddAnswer", requestResult.ViewName);
            mockRepo.Verify(r => r.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Never);
            mockRepo.Verify(r => r.AddAnswerAsync(It.IsAny<Answer>()), Times.Never);
            mockUserManager.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Never);
            mockFileOperations.Verify(fo => fo.ValidateImageType(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task AddAnswerPost_ErrorViewOnException() // NOT TESTING WITH IMAGE - too advanced for me at this time
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            Question tempQuestion = new Question { Id = 1, Title = "Test Title" };
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempQuestion).Verifiable();
            mockRepo.Setup(repo => repo.AddAnswerAsync(It.IsAny<Answer>())).Throws(new Exception()).Verifiable();

            // mocking UserManager            
            var mockUserManager = MockHelpers.MockUserManager<ApplicationUser>();
            ApplicationUser tempUser = new ApplicationUser { Id = "abcd", Email = "test@email.com" };
            mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(tempUser).Verifiable();

            // mock fileOperations
            var mockFileOperations = new Mock<IFileOperations>();
            mockFileOperations.Setup(fo => fo.ValidateImageType(It.IsAny<string>())).Returns(false).Verifiable();

            //// controller mock for SetPathAndUpload(answerViewModel)
            //var mockController = new Mock<AnswersController>();
            //mockController.Setup(c => c.SetPathAndUpload(It.IsAny<AnswerViewModel>())).ReturnsAsync("TestUniqueFileName").Verifiable();

            // adding a real mapper
            var myProfile = new AskMateProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var realMapper = new Mapper(configuration);

            // mocking Response.StatusCode = 404 setter
            var mockHttpContext = new Mock<HttpContext>();
            var response = new Mock<HttpResponse>();
            mockHttpContext.SetupGet(x => x.Response).Returns(response.Object);

            //creates an instance of an asp.net mvc controller
            var controller = new AnswersController(mockRepo.Object, realMapper, webHostEnvironment, mockFileOperations.Object, mockUserManager.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };
            FormFile tempFormFile = new FormFile(new MemoryStream(), 123, 123, "test", "test.xyz");
            AnswerViewModel tempAnswerViewModel = new AnswerViewModel { QuestionId = 1, UserId = "abcd", Image = tempFormFile };
            
            // Act
            var result = await controller.AddAnswer(tempAnswerViewModel);

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", requestResult.ViewName);
            mockRepo.Verify(r => r.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(r => r.AddAnswerAsync(It.IsAny<Answer>()), Times.Once);
            mockUserManager.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
            mockFileOperations.Verify(fo => fo.ValidateImageType(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task EditAnswerGet_ReturnViewAddAnswerOnSuccess()
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            Answer tempAnswer = new Answer { Id = 1, Body = "Test Body", UserId = "abcd" };
            mockRepo.Setup(repo => repo.GetAnswerByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempAnswer).Verifiable();
            Question tempQuestion = new Question { Id = 1, Title = "Test Title" };
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempQuestion).Verifiable();

            // adding a real mapper
            var myProfile = new AskMateProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var realMapper = new Mapper(configuration);

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

            //creates an instance of an asp.net mvc controller
            var controller = new AnswersController(mockRepo.Object, realMapper, webHostEnvironment, fileOperations, userManager)
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
            var result = await controller.EditAnswer(1);

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("EditAnswer", requestResult.ViewName);
            mockRepo.Verify(x => x.GetAnswerByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(x => x.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task EditAnswerGet_ReturnViewErrorOnNullAnswer()
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            Answer tempAnswer = null;
            mockRepo.Setup(repo => repo.GetAnswerByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempAnswer).Verifiable();
            Question tempQuestion = new Question { Id = 1, Title = "Test Title" };
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempQuestion).Verifiable();

            // adding a real mapper
            var myProfile = new AskMateProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var realMapper = new Mapper(configuration);

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

            //creates an instance of an asp.net mvc controller
            var controller = new AnswersController(mockRepo.Object, realMapper, webHostEnvironment, fileOperations, userManager)
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
            var result = await controller.EditAnswer(1);

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", requestResult.ViewName);
            mockRepo.Verify(x => x.GetAnswerByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(x => x.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task EditAnswerGet_ReturnErrorOnNullQuestion()
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            Answer tempAnswer = new Answer { Id = 1, Body = "Test Body", UserId = "abcd" };
            mockRepo.Setup(repo => repo.GetAnswerByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempAnswer).Verifiable();
            Question tempQuestion = null;
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempQuestion).Verifiable();

            // adding a real mapper
            var myProfile = new AskMateProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var realMapper = new Mapper(configuration);

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

            //creates an instance of an asp.net mvc controller
            var controller = new AnswersController(mockRepo.Object, realMapper, webHostEnvironment, fileOperations, userManager)
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
            var result = await controller.EditAnswer(1);

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", requestResult.ViewName);
            mockRepo.Verify(x => x.GetAnswerByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(x => x.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task EditAnswerGet_ReturnViewErrorOnException()
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            Answer tempAnswer = new Answer { Id = 1, Body = "Test Body", UserId = "abcd" };
            mockRepo.Setup(repo => repo.GetAnswerByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempAnswer).Verifiable();
            Question tempQuestion = new Question { Id = 1, Title = "Test Title" };
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>())).Throws(new Exception()).Verifiable();

            // adding a real mapper
            var myProfile = new AskMateProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var realMapper = new Mapper(configuration);

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

            //creates an instance of an asp.net mvc controller
            var controller = new AnswersController(mockRepo.Object, realMapper, webHostEnvironment, fileOperations, userManager)
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
            var result = await controller.EditAnswer(1);

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", requestResult.ViewName);
            mockRepo.Verify(x => x.GetAnswerByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(x => x.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task EditAnswerPost_RedirectToActionDetailsOnSuccess() // WITHOUT IMAGE - too advanced for me at this time
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            Answer tempAnswer = new Answer { Id = 1, Body = "Test Body", UserId = "abcd" };
            mockRepo.Setup(repo => repo.GetAnswerByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempAnswer).Verifiable();
            Question tempQuestion = new Question { Id = 1, Title = "Test Title" };
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempQuestion).Verifiable();
            mockRepo.Setup(repo => repo.EditAnswerAsync(It.IsAny<Answer>())).Verifiable();

            // mocking UserManager
            var mockUserManager = MockHelpers.MockUserManager<ApplicationUser>();
            var userFromDb = new ApplicationUser { Email = "test@email.com" };
            mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(userFromDb);

            // adding a real mapper
            var myProfile = new AskMateProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var realMapper = new Mapper(configuration);

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

            //creates an instance of an asp.net mvc controller
            var controller = new AnswersController(mockRepo.Object, realMapper, webHostEnvironment, fileOperations, mockUserManager.Object)
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
            var result = await controller.EditAnswer(new AnswerViewModel { Id = 1, Body = "Test Body", UserId = "abcd" });

            // Assert
            var requestResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", requestResult.ActionName);
            Assert.Equal("Questions", requestResult.ControllerName);
            mockRepo.Verify(mr => mr.GetAnswerByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(mr => mr.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(mr => mr.EditAnswerAsync(It.IsAny<Answer>()), Times.Once);
            mockUserManager.Verify(mu => mu.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
        }

        [Fact]
        public async Task EditAnswerPost_EditAnswerViewOnModelInvalid() // WITHOUT IMAGE - too advanced for me at this time
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            Answer tempAnswer = new Answer { Id = 1, Body = "Test Body", UserId = "abcd" };
            mockRepo.Setup(repo => repo.GetAnswerByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempAnswer).Verifiable();
            Question tempQuestion = new Question { Id = 1, Title = "Test Title" };
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempQuestion).Verifiable();
            mockRepo.Setup(repo => repo.EditAnswerAsync(It.IsAny<Answer>())).Verifiable();

            // mocking UserManager
            var mockUserManager = MockHelpers.MockUserManager<ApplicationUser>();
            var userFromDb = new ApplicationUser { Email = "test@email.com" };
            mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(userFromDb);

            // adding a real mapper
            var myProfile = new AskMateProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var realMapper = new Mapper(configuration);

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

            //creates an instance of an asp.net mvc controller
            var controller = new AnswersController(mockRepo.Object, realMapper, webHostEnvironment, fileOperations, mockUserManager.Object)
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
            controller.ModelState.AddModelError("UserId", "Required");

            // Act
            var result = await controller.EditAnswer(new AnswerViewModel { Id = 1, Body = "Test Body", UserId = "abcd" });

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("EditAnswer", requestResult.ViewName);
            mockRepo.Verify(mr => mr.GetAnswerByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Never);
            mockRepo.Verify(mr => mr.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Never);
            mockRepo.Verify(mr => mr.EditAnswerAsync(It.IsAny<Answer>()), Times.Never);
            mockUserManager.Verify(mu => mu.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Never);
        }

        [Fact]
        public async Task EditAnswerPost_ErrorViewOnNullAnswer() // WITHOUT IMAGE - too advanced for me at this time
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            Answer tempAnswer = null;
            mockRepo.Setup(repo => repo.GetAnswerByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempAnswer).Verifiable();
            Question tempQuestion = new Question { Id = 1, Title = "Test Title" };
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempQuestion).Verifiable();
            mockRepo.Setup(repo => repo.EditAnswerAsync(It.IsAny<Answer>())).Verifiable();

            // mocking UserManager
            var mockUserManager = MockHelpers.MockUserManager<ApplicationUser>();
            var userFromDb = new ApplicationUser { Email = "test@email.com" };
            mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(userFromDb);

            // adding a real mapper
            var myProfile = new AskMateProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var realMapper = new Mapper(configuration);

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

            //creates an instance of an asp.net mvc controller
            var controller = new AnswersController(mockRepo.Object, realMapper, webHostEnvironment, fileOperations, mockUserManager.Object)
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
            var result = await controller.EditAnswer(new AnswerViewModel { Id = 1, Body = "Test Body", UserId = "abcd" });

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", requestResult.ViewName);
            mockRepo.Verify(mr => mr.GetAnswerByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(mr => mr.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Never);
            mockRepo.Verify(mr => mr.EditAnswerAsync(It.IsAny<Answer>()), Times.Never);
            mockUserManager.Verify(mu => mu.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Never);
        }

        [Fact]
        public async Task EditAnswerPost_ErrorViewOnNullQuestion() // WITHOUT IMAGE - too advanced for me at this time
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            Answer tempAnswer = new Answer { Id = 1, Body = "Test Body", UserId = "abcd" };
            mockRepo.Setup(repo => repo.GetAnswerByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempAnswer).Verifiable();
            Question tempQuestion = null;
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempQuestion).Verifiable();
            mockRepo.Setup(repo => repo.EditAnswerAsync(It.IsAny<Answer>())).Verifiable();

            // mocking UserManager
            var mockUserManager = MockHelpers.MockUserManager<ApplicationUser>();
            var userFromDb = new ApplicationUser { Email = "test@email.com" };
            mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(userFromDb);

            // adding a real mapper
            var myProfile = new AskMateProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var realMapper = new Mapper(configuration);

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

            //creates an instance of an asp.net mvc controller
            var controller = new AnswersController(mockRepo.Object, realMapper, webHostEnvironment, fileOperations, mockUserManager.Object)
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
            var result = await controller.EditAnswer(new AnswerViewModel { Id = 1, Body = "Test Body", UserId = "abcd" });

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", requestResult.ViewName);
            mockRepo.Verify(mr => mr.GetAnswerByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(mr => mr.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(mr => mr.EditAnswerAsync(It.IsAny<Answer>()), Times.Never);
            mockUserManager.Verify(mu => mu.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Never);
        }

        [Fact]
        public async Task EditAnswerPost_ErrorViewOnException() // WITHOUT IMAGE - too advanced for me at this time
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            Answer tempAnswer = new Answer { Id = 1, Body = "Test Body", UserId = "abcd" };
            mockRepo.Setup(repo => repo.GetAnswerByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempAnswer).Verifiable();
            Question tempQuestion = new Question { Id = 1, Title = "Test Title" };
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempQuestion).Verifiable();
            mockRepo.Setup(repo => repo.EditAnswerAsync(It.IsAny<Answer>())).Throws(new Exception()).Verifiable();

            // mocking UserManager
            var mockUserManager = MockHelpers.MockUserManager<ApplicationUser>();
            var userFromDb = new ApplicationUser { Email = "test@email.com" };
            mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(userFromDb);

            // adding a real mapper
            var myProfile = new AskMateProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var realMapper = new Mapper(configuration);

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

            //creates an instance of an asp.net mvc controller
            var controller = new AnswersController(mockRepo.Object, realMapper, webHostEnvironment, fileOperations, mockUserManager.Object)
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
            var result = await controller.EditAnswer(new AnswerViewModel { Id = 1, Body = "Test Body", UserId = "abcd" });

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", requestResult.ViewName);
            mockRepo.Verify(mr => mr.GetAnswerByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(mr => mr.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(mr => mr.EditAnswerAsync(It.IsAny<Answer>()), Times.Once);
            mockUserManager.Verify(mu => mu.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
        }

        [Fact]
        public async Task RemoveGet_RedirectToDetailsOnSuccess()
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            Answer tempAnswer = new Answer { Id = 1, Body = "Test Body", UserId = "abcd" };
            mockRepo.Setup(repo => repo.GetAnswerByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempAnswer).Verifiable();
            Question tempQuestion = new Question { Id = 1, Title = "Test Title" };
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempQuestion).Verifiable();
            mockRepo.Setup(repo => repo.RemoveAnswerById(It.IsAny<int>())).Verifiable();

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

            //creates an instance of an asp.net mvc controller
            var controller = new AnswersController(mockRepo.Object, mapper, webHostEnvironment, fileOperations, userManager)
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
            var result = await controller.Remove(1, 1);

            // Assert
            var requestResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", requestResult.ActionName);
            Assert.Equal("Questions", requestResult.ControllerName);
            mockRepo.Verify(mr => mr.GetAnswerByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(mr => mr.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(mr => mr.RemoveAnswerById(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task RemoveGet_ErrorViewOnNullAnswer()
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            Answer tempAnswer = null;
            mockRepo.Setup(repo => repo.GetAnswerByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempAnswer).Verifiable();
            Question tempQuestion = new Question { Id = 1, Title = "Test Title" };
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempQuestion).Verifiable();
            mockRepo.Setup(repo => repo.RemoveAnswerById(It.IsAny<int>())).Verifiable();

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

            //creates an instance of an asp.net mvc controller
            var controller = new AnswersController(mockRepo.Object, mapper, webHostEnvironment, fileOperations, userManager)
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
            var result = await controller.Remove(1, 1);

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", requestResult.ViewName);
            mockRepo.Verify(mr => mr.GetAnswerByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(mr => mr.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Never);
            mockRepo.Verify(mr => mr.RemoveAnswerById(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task RemoveGet_ErrorViewOnNullQuestion()
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            Answer tempAnswer = new Answer { Id = 1, Body = "Test Body", UserId = "abcd" };
            mockRepo.Setup(repo => repo.GetAnswerByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempAnswer).Verifiable();
            Question tempQuestion = null;
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempQuestion).Verifiable();
            mockRepo.Setup(repo => repo.RemoveAnswerById(It.IsAny<int>())).Verifiable();

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

            //creates an instance of an asp.net mvc controller
            var controller = new AnswersController(mockRepo.Object, mapper, webHostEnvironment, fileOperations, userManager)
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
            var result = await controller.Remove(1, 1);

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", requestResult.ViewName);
            mockRepo.Verify(mr => mr.GetAnswerByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(mr => mr.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(mr => mr.RemoveAnswerById(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task RemoveImageGet_RedirectToDetailsOnSuccess()
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            Answer tempAnswer = new Answer { Id = 1, Body = "Test Body", UserId = "abcd" };
            mockRepo.Setup(repo => repo.GetAnswerByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempAnswer).Verifiable();
            Question tempQuestion = new Question { Id = 1, Title = "Test Title" };
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempQuestion).Verifiable();
            mockRepo.Setup(repo => repo.RemoveAnswerImageByAnswerId(It.IsAny<int>())).Verifiable();

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

            //creates an instance of an asp.net mvc controller
            var controller = new AnswersController(mockRepo.Object, mapper, webHostEnvironment, fileOperations, userManager)
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
            var result = await controller.RemoveImage(1, 1);

            // Assert
            var requestResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", requestResult.ActionName);
            Assert.Equal("Questions", requestResult.ControllerName);
            mockRepo.Verify(mr => mr.GetAnswerByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(mr => mr.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(mr => mr.RemoveAnswerImageByAnswerId(It.IsAny<int>()), Times.Once);
        }






        //[Fact]
        //public async Task AddAnswerGet_ReturnAViewResult()
        //{
        //    // Arrange
        //    var mockRepo = new Mock<IAsyncRepository>();
        //    mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(1))
        //        .ReturnsAsync(GetQuestionWithoutDetails());
        //    var controller = new AnswersController(mockRepo.Object, mapper, webHostEnvironment, fileOperations, userManager);

        //    // Act
        //    var result = await controller.AddAnswer(1);

        //    // Assert
        //    var viewResult = Assert.IsType<ViewResult>(result);
        //    Assert.NotEqual("Error", viewResult.ViewName);
        //}

        //[Fact]
        //public async Task AddAnswerPost_ReturnErrorView()
        //{
        //    // Arrange - question exists, model is valid, processing fails
        //    var mockRepo = new Mock<IAsyncRepository>();
        //    mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(1))
        //        .ReturnsAsync(GetQuestionWithoutDetails());
        //    var controller = new AnswersController(mockRepo.Object, mapper, webHostEnvironment, fileOperations, userManager);
        //    AnswerViewModel answerViewModel = new AnswerViewModel
        //    {
        //        Id = 1,
        //        QuestionId = 1,
        //        Body = "Test Body"
        //    };

        //    // Act
        //    var result = await controller.AddAnswer(answerViewModel);

        //    // Assert
        //    var viewResult = Assert.IsType<ViewResult>(result);
        //    Assert.Equal("Error", viewResult.ViewName);
        //}

        //[Fact]
        //public async Task AddAnswerPost_ReturnViewModelInvalid()
        //{
        //    // Arrange - question exists, model is invalid, processing doesn't start
        //    var mockRepo = new Mock<IAsyncRepository>();
        //    mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(1))
        //        .ReturnsAsync(GetQuestionWithoutDetails());
        //    var controller = new AnswersController(mockRepo.Object, mapper, webHostEnvironment, fileOperations, userManager);
        //    AnswerViewModel answerViewModel = new AnswerViewModel
        //    {
        //        Id = 1,
        //        QuestionId = 1,
        //        Body = ""
        //    };
        //    controller.ModelState.AddModelError("Body", "Required");

        //    // Act
        //    var result = await controller.AddAnswer(answerViewModel);

        //    // Assert
        //    var viewResult = Assert.IsType<ViewResult>(result);
        //    Assert.NotEqual("Error", viewResult.ViewName);
        //}

        //[Fact]
        //public async Task AddAnswerPost_ReturnErrorQuestionInvalid()
        //{
        //    // Arrange - question doesn't exist, model is valid, processing doesn't start
        //    var mockRepo = new Mock<IAsyncRepository>();
        //    mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(1))
        //        .ReturnsAsync(GetQuestionWithoutDetails());
        //    var controller = new AnswersController(mockRepo.Object, mapper, webHostEnvironment, fileOperations, userManager);
        //    AnswerViewModel answerViewModel = new AnswerViewModel
        //    {
        //        Id = 1,
        //        QuestionId = 1,
        //        Body = "Test Body"
        //    };

        //    // Act
        //    var result = await controller.AddAnswer(answerViewModel);

        //    // Assert
        //    var viewResult = Assert.IsType<ViewResult>(result);
        //    Assert.Equal("Error", viewResult.ViewName);
        //}

        //[Fact]
        //public async Task EditAnswerGet_ReturnErrorViewResultForAccessCheckFail()
        //{
        //    // Arrange
        //    // access denied result test - UserManager mock needed for more
        //    var mockRepo = new Mock<IAsyncRepository>();
        //    mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(1))
        //        .ReturnsAsync(GetQuestionWithoutDetails());
        //    mockRepo.Setup(repo => repo.GetAnswerByIdWithoutDetailsAsync(1))
        //        .ReturnsAsync(GetAnswerWithoutDetails());

        //    var controller = new AnswersController(mockRepo.Object, mapper, webHostEnvironment, fileOperations, userManager);

        //    // Act
        //    var result = await controller.EditAnswer(1);

        //    // Assert
        //    var viewResult = Assert.IsType<ViewResult>(result);
        //    Assert.Equal("Error", viewResult.ViewName);
        //}

        //[Fact]
        //public async Task EditAnswerPost_ReturnErrorViewResultForModelInvalid()
        //{
        //    // Arrange
        //    var mockRepo = new Mock<IAsyncRepository>();
        //    mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(1))
        //        .ReturnsAsync(GetQuestionWithoutDetails());
        //    mockRepo.Setup(repo => repo.GetAnswerByIdWithoutDetailsAsync(1))
        //        .ReturnsAsync(GetAnswerWithoutDetails());
        //    AnswerViewModel tempAnswerViewModel = new AnswerViewModel
        //    {
        //        Id = 1,
        //        QuestionId = 1,
        //        Body = "Test Body",
        //        UserId = "abcd"
        //    };

        //    var controller = new AnswersController(mockRepo.Object, mapper, webHostEnvironment, fileOperations, userManager);
        //    controller.ModelState.AddModelError("Body", "Required");

        //    // Act
        //    var result = await controller.EditAnswer(tempAnswerViewModel);

        //    // Assert
        //    var viewResult = Assert.IsType<ViewResult>(result);
        //    Assert.NotEqual("Error", viewResult.ViewName);
        //}







        // helper methods

        //private Question GetQuestionWithoutDetails()
        //{
        //    Question tempQuestion = new Question
        //    {
        //        Id = 1,
        //        Title = "Test Title",
        //        Body = "Test Body"
        //    };
        //    return tempQuestion;
        //}

        //private Question GetQuestionWithoutDetailsNull()
        //{
        //    Question tempQuestion = null;
        //    return tempQuestion;
        //}

        //private Answer GetAnswerWithoutDetails()
        //{
        //    Answer tempAnswer = new Answer
        //    {
        //        Id = 1,
        //        QuestionId = 1,
        //        Body = "Test Body",
        //        UserId = "abcd"
        //    };
        //    return tempAnswer;
        //}


    }
}
