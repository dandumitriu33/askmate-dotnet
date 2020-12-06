using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
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
    public class UserControllerShould
    {
        private IAsyncRepository repository { get; }
        private IMapper mapper { get; }
        private UserManager<ApplicationUser> userManager { get; }

        [Fact]
        public async Task AllUsers_ReturnAllUsersViewOnSuccess()
        {
            // Arrange
            // mocking repository
            var mockRepo = new Mock<IAsyncRepository>();
            mockRepo.Setup(repo => repo.GetAllUsers()).ReturnsAsync(new List<ApplicationUser>()).Verifiable();
            
            // adding a real mapper
            var myProfile = new AskMateProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var realMapper = new Mapper(configuration);

            var controller = new UserController(mockRepo.Object, realMapper, userManager);

            // Act

            var result = await controller.AllUsers();

            // Assert
            var requestResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("AllUsers", requestResult.ViewName);
            mockRepo.Verify(x => x.GetAllUsers(), Times.Once);
        }





    }
}
