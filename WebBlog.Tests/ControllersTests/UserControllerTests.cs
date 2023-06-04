using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using WebBlog.Contracts.Models.Query.User;
using WebBlog.Controllers;
using WebBlog.DAL.Models;

namespace WebBlog.Tests.ControllersTests
{
    public class UserControllerTests
    {

        private readonly Mock<UserManager<BlogUser>> _userManagerMock;
        private readonly Mock<RoleManager<IdentityRole>> _roleManagerMock;
        private readonly Mock<ILogger<UserController>> _loggerMock;
        private readonly Mock<IMapper> _mapperMock;

        public UserControllerTests()
        {
            _userManagerMock = new Mock<UserManager<BlogUser>>(new Mock<IUserStore<BlogUser>>().Object,
                null, null, null, null, null, null, null, null);
            _roleManagerMock = new Mock<RoleManager<IdentityRole>>(new Mock<IRoleStore<IdentityRole>>().Object,
                null, null, null, null);
            _loggerMock = new Mock<ILogger<UserController>>();
            _mapperMock = new Mock<IMapper>();
        }

        [Fact]
        public async Task AddUser_ReturnsOkResult_WhenModelStateIsValid()
        {
            // Arrange
            var controller = new UserController(_userManagerMock.Object, _roleManagerMock.Object,
                _mapperMock.Object, _loggerMock.Object);
            var newUserRequest = new NewUserRequest { UserName = "testuser", Email = "testuser@example.com", Password = "password" };
            var user = new BlogUser { UserName = "testuser", Email = "testuser@example.com" };
            var resultSucceeded = IdentityResult.Success;

            _mapperMock.Setup(m => m.Map<NewUserRequest, BlogUser>(newUserRequest)).Returns(user);
            _userManagerMock.Setup(m => m.CreateAsync(user, newUserRequest.Password)).ReturnsAsync(resultSucceeded);

            // Act
            var result = await controller.AddUser(newUserRequest);

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        }

        [Fact]
        public async Task AddUser_ReturnsBadRequestResult_WhenModelStateIsInvalid()
        {
            // Arrange
            var controller = new UserController(_userManagerMock.Object, _roleManagerMock.Object,
                _mapperMock.Object, _loggerMock.Object);
            var newUserRequest = new NewUserRequest { UserName = "testuser", Email = "testuser@example.com", Password = "" };
            controller.ModelState.AddModelError("Password", "The Password field is required.");

            // Act
            var result = await controller.AddUser(newUserRequest);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestResult>(result);
            Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
        }

        [Fact]
        public async Task AddUser_ReturnsBadRequestObjectResult_WhenCreateUserFails()
        {
            // Arrange
            var controller = new UserController(_userManagerMock.Object, _roleManagerMock.Object,
                _mapperMock.Object, _loggerMock.Object);
            var newUserRequest = new NewUserRequest { UserName = "testuser", Email = "testuser@example.com", Password = "password" };
            var user = new BlogUser { UserName = "testuser", Email = "testuser@example.com" };
            var resultFailed = IdentityResult.Failed(new IdentityError { Description = "Error message." });

            _mapperMock.Setup(m => m.Map<NewUserRequest, BlogUser>(newUserRequest)).Returns(user);
            _userManagerMock.Setup(m => m.CreateAsync(user, newUserRequest.Password)).ReturnsAsync(resultFailed);

            // Act
            var result = await controller.AddUser(newUserRequest);

            // Assert
            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(StatusCodes.Status400BadRequest, badRequestObjectResult.StatusCode);
            Assert.Equal(resultFailed.Errors, badRequestObjectResult.Value);
        }

        [Fact]
        public async Task GetUser_ReturnsOkResult_WhenUserExists()
        {
            // Arrange
            var controller = new UserController(_userManagerMock.Object, _roleManagerMock.Object,
                _mapperMock.Object, _loggerMock.Object);
            var userId = "1";
            var user = new BlogUser { Id = userId, UserName = "testuser", Email = "testuser@example.com" };

            _userManagerMock.Setup(m => m.FindByIdAsync(userId)).ReturnsAsync(user);

            // Act
            var result = await controller.GetUser(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
            Assert.Equal(user, okResult.Value);
        }

        [Fact]
        public async Task GetUser_ReturnsNotFoundResult_WhenUserDoesNotExist()
        {
            // Arrange
            var controller = new UserController(_userManagerMock.Object, _roleManagerMock.Object,
                _mapperMock.Object, _loggerMock.Object);
            var userId = "1";

            _userManagerMock.Setup(m => m.FindByIdAsync(userId)).ReturnsAsync((BlogUser?)null);

            // Act
            var result = await controller.GetUser(userId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result);
            Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
        }
    }
}
