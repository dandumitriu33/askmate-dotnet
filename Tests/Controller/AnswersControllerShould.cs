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
        public async Task AddAnswerGet_ReturnAViewResult()
        {
            // Arrange
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

        [Fact]
        public async Task AddAnswerPost_ReturnErrorView()
        {
            // Arrange - question exists, model is valid, processing fails
            var mockRepo = new Mock<IAsyncRepository>();
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(1))
                .ReturnsAsync(GetQuestionWithoutDetails());
            var controller = new AnswersController(mockRepo.Object, mapper, webHostEnvironment, fileOperations, userManager);
            AnswerViewModel answerViewModel = new AnswerViewModel
            {
                Id = 1,
                QuestionId = 1,
                Body = "Test Body"
            };

            // Act
            var result = await controller.AddAnswer(answerViewModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task AddAnswerPost_ReturnViewModelInvalid()
        {
            // Arrange - question exists, model is invalid, processing doesn't start
            var mockRepo = new Mock<IAsyncRepository>();
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(1))
                .ReturnsAsync(GetQuestionWithoutDetails());
            var controller = new AnswersController(mockRepo.Object, mapper, webHostEnvironment, fileOperations, userManager);
            AnswerViewModel answerViewModel = new AnswerViewModel
            {
                Id = 1,
                QuestionId = 1,
                Body = ""
            };
            controller.ModelState.AddModelError("Body", "Required");

            // Act
            var result = await controller.AddAnswer(answerViewModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.NotEqual("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task AddAnswerPost_ReturnErrorQuestionInvalid()
        {
            // Arrange - question doesn't exist, model is valid, processing doesn't start
            var mockRepo = new Mock<IAsyncRepository>();
            mockRepo.Setup(repo => repo.GetQuestionByIdWithoutDetailsAsync(1))
                .ReturnsAsync(GetQuestionWithoutDetails());
            var controller = new AnswersController(mockRepo.Object, mapper, webHostEnvironment, fileOperations, userManager);
            AnswerViewModel answerViewModel = new AnswerViewModel
            {
                Id = 1,
                QuestionId = 1,
                Body = "Test Body"
            };

            // Act
            var result = await controller.AddAnswer(answerViewModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        // helper methods
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
