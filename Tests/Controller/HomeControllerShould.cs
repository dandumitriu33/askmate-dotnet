using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Web.AutomapperProfiles;
using Web.Controllers;
using Xunit;

namespace Tests.Controller
{
    public class HomeControllerShould
    {
        private ILogger<HomeController> logger { get; }
        private IAsyncRepository repository { get; }
        private IMapper mapper { get; }

        [Fact]
        public async Task IndexGet_ReturnIndexViewOnSuccess()
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            Question tempQuestion = new Question { Id = 1, Title = "Test Title" };
            Question tempQuestion2 = new Question { Id = 2, Title = "Test Title 2" };
            List<Question> tempQuestionsList = new List<Question>();
            tempQuestionsList.Add(tempQuestion);
            tempQuestionsList.Add(tempQuestion2);
            mockRepo.Setup(repo => repo.GetLatestQuestions(It.IsAny<int>())).ReturnsAsync(tempQuestionsList).Verifiable();

            // adding a real mapper
            var myProfile = new AskMateProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var realMapper = new Mapper(configuration);

            // mocking Response.StatusCode = 404 setter
            var mockHttpContext = new Mock<HttpContext>();
            var response = new Mock<HttpResponse>();
            mockHttpContext.SetupGet(x => x.Response).Returns(response.Object);

            //creates an instance of an asp.net mvc controller
            var controller = new HomeController(logger, mockRepo.Object, realMapper)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };

            // Act
            var result = await controller.Index();

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Index", requestResult.ViewName);
            mockRepo.Verify(x => x.GetLatestQuestions(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task IndexGet_ReturnErrorViewOnException()
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            Question tempQuestion = new Question { Id = 1, Title = "Test Title" };
            Question tempQuestion2 = new Question { Id = 2, Title = "Test Title 2" };
            List<Question> tempQuestionsList = new List<Question>();
            tempQuestionsList.Add(tempQuestion);
            tempQuestionsList.Add(tempQuestion2);
            mockRepo.Setup(repo => repo.GetLatestQuestions(It.IsAny<int>())).Throws(new Exception()).Verifiable();

            // adding a real mapper
            var myProfile = new AskMateProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var realMapper = new Mapper(configuration);

            // mocking Response.StatusCode = 404 setter
            var mockHttpContext = new Mock<HttpContext>();
            var response = new Mock<HttpResponse>();
            mockHttpContext.SetupGet(x => x.Response).Returns(response.Object);

            //creates an instance of an asp.net mvc controller
            var controller = new HomeController(logger, mockRepo.Object, realMapper)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };

            // Act
            var result = await controller.Index();

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", requestResult.ViewName);
            mockRepo.Verify(x => x.GetLatestQuestions(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task SearchGet_ReturnSearchViewOnSuccess()
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            Question tempQuestion = new Question { Id = 1, Title = "Test Title" };
            Question tempQuestion2 = new Question { Id = 2, Title = "Test Title 2" };
            List<Question> tempQuestionsList = new List<Question>();
            tempQuestionsList.Add(tempQuestion);
            tempQuestionsList.Add(tempQuestion2);
            mockRepo.Setup(repo => repo.GetSearchResults(It.IsAny<string>())).ReturnsAsync(tempQuestionsList).Verifiable();

            // adding a real mapper
            var myProfile = new AskMateProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var realMapper = new Mapper(configuration);

            // mocking Response.StatusCode = 404 setter
            var mockHttpContext = new Mock<HttpContext>();
            var response = new Mock<HttpResponse>();
            mockHttpContext.SetupGet(x => x.Response).Returns(response.Object);

            //creates an instance of an asp.net mvc controller
            var controller = new HomeController(logger, mockRepo.Object, realMapper)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };

            // Act
            var result = await controller.Search("abcd");

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Search", requestResult.ViewName);
            mockRepo.Verify(x => x.GetSearchResults(It.IsAny<string>()), Times.Once);
        }











    }
}
