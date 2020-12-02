using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
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
    public class AnswersControllerShould
    {
        private UserManager<ApplicationUser> userManager { get; }
        private IWebHostEnvironment webHostEnvironment { get; }
        private IFileOperations fileOperations { get; }
        private IAsyncRepository repository { get; }
        private IMapper mapper { get; }

        [Fact]
        public async Task AddAnswerGet_ReturnAViewResult()
        {
            // Arrange
            //var controller = new AnswersController(repository, mapper, webHostEnvironment, fileOperations, userManager);
            var mockRepo = new Mock<IAsyncRepository>();
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(1))
                .ReturnsAsync(GetQuestionWithoutDetails());
            var controller = new AnswersController(mockRepo.Object, mapper, webHostEnvironment, fileOperations, userManager);

            // Act
            var result = await controller.AddAnswer(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.NotEqual("Error", viewResult.ViewName);
        }

        private Question GetQuestionWithoutDetails()
        {
            Question tempQuestion = new Question
            {
                Id = 1,
                Title = "Test Title",
                Body = "Test Body"
            };
            return tempQuestion;
        }
    }
}
