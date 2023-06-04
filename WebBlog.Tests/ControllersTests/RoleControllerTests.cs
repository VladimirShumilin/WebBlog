using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WebBlog.Contracts.Models.Request.Role;
using WebBlog.Controllers;
using WebBlog.DAL.Models;

namespace WebBlog.Tests.ControllersTests
{
    public class RoleControllerTests
    {
        private readonly Mock<RoleManager<IdentityRole>> _roleManagerMock;
        private readonly IMapper _mapperMock;
        private readonly RoleController _controller;

        public RoleControllerTests()
        {
            _roleManagerMock = new Mock<RoleManager<IdentityRole>>(
                new Mock<IRoleStore<IdentityRole>>().Object,
                null, null, null, null);

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            });
            _mapperMock = config.CreateMapper();

            _controller = new RoleController(_roleManagerMock.Object, _mapperMock);
        }

        [Fact]
        public async Task GetRole_WithValidId_ReturnsRole()
        {
            // Arrange
            var roleId = "1";
            var role = new IdentityRole { Id = roleId, Name = "Admin" };
            _roleManagerMock.Setup(x => x.FindByIdAsync(roleId)).ReturnsAsync(role);

            // Act
            var result = await _controller.GetRole(roleId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedRole = Assert.IsType<IdentityRole>(okResult.Value);
            Assert.Equal(role, returnedRole);
        }

        [Fact]
        public async Task GetRole_WithInvalidId_ReturnsBadRequest()
        {
            // Arrange
            var roleId = "";

            // Act
            var result = await _controller.GetRole(roleId);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task EditRole_WithValidRequest_ReturnsOk()
        {
            // Arrange
            var roleId = "1";
            var roleName = "NewAdmin";
            var request = new EditRoleRequest { Id = roleId, Name = roleName };
            var role = new IdentityRole { Id = roleId, Name = "Admin" };
            _roleManagerMock.Setup(x => x.FindByIdAsync(roleId)).ReturnsAsync(role);

            // Act
            var result = await _controller.EditRole(request);

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task EditRole_WithInvalidRequest_ReturnsBadRequest()
        {
            // Arrange
            var request = new EditRoleRequest();

            // Act
            var result = await _controller.EditRole(request);

            // Assert
             Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task EditRole_WithInvalidId_ReturnsBadRequest()
        {
            // Arrange
            var roleId = "";
            var roleName = "NewAdmin";
            var request = new EditRoleRequest { Id = roleId, Name = roleName };
            _roleManagerMock.Setup(x => x.FindByIdAsync(roleId)).ReturnsAsync((IdentityRole?)null);
            // Act
            var result = await _controller.EditRole(request);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task EditRole_WithNotFoundId_ReturnsBadRequest()
        {
            // Arrange
            var roleId = "1";
            var roleName = "NewAdmin";
            var request = new EditRoleRequest { Id = roleId, Name = roleName };
            _roleManagerMock.Setup(x => x.FindByIdAsync(roleId)).ReturnsAsync((IdentityRole?)null);

            // Act
            var result = await _controller.EditRole(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal($"Role Id:{roleId} not found", badRequestResult.Value);
        }

        [Fact]
        public async Task DeleteRole_WithValidId_ReturnsOk()
        {
            // Arrange
            var roleId = "1";
            var role = new IdentityRole { Id = roleId, Name = "Admin" };
            _roleManagerMock.Setup(x => x.FindByIdAsync(roleId)).ReturnsAsync(role);

            // Act
            var result = await _controller.DeleteRole(roleId);

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task DeleteRole_WithInvalidId_ReturnsBadRequest()
        {
            // Arrange
            var roleId = "";

            // Act
            var result = await _controller.DeleteRole(roleId);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task DeleteRole_WithNotFoundId_ReturnsBadRequest()
        {
            // Arrange
            var roleId = "1";
            _roleManagerMock.Setup(x => x.FindByIdAsync(roleId)).ReturnsAsync((IdentityRole?)null);

            // Act
            var result = await _controller.DeleteRole(roleId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal($"Role Id:{roleId} not found", badRequestResult.Value);
        }
    }
}
