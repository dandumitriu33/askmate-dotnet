using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Web.AutomapperProfiles;
using Web.Controllers;
using Web.ViewModels;
using Xunit;

namespace Tests.Controller
{
    public class TagsControllerShould
    {
        private IAsyncRepository repository { get; }
        private IMapper mapper { get; }

        [Fact]
        public async Task AddTagGet_ReturnAttachTagViewOnSuccess()
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            Question tempQuestion = new Question { Id = 1, Title = "Test Title" };
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempQuestion).Verifiable();
            mockRepo.Setup(repo => repo.GetAllTagsNoDuplicates(It.IsAny<int>())).ReturnsAsync(new List<Tag>()).Verifiable();

            // adding a real mapper
            var myProfile = new AskMateProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var realMapper = new Mapper(configuration);

            var controller = new TagsController(mockRepo.Object, realMapper);

            // Act
            var result = await controller.AddTag(1);

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("AddTag", requestResult.ViewName);
            mockRepo.Verify(x => x.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(x => x.GetAllTagsNoDuplicates(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task AddTagGet_ReturnErrorViewOnNullQuestion()
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            Question tempQuestion = null;
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempQuestion).Verifiable();
            mockRepo.Setup(repo => repo.GetAllTagsNoDuplicates(It.IsAny<int>())).ReturnsAsync(new List<Tag>()).Verifiable();

            // adding a real mapper
            var myProfile = new AskMateProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var realMapper = new Mapper(configuration);

            var controller = new TagsController(mockRepo.Object, realMapper);

            // Act
            var result = await controller.AddTag(1);

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", requestResult.ViewName);
            mockRepo.Verify(x => x.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(x => x.GetAllTagsNoDuplicates(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task AddTagGet_ReturnErrorViewOnException()
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            Question tempQuestion = new Question { Id = 1, Title = "Test Title" };
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempQuestion).Verifiable();
            mockRepo.Setup(repo => repo.GetAllTagsNoDuplicates(It.IsAny<int>())).Throws(new Exception()).Verifiable();

            // adding a real mapper
            var myProfile = new AskMateProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var realMapper = new Mapper(configuration);

            var controller = new TagsController(mockRepo.Object, realMapper);

            // Act
            var result = await controller.AddTag(1);

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", requestResult.ViewName);
            mockRepo.Verify(x => x.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(x => x.GetAllTagsNoDuplicates(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task AddTagPost_RedirectToDetailsActionOnSuccessExistingTag()
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            Question tempQuestion = new Question { Id = 1, Title = "Test Title" };
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempQuestion).Verifiable();
            Tag tempTag = new Tag { Id = 1, Name = "Test Tag" };
            mockRepo.Setup(repo => repo.GetTagByIdAsync(It.IsAny<int>())).ReturnsAsync(tempTag).Verifiable();
            mockRepo.Setup(repo => repo.AddTagAsync(It.IsAny<Tag>())).ReturnsAsync(tempTag).Verifiable();
            mockRepo.Setup(repo => repo.AttachQuestionTagAsync(It.IsAny<QuestionTag>())).Verifiable();

            // adding a real mapper
            var myProfile = new AskMateProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var realMapper = new Mapper(configuration);

            var controller = new TagsController(mockRepo.Object, realMapper);

            // Act
            var result = await controller.AddTag(new TagViewModel { Id = 1, Name = "Test Name" }, 1);

            // Assert
            var requestResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", requestResult.ActionName);
            Assert.Equal("Questions", requestResult.ControllerName);
            mockRepo.Verify(x => x.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Exactly(2));
            mockRepo.Verify(x => x.GetTagByIdAsync(It.IsAny<int>()), Times.Exactly(2));
            mockRepo.Verify(x => x.AddTagAsync(It.IsAny<Tag>()), Times.Never);
            mockRepo.Verify(x => x.AttachQuestionTagAsync(It.IsAny<QuestionTag>()), Times.Once);
        }

        [Fact]
        public async Task AddTagPost_RedirectToDetailsActionOnSuccessNewTag()
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            Question tempQuestion = new Question { Id = 1, Title = "Test Title" };
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempQuestion).Verifiable();
            Tag tempTag = new Tag { Id = 1, Name = "Test Tag" };
            Tag tempNullTag = null;
            mockRepo.SetupSequence(repo => repo.GetTagByIdAsync(It.IsAny<int>())).ReturnsAsync(tempNullTag).ReturnsAsync(tempTag);
            mockRepo.Setup(repo => repo.AddTagAsync(It.IsAny<Tag>())).ReturnsAsync(tempTag).Verifiable();
            mockRepo.Setup(repo => repo.AttachQuestionTagAsync(It.IsAny<QuestionTag>())).Verifiable();

            // adding a real mapper
            var myProfile = new AskMateProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var realMapper = new Mapper(configuration);

            var controller = new TagsController(mockRepo.Object, realMapper);

            // Act
            var result = await controller.AddTag(new TagViewModel { Id = 1, Name = "Test Name" }, 1);

            // Assert
            var requestResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", requestResult.ActionName);
            Assert.Equal("Questions", requestResult.ControllerName);
            mockRepo.Verify(x => x.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Exactly(2));
            mockRepo.Verify(x => x.AddTagAsync(It.IsAny<Tag>()), Times.Once);
            mockRepo.Verify(x => x.AttachQuestionTagAsync(It.IsAny<QuestionTag>()), Times.Once);
        }

        [Fact]
        public async Task AddTagPost_ReturnAttachTagOnInvalidModel()
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            Question tempQuestion = new Question { Id = 1, Title = "Test Title" };
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempQuestion).Verifiable();
            Tag tempTag = new Tag { Id = 1, Name = "Test Tag" };
            mockRepo.Setup(repo => repo.GetTagByIdAsync(It.IsAny<int>())).ReturnsAsync(tempTag).Verifiable();
            mockRepo.Setup(repo => repo.AddTagAsync(It.IsAny<Tag>())).ReturnsAsync(tempTag).Verifiable();
            mockRepo.Setup(repo => repo.AttachQuestionTagAsync(It.IsAny<QuestionTag>())).Verifiable();

            // adding a real mapper
            var myProfile = new AskMateProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var realMapper = new Mapper(configuration);

            var controller = new TagsController(mockRepo.Object, realMapper);
            controller.ModelState.AddModelError("Name", "Required");

            // Act
            var result = await controller.AddTag(new TagViewModel { Id = 1 }, 1);

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("AddTag", requestResult.ViewName);
            mockRepo.Verify(x => x.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Never);
            mockRepo.Verify(x => x.GetTagByIdAsync(It.IsAny<int>()), Times.Never);
            mockRepo.Verify(x => x.AddTagAsync(It.IsAny<Tag>()), Times.Never);
            mockRepo.Verify(x => x.AttachQuestionTagAsync(It.IsAny<QuestionTag>()), Times.Never);
        }

        [Fact]
        public async Task AddTagPost_ReturnErrorOnNullQuestion()
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            Question tempQuestion = null;
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempQuestion).Verifiable();
            Tag tempTag = new Tag { Id = 1, Name = "Test Tag" };
            mockRepo.Setup(repo => repo.GetTagByIdAsync(It.IsAny<int>())).ReturnsAsync(tempTag).Verifiable();
            mockRepo.Setup(repo => repo.AddTagAsync(It.IsAny<Tag>())).ReturnsAsync(tempTag).Verifiable();
            mockRepo.Setup(repo => repo.AttachQuestionTagAsync(It.IsAny<QuestionTag>())).Verifiable();

            // adding a real mapper
            var myProfile = new AskMateProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var realMapper = new Mapper(configuration);

            // mocking Response.StatusCode = 404 setter
            var mockHttpContext = new Mock<HttpContext>();
            var response = new Mock<HttpResponse>();
            mockHttpContext.SetupGet(x => x.Response).Returns(response.Object);

            //creates an instance of an asp.net mvc controller
            var controller = new TagsController(mockRepo.Object, realMapper)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };

            // Act
            var result = await controller.AddTag(new TagViewModel { Id = 1, Name = "Test Tag" }, 1);

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", requestResult.ViewName);
            mockRepo.Verify(x => x.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(x => x.GetTagByIdAsync(It.IsAny<int>()), Times.Never);
            mockRepo.Verify(x => x.AddTagAsync(It.IsAny<Tag>()), Times.Never);
            mockRepo.Verify(x => x.AttachQuestionTagAsync(It.IsAny<QuestionTag>()), Times.Never);
        }

        [Fact]
        public async Task AddTagPost_ReturnErrorOnException()
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            Question tempQuestion = new Question { Id = 1, Title = "Test Title" };
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>())).Throws(new Exception()).Verifiable();
            Tag tempTag = new Tag { Id = 1, Name = "Test Tag" };
            mockRepo.Setup(repo => repo.GetTagByIdAsync(It.IsAny<int>())).ReturnsAsync(tempTag).Verifiable();
            mockRepo.Setup(repo => repo.AddTagAsync(It.IsAny<Tag>())).ReturnsAsync(tempTag).Verifiable();
            mockRepo.Setup(repo => repo.AttachQuestionTagAsync(It.IsAny<QuestionTag>())).Verifiable();

            // adding a real mapper
            var myProfile = new AskMateProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var realMapper = new Mapper(configuration);

            // mocking Response.StatusCode = 404 setter
            var mockHttpContext = new Mock<HttpContext>();
            var response = new Mock<HttpResponse>();
            mockHttpContext.SetupGet(x => x.Response).Returns(response.Object);

            //creates an instance of an asp.net mvc controller
            var controller = new TagsController(mockRepo.Object, realMapper)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };

            // Act
            var result = await controller.AddTag(new TagViewModel { Id = 1, Name = "Test Tag" }, 1);

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", requestResult.ViewName);
            mockRepo.Verify(x => x.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(x => x.GetTagByIdAsync(It.IsAny<int>()), Times.Never);
            mockRepo.Verify(x => x.AddTagAsync(It.IsAny<Tag>()), Times.Never);
            mockRepo.Verify(x => x.AttachQuestionTagAsync(It.IsAny<QuestionTag>()), Times.Never);
        }

        [Fact]
        public async Task DetachQuestionTagGet_RedirectToDetailsActionOnSuccess()
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            Question tempQuestion = new Question { Id = 1, Title = "Test Title" };
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempQuestion).Verifiable();
            Tag tempTag = new Tag { Id = 1, Name = "Test Tag" };
            mockRepo.Setup(repo => repo.GetTagByIdAsync(It.IsAny<int>())).ReturnsAsync(tempTag).Verifiable();
            mockRepo.Setup(repo => repo.DetachTag(It.IsAny<QuestionTag>())).Verifiable();

            // adding a real mapper
            var myProfile = new AskMateProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var realMapper = new Mapper(configuration);

            // mocking Response.StatusCode = 404 setter
            var mockHttpContext = new Mock<HttpContext>();
            var response = new Mock<HttpResponse>();
            mockHttpContext.SetupGet(x => x.Response).Returns(response.Object);

            var controller = new TagsController(mockRepo.Object, realMapper)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };

            // Act
            var result = await controller.DetachQuestionTag(1, 1);

            // Assert
            var requestResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", requestResult.ActionName);
            Assert.Equal("Questions", requestResult.ControllerName);
            mockRepo.Verify(x => x.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(x => x.GetTagByIdAsync(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(x => x.DetachTag(It.IsAny<QuestionTag>()), Times.Once);
        }

        [Fact]
        public async Task DetachQuestionTagGet_ReturnErrorViewOnNullQuestion()
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            Question tempQuestion = null;
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempQuestion).Verifiable();
            Tag tempTag = new Tag { Id = 1, Name = "Test Tag" };
            mockRepo.Setup(repo => repo.GetTagByIdAsync(It.IsAny<int>())).ReturnsAsync(tempTag).Verifiable();
            mockRepo.Setup(repo => repo.DetachTag(It.IsAny<QuestionTag>())).Verifiable();

            // adding a real mapper
            var myProfile = new AskMateProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var realMapper = new Mapper(configuration);

            // mocking Response.StatusCode = 404 setter
            var mockHttpContext = new Mock<HttpContext>();
            var response = new Mock<HttpResponse>();
            mockHttpContext.SetupGet(x => x.Response).Returns(response.Object);

            var controller = new TagsController(mockRepo.Object, realMapper)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };

            // Act
            var result = await controller.DetachQuestionTag(1, 1);

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", requestResult.ViewName);
            mockRepo.Verify(x => x.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(x => x.GetTagByIdAsync(It.IsAny<int>()), Times.Never);
            mockRepo.Verify(x => x.DetachTag(It.IsAny<QuestionTag>()), Times.Never);
        }

        [Fact]
        public async Task DetachQuestionTagGet_ReturnErrorViewOnNullTag()
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            Question tempQuestion = new Question { Id = 1, Title = "Test Title" };
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempQuestion).Verifiable();
            Tag tempTag = null;
            mockRepo.Setup(repo => repo.GetTagByIdAsync(It.IsAny<int>())).ReturnsAsync(tempTag).Verifiable();
            mockRepo.Setup(repo => repo.DetachTag(It.IsAny<QuestionTag>())).Verifiable();

            // adding a real mapper
            var myProfile = new AskMateProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var realMapper = new Mapper(configuration);

            // mocking Response.StatusCode = 404 setter
            var mockHttpContext = new Mock<HttpContext>();
            var response = new Mock<HttpResponse>();
            mockHttpContext.SetupGet(x => x.Response).Returns(response.Object);

            var controller = new TagsController(mockRepo.Object, realMapper)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };

            // Act
            var result = await controller.DetachQuestionTag(1, 1);

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", requestResult.ViewName);
            mockRepo.Verify(x => x.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(x => x.GetTagByIdAsync(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(x => x.DetachTag(It.IsAny<QuestionTag>()), Times.Never);
        }

        [Fact]
        public async Task DetachQuestionTagGet_ReturnErrorViewOnException()
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            Question tempQuestion = new Question { Id = 1, Title = "Test Title" };
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempQuestion).Verifiable();
            Tag tempTag = new Tag { Id = 1, Name = "Test Tag" };
            mockRepo.Setup(repo => repo.GetTagByIdAsync(It.IsAny<int>())).ReturnsAsync(tempTag).Verifiable();
            mockRepo.Setup(repo => repo.DetachTag(It.IsAny<QuestionTag>())).Throws(new Exception()).Verifiable();

            // adding a real mapper
            var myProfile = new AskMateProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var realMapper = new Mapper(configuration);

            // mocking Response.StatusCode = 404 setter
            var mockHttpContext = new Mock<HttpContext>();
            var response = new Mock<HttpResponse>();
            mockHttpContext.SetupGet(x => x.Response).Returns(response.Object);

            var controller = new TagsController(mockRepo.Object, realMapper)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };

            // Act
            var result = await controller.DetachQuestionTag(1, 1);

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", requestResult.ViewName);
            mockRepo.Verify(x => x.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(x => x.GetTagByIdAsync(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(x => x.DetachTag(It.IsAny<QuestionTag>()), Times.Once);
        }

        [Fact]
        public async Task AttachQuestionTagGet_RedirectToDetailsActionOnSuccess()
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            Question tempQuestion = new Question { Id = 1, Title = "Test Title" };
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempQuestion).Verifiable();
            Tag tempTag = new Tag { Id = 1, Name = "Test Tag" };
            mockRepo.Setup(repo => repo.GetTagByIdAsync(It.IsAny<int>())).ReturnsAsync(tempTag).Verifiable();
            mockRepo.Setup(repo => repo.AttachQuestionTagAsync(It.IsAny<QuestionTag>())).Verifiable();

            // adding a real mapper
            var myProfile = new AskMateProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var realMapper = new Mapper(configuration);

            // mocking Response.StatusCode = 404 setter
            var mockHttpContext = new Mock<HttpContext>();
            var response = new Mock<HttpResponse>();
            mockHttpContext.SetupGet(x => x.Response).Returns(response.Object);

            var controller = new TagsController(mockRepo.Object, realMapper)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };

            // Act
            var result = await controller.AttachQuestionTag(1, 1);

            // Assert
            var requestResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", requestResult.ActionName);
            Assert.Equal("Questions", requestResult.ControllerName);
            mockRepo.Verify(x => x.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(x => x.GetTagByIdAsync(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(x => x.AttachQuestionTagAsync(It.IsAny<QuestionTag>()), Times.Once);
        }

        [Fact]
        public async Task AttachQuestionTagGet_ReturnErrorViewOnNullQuestion()
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            Question tempQuestion = null;
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempQuestion).Verifiable();
            Tag tempTag = new Tag { Id = 1, Name = "Test Tag" };
            mockRepo.Setup(repo => repo.GetTagByIdAsync(It.IsAny<int>())).ReturnsAsync(tempTag).Verifiable();
            mockRepo.Setup(repo => repo.AttachQuestionTagAsync(It.IsAny<QuestionTag>())).Verifiable();

            // adding a real mapper
            var myProfile = new AskMateProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var realMapper = new Mapper(configuration);

            // mocking Response.StatusCode = 404 setter
            var mockHttpContext = new Mock<HttpContext>();
            var response = new Mock<HttpResponse>();
            mockHttpContext.SetupGet(x => x.Response).Returns(response.Object);

            var controller = new TagsController(mockRepo.Object, realMapper)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };

            // Act
            var result = await controller.AttachQuestionTag(1, 1);

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", requestResult.ViewName);
            mockRepo.Verify(x => x.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(x => x.GetTagByIdAsync(It.IsAny<int>()), Times.Never);
            mockRepo.Verify(x => x.AttachQuestionTagAsync(It.IsAny<QuestionTag>()), Times.Never);
        }

        [Fact]
        public async Task AttachQuestionTagGet_ReturnErrorViewOnNullTag()
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            Question tempQuestion = new Question { Id = 1, Title = "Test Title" };
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>())).ReturnsAsync(tempQuestion).Verifiable();
            Tag tempTag = null;
            mockRepo.Setup(repo => repo.GetTagByIdAsync(It.IsAny<int>())).ReturnsAsync(tempTag).Verifiable();
            mockRepo.Setup(repo => repo.AttachQuestionTagAsync(It.IsAny<QuestionTag>())).Verifiable();

            // adding a real mapper
            var myProfile = new AskMateProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var realMapper = new Mapper(configuration);

            // mocking Response.StatusCode = 404 setter
            var mockHttpContext = new Mock<HttpContext>();
            var response = new Mock<HttpResponse>();
            mockHttpContext.SetupGet(x => x.Response).Returns(response.Object);

            var controller = new TagsController(mockRepo.Object, realMapper)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };

            // Act
            var result = await controller.AttachQuestionTag(1, 1);

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", requestResult.ViewName);
            mockRepo.Verify(x => x.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(x => x.GetTagByIdAsync(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(x => x.AttachQuestionTagAsync(It.IsAny<QuestionTag>()), Times.Never);
        }







    }
}
