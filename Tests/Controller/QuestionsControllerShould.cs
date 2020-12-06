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

        [Fact]
        public async Task AddQuestionPost_ReturnErrorViewOnException() // Not including IMAGE, too advanced for me at this time
        {
            // Arrange
            // mocking UserManager
            var mockUserManager = MockHelpers.MockUserManager<ApplicationUser>();
            var userFromDb = new ApplicationUser { Email = "test@email.com" };
            mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(userFromDb);

            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            Question tempQuestion = new Question { Id = 1, Title = "Test Title" };
            mockRepo.Setup(repo => repo.AddQuestionAsync(It.IsAny<Question>())).Throws(new Exception()).Verifiable();

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
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", requestResult.ViewName);
            mockUserManager.Verify(mu => mu.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
            mockRepo.Verify(x => x.AddQuestionAsync(It.IsAny<Question>()), Times.Once);
        }

        [Fact]
        public async Task EditQuestionGet_ReturnEditQuestionViewOnSuccess()
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            Question tempQuestion = new Question { Id = 1, Title = "Test Title", UserId = "abcd" };
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
            var controller = new QuestionsController(mockRepo.Object, realMapper, webHostEnvironment, fileOperations, signInManager, userManager)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };
            // ClaimsPrincipal component
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            // Act
            var result = await controller.EditQuestion(1);

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("EditQuestion", requestResult.ViewName);
            mockRepo.Verify(mr => mr.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task EditQuestionGet_ReturnErrorViewOnNullQuestion()
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
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
            var controller = new QuestionsController(mockRepo.Object, realMapper, webHostEnvironment, fileOperations, signInManager, userManager)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };
            // ClaimsPrincipal component
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            // Act
            var result = await controller.EditQuestion(1);

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", requestResult.ViewName);
            mockRepo.Verify(mr => mr.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task EditQuestionGet_ReturnErrorViewOnException()
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            Question tempQuestion = new Question { Id = 1, Title = "Test Title", UserId = "abcd" };
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
            var controller = new QuestionsController(mockRepo.Object, realMapper, webHostEnvironment, fileOperations, signInManager, userManager)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };
            // ClaimsPrincipal component
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            // Act
            var result = await controller.EditQuestion(1);

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", requestResult.ViewName);
            mockRepo.Verify(mr => mr.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task EditQuestionPost_RdirectToDetailsActionOnSuccess() // No IMAGE - too advanced at this time
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            Question tempQuestion = new Question { Id = 1, Title = "Test Title", UserId = "abcd" };
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempQuestion).Verifiable();
            mockRepo.Setup(repo => repo.EditQuestionAsync(It.IsAny<Question>())).Verifiable();

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
            var controller = new QuestionsController(mockRepo.Object, realMapper, webHostEnvironment, fileOperations, signInManager, mockUserManager.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };
            // ClaimsPrincipal component
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            // Act
            var result = await controller.EditQuestion(new QuestionViewModel { Id = 1, Title = "Test Title", UserId = "abcd" });

            // Assert
            var requestResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", requestResult.ActionName);
            mockRepo.Verify(mr => mr.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
            mockUserManager.Verify(mu => mu.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
            mockRepo.Verify(x => x.EditQuestionAsync(It.IsAny<Question>()), Times.Once);
        }

        [Fact]
        public async Task EditQuestionPost_ReturnEditQuestionViewOnInvalidModel() // No IMAGE - too advanced at this time
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            Question tempQuestion = new Question { Id = 1, Title = "Test Title", UserId = "abcd" };
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempQuestion).Verifiable();
            mockRepo.Setup(repo => repo.EditQuestionAsync(It.IsAny<Question>())).Verifiable();

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
            var controller = new QuestionsController(mockRepo.Object, realMapper, webHostEnvironment, fileOperations, signInManager, mockUserManager.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };
            // ClaimsPrincipal component
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
            controller.ModelState.AddModelError("UserId", "Required");

            // Act
            var result = await controller.EditQuestion(new QuestionViewModel { Id = 1, Title = "Test Title", UserId = "abcd" });

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("EditQuestion", requestResult.ViewName);
            mockRepo.Verify(mr => mr.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Never);
            mockUserManager.Verify(mu => mu.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Never);
            mockRepo.Verify(x => x.EditQuestionAsync(It.IsAny<Question>()), Times.Never);
        }

        [Fact]
        public async Task EditQuestionPost_ReturnErrorViewOnNullQuestion() // No IMAGE - too advanced at this time
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            Question tempQuestion = null;
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempQuestion).Verifiable();
            mockRepo.Setup(repo => repo.EditQuestionAsync(It.IsAny<Question>())).Verifiable();

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
            var controller = new QuestionsController(mockRepo.Object, realMapper, webHostEnvironment, fileOperations, signInManager, mockUserManager.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };
            // ClaimsPrincipal component
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            // Act
            var result = await controller.EditQuestion(new QuestionViewModel { Id = 1, Title = "Test Title", UserId = "abcd" });

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", requestResult.ViewName);
            mockRepo.Verify(mr => mr.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
            mockUserManager.Verify(mu => mu.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Never);
            mockRepo.Verify(x => x.EditQuestionAsync(It.IsAny<Question>()), Times.Never);
        }

        [Fact]
        public async Task EditQuestionPost_ReturnErrorViewOnException() // No IMAGE - too advanced at this time
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            Question tempQuestion = new Question { Id = 1, Title = "Test Title", UserId = "abcd" };
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempQuestion).Verifiable();
            mockRepo.Setup(repo => repo.EditQuestionAsync(It.IsAny<Question>())).Throws(new Exception()).Verifiable();

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
            var controller = new QuestionsController(mockRepo.Object, realMapper, webHostEnvironment, fileOperations, signInManager, mockUserManager.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };
            // ClaimsPrincipal component
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            // Act
            var result = await controller.EditQuestion(new QuestionViewModel { Id = 1, Title = "Test Title", UserId = "abcd" });

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", requestResult.ViewName);
            mockRepo.Verify(mr => mr.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
            mockUserManager.Verify(mu => mu.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
            mockRepo.Verify(x => x.EditQuestionAsync(It.IsAny<Question>()), Times.Once);
        }

        [Fact]
        public async Task RemoveQuestionGet_RdirectToAllQuestionsActionOnSuccess()
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            Question tempQuestion = new Question { Id = 1, Title = "Test Title", UserId = "abcd" };
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempQuestion).Verifiable();
            mockRepo.Setup(repo => repo.RemoveQuestionById(It.IsAny<int>())).Verifiable();
            
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
            var controller = new QuestionsController(mockRepo.Object, realMapper, webHostEnvironment, fileOperations, signInManager, userManager)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };
            // ClaimsPrincipal component
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            // Act
            var result = await controller.RemoveQuestion(1);

            // Assert
            var requestResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("AllQuestions", requestResult.ActionName);
            Assert.Equal("Home", requestResult.ControllerName);
            mockRepo.Verify(mr => mr.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(x => x.RemoveQuestionById(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task RemoveQuestionGet_ReturnErrorViewOnNullQuestion()
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            Question tempQuestion = null;
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempQuestion).Verifiable();
            mockRepo.Setup(repo => repo.RemoveQuestionById(It.IsAny<int>())).Verifiable();

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
            var controller = new QuestionsController(mockRepo.Object, realMapper, webHostEnvironment, fileOperations, signInManager, userManager)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };
            // ClaimsPrincipal component
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            // Act
            var result = await controller.RemoveQuestion(1);

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", requestResult.ViewName);
            mockRepo.Verify(mr => mr.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(x => x.RemoveQuestionById(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task RemoveQuestionGet_ReturnErrorViewOnException()
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            Question tempQuestion = new Question { Id = 1, Title = "Test Title", UserId = "abcd" };
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempQuestion).Verifiable();
            mockRepo.Setup(repo => repo.RemoveQuestionById(It.IsAny<int>())).Throws(new Exception()).Verifiable();

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
            var controller = new QuestionsController(mockRepo.Object, realMapper, webHostEnvironment, fileOperations, signInManager, userManager)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };
            // ClaimsPrincipal component
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            // Act
            var result = await controller.RemoveQuestion(1);

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", requestResult.ViewName);
            mockRepo.Verify(mr => mr.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(x => x.RemoveQuestionById(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task RemoveImageGet_RdirectToDetailsActionOnSuccess()
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            Question tempQuestion = new Question { Id = 1, Title = "Test Title", UserId = "abcd" };
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempQuestion).Verifiable();
            mockRepo.Setup(repo => repo.RemoveQuestionImageByQuestionId(It.IsAny<int>())).Verifiable();

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
            var controller = new QuestionsController(mockRepo.Object, realMapper, webHostEnvironment, fileOperations, signInManager, userManager)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };
            // ClaimsPrincipal component
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            // Act
            var result = await controller.RemoveImage(1);

            // Assert
            var requestResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", requestResult.ActionName);
            mockRepo.Verify(mr => mr.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(x => x.RemoveQuestionImageByQuestionId(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task RemoveImageGet_ReturnErrorViewOnNullQuestion()
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            Question tempQuestion = null;
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempQuestion).Verifiable();
            mockRepo.Setup(repo => repo.RemoveQuestionImageByQuestionId(It.IsAny<int>())).Verifiable();

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
            var controller = new QuestionsController(mockRepo.Object, realMapper, webHostEnvironment, fileOperations, signInManager, userManager)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };
            // ClaimsPrincipal component
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            // Act
            var result = await controller.RemoveImage(1);

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", requestResult.ViewName);
            mockRepo.Verify(mr => mr.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(x => x.RemoveQuestionImageByQuestionId(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task RemoveImageGet_ReturnErrorViewOnException()
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            Question tempQuestion = new Question { Id = 1, Title = "Test Title", UserId = "abcd" };
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempQuestion).Verifiable();
            mockRepo.Setup(repo => repo.RemoveQuestionImageByQuestionId(It.IsAny<int>())).Throws(new Exception()).Verifiable();

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
            var controller = new QuestionsController(mockRepo.Object, realMapper, webHostEnvironment, fileOperations, signInManager, userManager)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };
            // ClaimsPrincipal component
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            // Act
            var result = await controller.RemoveImage(1);

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", requestResult.ViewName);
            mockRepo.Verify(mr => mr.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(x => x.RemoveQuestionImageByQuestionId(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task VoteUpQuestionGet_RdirectToAllQuestionsActionOnSuccess()
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            Question tempQuestion = new Question { Id = 1, Title = "Test Title", UserId = "abcd" };
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempQuestion).Verifiable();
            mockRepo.Setup(repo => repo.VoteUpQuestionById(It.IsAny<int>())).Verifiable();

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
            var controller = new QuestionsController(mockRepo.Object, realMapper, webHostEnvironment, fileOperations, signInManager, userManager)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };
            // ClaimsPrincipal component
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            // Act
            var result = await controller.VoteUpQuestion(1);

            // Assert
            var requestResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("AllQuestions", requestResult.ActionName);
            mockRepo.Verify(mr => mr.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(x => x.VoteUpQuestionById(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task VoteUpQuestionGet_RedirectToDetailsActionOnSuccess()
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            Question tempQuestion = new Question { Id = 1, Title = "Test Title", UserId = "abcd" };
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempQuestion).Verifiable();
            mockRepo.Setup(repo => repo.VoteUpQuestionById(It.IsAny<int>())).Verifiable();

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
            var controller = new QuestionsController(mockRepo.Object, realMapper, webHostEnvironment, fileOperations, signInManager, userManager)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };
            // ClaimsPrincipal component
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            // Act
            var result = await controller.VoteUpQuestion(1, "redirectToDetails");

            // Assert
            var requestResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", requestResult.ActionName);
            mockRepo.Verify(mr => mr.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(x => x.VoteUpQuestionById(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task VoteUpQuestionGet_RedirectToHomeActionOnSuccess()
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            Question tempQuestion = new Question { Id = 1, Title = "Test Title", UserId = "abcd" };
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempQuestion).Verifiable();
            mockRepo.Setup(repo => repo.VoteUpQuestionById(It.IsAny<int>())).Verifiable();

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
            var controller = new QuestionsController(mockRepo.Object, realMapper, webHostEnvironment, fileOperations, signInManager, userManager)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };
            // ClaimsPrincipal component
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            // Act
            var result = await controller.VoteUpQuestion(1, "redirectToHome");

            // Assert
            var requestResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", requestResult.ActionName);
            Assert.Equal("Home", requestResult.ControllerName);
            mockRepo.Verify(mr => mr.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(x => x.VoteUpQuestionById(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task VoteUpQuestionGet_ReturnErrorViewOnNullQuestion()
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            Question tempQuestion = null;
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempQuestion).Verifiable();
            mockRepo.Setup(repo => repo.VoteUpQuestionById(It.IsAny<int>())).Verifiable();

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
            var controller = new QuestionsController(mockRepo.Object, realMapper, webHostEnvironment, fileOperations, signInManager, userManager)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };
            // ClaimsPrincipal component
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            // Act
            var result = await controller.VoteUpQuestion(1);

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", requestResult.ViewName);
            mockRepo.Verify(mr => mr.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(x => x.VoteUpQuestionById(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task VoteUpQuestionGet_ReturnErrorViewOnException()
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            Question tempQuestion = new Question { Id = 1, Title = "Test Title", UserId = "abcd" };
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempQuestion).Verifiable();
            mockRepo.Setup(repo => repo.VoteUpQuestionById(It.IsAny<int>())).Throws(new Exception()).Verifiable();

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
            var controller = new QuestionsController(mockRepo.Object, realMapper, webHostEnvironment, fileOperations, signInManager, userManager)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };
            // ClaimsPrincipal component
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            // Act
            var result = await controller.VoteUpQuestion(1);

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", requestResult.ViewName);
            mockRepo.Verify(mr => mr.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(x => x.VoteUpQuestionById(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task VoteDownQuestionGet_RdirectToAllQuestionsActionOnSuccess()
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            Question tempQuestion = new Question { Id = 1, Title = "Test Title", UserId = "abcd" };
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempQuestion).Verifiable();
            mockRepo.Setup(repo => repo.VoteDownQuestionById(It.IsAny<int>())).Verifiable();

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
            var controller = new QuestionsController(mockRepo.Object, realMapper, webHostEnvironment, fileOperations, signInManager, userManager)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };
            // ClaimsPrincipal component
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            // Act
            var result = await controller.VoteDownQuestion(1);

            // Assert
            var requestResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("AllQuestions", requestResult.ActionName);
            mockRepo.Verify(mr => mr.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(x => x.VoteDownQuestionById(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task VoteDownQuestionGet_RedirectToDetailsActionOnSuccess()
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            Question tempQuestion = new Question { Id = 1, Title = "Test Title", UserId = "abcd" };
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempQuestion).Verifiable();
            mockRepo.Setup(repo => repo.VoteDownQuestionById(It.IsAny<int>())).Verifiable();

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
            var controller = new QuestionsController(mockRepo.Object, realMapper, webHostEnvironment, fileOperations, signInManager, userManager)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };
            // ClaimsPrincipal component
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            // Act
            var result = await controller.VoteDownQuestion(1, "redirectToDetails");

            // Assert
            var requestResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", requestResult.ActionName);
            mockRepo.Verify(mr => mr.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(x => x.VoteDownQuestionById(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task VoteDownQuestionGet_RedirectToHomeActionOnSuccess()
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            Question tempQuestion = new Question { Id = 1, Title = "Test Title", UserId = "abcd" };
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempQuestion).Verifiable();
            mockRepo.Setup(repo => repo.VoteDownQuestionById(It.IsAny<int>())).Verifiable();

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
            var controller = new QuestionsController(mockRepo.Object, realMapper, webHostEnvironment, fileOperations, signInManager, userManager)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };
            // ClaimsPrincipal component
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            // Act
            var result = await controller.VoteDownQuestion(1, "redirectToHome");

            // Assert
            var requestResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", requestResult.ActionName);
            Assert.Equal("Home", requestResult.ControllerName);
            mockRepo.Verify(mr => mr.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(x => x.VoteDownQuestionById(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task VoteDownQuestionGet_ReturnErrorViewOnNullQuestion()
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            Question tempQuestion = null;
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempQuestion).Verifiable();
            mockRepo.Setup(repo => repo.VoteDownQuestionById(It.IsAny<int>())).Verifiable();

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
            var controller = new QuestionsController(mockRepo.Object, realMapper, webHostEnvironment, fileOperations, signInManager, userManager)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };
            // ClaimsPrincipal component
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            // Act
            var result = await controller.VoteDownQuestion(1);

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", requestResult.ViewName);
            mockRepo.Verify(mr => mr.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(x => x.VoteDownQuestionById(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task VoteDownQuestionGet_ReturnErrorViewOnException()
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            Question tempQuestion = new Question { Id = 1, Title = "Test Title", UserId = "abcd" };
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempQuestion).Verifiable();
            mockRepo.Setup(repo => repo.VoteDownQuestionById(It.IsAny<int>())).Throws(new Exception()).Verifiable();

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
            var controller = new QuestionsController(mockRepo.Object, realMapper, webHostEnvironment, fileOperations, signInManager, userManager)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };
            // ClaimsPrincipal component
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            // Act
            var result = await controller.VoteDownQuestion(1);

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", requestResult.ViewName);
            mockRepo.Verify(mr => mr.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(x => x.VoteDownQuestionById(It.IsAny<int>()), Times.Once);
        }









    }
}
