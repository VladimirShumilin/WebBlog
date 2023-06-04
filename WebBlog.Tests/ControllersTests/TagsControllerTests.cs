using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Xml.Linq;
using WebBlog.Contracts.Models.Request.Tag;
using WebBlog.Contracts.Models.Responce.Article;
using WebBlog.Contracts.Models.Responce.Comment;
using WebBlog.Contracts.Models.Responce.Tag;
using WebBlog.Controllers;
using WebBlog.DAL.Interfaces;
using WebBlog.DAL.Models;


namespace WebBlog.Tests.ControllersTests
{
    public class TagsControllerTests
    {
        private readonly Mock<ITagRepository> _TagRepositoryMock;
        private readonly Mock<ILogger<TagsController>> _loggerMock;
        private readonly Mock<IMapper> _mapperMock;

        public TagsControllerTests()
        {
            _TagRepositoryMock = new Mock<ITagRepository>();
            _loggerMock = new Mock<ILogger<TagsController>>();
            _mapperMock = new Mock<IMapper>();
        }

        private TagsController CreateController()
        {
            return new TagsController(
                _TagRepositoryMock.Object,
                _loggerMock.Object,
                _mapperMock.Object
            );
        }

        [Fact]
        public async Task GetTags_ReturnsAllTags()
        {
            // Arrange
            var Tags = new List<Tag>
            {
                new Tag { TagId = Guid.NewGuid(), Name = "Test Tag 1" },
                new Tag { TagId = Guid.NewGuid(), Name = "Test Tag 2" }
            };
            var viewModel = new List<TagViewModel>
            {
                new TagViewModel { TagId = Guid.NewGuid(), Name = "Test Tag 1" },
                new TagViewModel { TagId = Guid.NewGuid(), Name = "Test Tag 2" }
            };

           
            _mapperMock.Setup(mapper => mapper.Map<Tag[], List<TagViewModel>>(Tags.ToArray()))
                .Returns(viewModel);

            _TagRepositoryMock.Setup(c => c.GetTagsAsync()).ReturnsAsync(Tags);
            var controller = CreateController();

            // Act
            var result = await controller.GetTags();

            // Assert
            var viewResult = Assert.IsType<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.Equal(viewModel, okResult?.Value);

        }
        [Fact]
        public async Task Details_WithExistingId_ReturnsTag()
        {
            // Arrange
            Guid id = Guid.NewGuid();
            var Tag = new Tag { TagId = Guid.NewGuid(), Name = "Test Tag" };
            _TagRepositoryMock.Setup(c => c.GetTagByIDAsync(id)).ReturnsAsync(Tag);
            var viewModel = new TagViewModel() { TagId = Guid.NewGuid(), Name = "Test Tag" };
            _mapperMock.Setup(mapper => mapper.Map<Tag, TagViewModel>(Tag))
                .Returns(viewModel);

            var controller = CreateController();

            // Act
            var result = await controller.Details(id);

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.Equal(viewModel, okResult?.Value);
        }
        [Fact]
        public async Task Details_WithNonExistingId_ReturnsNotFoundResult()
        {
            // Arrange
            Guid id = Guid.NewGuid();
            _TagRepositoryMock.Setup(c => c.GetTagByIDAsync(id)).ReturnsAsync((Tag?)null);
            var controller = CreateController();

            // Act
            var result = await controller.Details(id);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }
        [Fact]
        public async Task CreateTag_WithValidTag_ReturnsCreatedAtActionResult()
        {
            // Arrange
            var Tag = new NewTagRequest {   Name = "Test Tag" };
            _mapperMock.Setup(m => m.Map<NewTagRequest, Tag>(It.IsAny<NewTagRequest>()))
                .Returns(new Tag() { Name = Tag.Name });

            _TagRepositoryMock.Setup(c => c.InsertTagAsync(It.IsAny<Tag>())).Returns(Task.CompletedTask);
            _TagRepositoryMock.Setup(c => c.SaveAsync()).Returns(Task.CompletedTask);
            var controller = CreateController();

            // Act
            var result = await controller.CreateTag(Tag);

            // Assert
            Assert.IsType<CreatedAtActionResult>(result.Result);

            var createdAtActionResult = result.Result as CreatedAtActionResult;
            Assert.Equal(nameof(TagsController.Details), createdAtActionResult?.ActionName);
            Assert.IsType<Tag>(createdAtActionResult?.Value);
            Tag? c = createdAtActionResult.Value as Tag;
            Assert.Equal(Tag.Name, c?.Name);
            
        }
        [Fact]
        public async Task CreateTag_WithNullTag_ReturnsBadRequest()
        {
            // Arrange
            var controller = CreateController();

            // Act
            var result = await controller.CreateTag(new NewTagRequest());

            // Assert
            Assert.IsType<BadRequestResult>(result.Result);
        }
        [Fact]
        public async Task UpdateTag_WithValidRequest_ReturnsNoContentResult()
        {
            // Arrange
            var editTagRequest = new EditTagRequest
            {
                TagId = Guid.NewGuid(),
                Name = "Updated Test Tag"
            };

            _mapperMock.Setup(m => m.Map<EditTagRequest, Tag>(It.IsAny<EditTagRequest>()))
                .Returns(new Tag { TagId = editTagRequest.TagId, Name = editTagRequest.Name });

            _TagRepositoryMock.Setup(c => c.TagExistsAsync(editTagRequest.TagId)).ReturnsAsync(true);
            _TagRepositoryMock.Setup(c => c.UpdateTag(It.IsAny<Tag>())).Returns(true);
            _TagRepositoryMock.Setup(c => c.SaveAsync()).Returns(Task.CompletedTask);

            var controller = CreateController();

            // Act
            var result = await controller.UpdateTag(editTagRequest.TagId, editTagRequest);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }
        [Fact]
        public async Task UpdateTag_WithInvalidModelState_ReturnsBadRequest()
        {
            // Arrange
            var editTagRequest = new EditTagRequest
            {
                TagId = Guid.NewGuid(),
                Name = "Updated Test Tag"
            };

            var controller = CreateController();
            controller.ModelState.AddModelError("Content", "Required");

            // Act
            var result = await controller.UpdateTag(editTagRequest.TagId, editTagRequest);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }
        [Fact]
        public async Task UpdateTag_WithNonExistingId_ReturnsNotFoundResult()
        {
            // Arrange
            var editTagRequest = new EditTagRequest
            {
                TagId = Guid.NewGuid(),
                Name = "Updated Test Tag"
            };

            _TagRepositoryMock.Setup(c => c.TagExistsAsync(editTagRequest.TagId)).ReturnsAsync(false);

            var controller = CreateController();

            // Act
            var result = await controller.UpdateTag(editTagRequest.TagId, editTagRequest);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
        [Fact]
        public async Task UpdateTag_WithExceptionThrown_ReturnsInternalServerError()
        {
            // Arrange
            var editTagRequest = new EditTagRequest
            {
                TagId = Guid.NewGuid(),
                Name = "Updated Test Tag"
            };

            _mapperMock.Setup(m => m.Map<EditTagRequest, Tag>(It.IsAny<EditTagRequest>()))
                .Throws(new Exception("Test exception"));

            var controller = CreateController();

            // Act
            var result = await controller.UpdateTag(editTagRequest.TagId, editTagRequest);

            // Assert
            Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, ((ObjectResult)result).StatusCode);
        }
        [Fact]
        public async Task DeleteTag_WithExistingId_ReturnsNoContentResult()
        {
            // Arrange
            Guid id = Guid.NewGuid();
            var Tag = new Tag { TagId = id, Name = "Test Tag" };
            _TagRepositoryMock.Setup(c => c.GetTagByIDAsync(id)).ReturnsAsync(Tag);
            _TagRepositoryMock.Setup(c => c.DeleteTagAsync(id)).Returns(Task.CompletedTask);
            _TagRepositoryMock.Setup(c => c.SaveAsync()).Returns(Task.CompletedTask);

            var controller = CreateController();

            // Act
            var result = await controller.DeleteTag(id);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteTag_WithNonExistingId_ReturnsNotFoundResult()
        {
            // Arrange
            Guid id = Guid.NewGuid();
            _TagRepositoryMock.Setup(c => c.GetTagByIDAsync(id)).ReturnsAsync((Tag?)null);

            var controller = CreateController();

            // Act
            var result = await controller.DeleteTag(id);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteTag_WithExceptionThrown_ReturnsInternalServerError()
        {
            // Arrange
            Guid id = Guid.NewGuid();
            var Tag = new Tag { TagId = id, Name = "Test Tag" };
            _TagRepositoryMock.Setup(c => c.GetTagByIDAsync(id)).ReturnsAsync(Tag);
            _TagRepositoryMock.Setup(c => c.DeleteTagAsync(id)).Throws(new Exception("Test exception"));

            var controller = CreateController();

            // Act
            var result = await controller.DeleteTag(id);

            // Assert
            Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, ((ObjectResult)result).StatusCode);
        }
    }
}


