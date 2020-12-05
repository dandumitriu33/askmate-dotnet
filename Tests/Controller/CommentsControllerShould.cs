using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Web.Controllers;
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
        public async Task AddAnswerCommentGet_ReturnViewErrorCommentOnNullAnswer()
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
        public async Task AddAnswerCommentGet_ReturnViewErrorCommentOnNullQuestion()
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




    }
}
