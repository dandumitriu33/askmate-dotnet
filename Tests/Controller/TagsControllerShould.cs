using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
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
    public class TagsControllerShould
    {
        private IAsyncRepository repository { get; }
        private IMapper mapper { get; }

        [Fact]
        public async Task AttachTagGet_ReturnAttachTagViewOnSuccess()
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
            var result = await controller.AttachTag(1);

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("AttachTag", requestResult.ViewName);
            mockRepo.Verify(x => x.GetQuestionByIdWithoutDetailsAsync(It.IsAny<int>()), Times.Once);
            mockRepo.Verify(x => x.GetAllTagsNoDuplicates(It.IsAny<int>()), Times.Once);
        }


    }
}
