using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using WebBlog.Contracts.Models.Request.Comment;
using WebBlog.Contracts.Models.Responce.Article;
using WebBlog.Contracts.Models.Responce.Comment;
using WebBlog.Controllers;
using WebBlog.DAL.Interfaces;
using WebBlog.DAL.Models;


namespace WebBlog.Tests.ControllersTests
{
    public class CommentsControllerTests
    {
        private readonly Mock<ICommentRepository> _commentRepositoryMock;
        private readonly Mock<ILogger<CommentsController>> _loggerMock;
        private readonly Mock<IMapper> _mapperMock;

        public CommentsControllerTests()
        {
            _commentRepositoryMock = new Mock<ICommentRepository>();
            _loggerMock = new Mock<ILogger<CommentsController>>();
            _mapperMock = new Mock<IMapper>();
        }

        private CommentsController CreateController()
        {
            return new CommentsController(
                _commentRepositoryMock.Object,
                _loggerMock.Object,
                _mapperMock.Object
            );
        }

        [Fact]
        public async Task GetComments_ReturnsAllComments()
        {
            // Arrange
            var comments = new List<Comment>
            {
                new Comment { CommentId = Guid.NewGuid(), Content = "Test Comment 1" },
                new Comment { CommentId = Guid.NewGuid(), Content = "Test Comment 2" }
            };

            var viewModel = new List<CommentViewModel>
            {
                new CommentViewModel { CommentId = Guid.NewGuid(), Content = "Test Comment 1" },
                new CommentViewModel { CommentId = Guid.NewGuid(), Content = "Test Comment 2" }
            };

            _commentRepositoryMock.Setup(c => c.GetCommentsAsync()).ReturnsAsync(comments);
            _mapperMock.Setup(mapper => mapper.Map<Comment[], List<CommentViewModel>>(comments.ToArray()))
                .Returns(viewModel);

            var controller = CreateController();

            // Act
            var result = await controller.GetComments();

            // Assert
            var viewResult = Assert.IsType<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.Equal(viewModel, okResult?.Value);

        }
        [Fact]
        public async Task Details_WithExistingId_ReturnsComment()
        {
            // Arrange
            Guid id = Guid.NewGuid();
            var comment = new Comment { CommentId = id, Content = "Test Comment" };
            _commentRepositoryMock.Setup(c => c.GetCommentByIDAsync(id)).ReturnsAsync(comment);
            var viewModel = new CommentViewModel() { Title = "Test Title", Content = "Test Content", CommentId = id };
            _mapperMock.Setup(mapper => mapper.Map<Comment, CommentViewModel>(comment))
                .Returns(viewModel);

            var controller = CreateController();

            // Act
            var result = await controller.Details(id);

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            var model = Assert.IsAssignableFrom<CommentViewModel>(okResult?.Value);
            Assert.Equal(viewModel, okResult?.Value);
        }
        [Fact]
        public async Task Details_WithNonExistingId_ReturnsNotFoundResult()
        {
            // Arrange
            Guid id = Guid.NewGuid();
            _commentRepositoryMock.Setup(c => c.GetCommentByIDAsync(id)).ReturnsAsync((Comment?)null);
            var controller = CreateController();

            // Act
            var result = await controller.Details(id);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }
        [Fact]
        public async Task CreateComment_WithValidComment_ReturnsCreatedAtActionResult()
        {
            // Arrange
            var comment = new NewCommentRequest {  Title = "", Content = "Test Comment" };
            _mapperMock.Setup(m => m.Map<NewCommentRequest, Comment>(It.IsAny<NewCommentRequest>()))
                .Returns(new Comment() { Title = comment.Title, Content = comment.Content });

            _commentRepositoryMock.Setup(c => c.InsertCommentAsync(It.IsAny<Comment>())).Returns(Task.CompletedTask);
            _commentRepositoryMock.Setup(c => c.SaveAsync()).Returns(Task.CompletedTask);
            var controller = CreateController();

            // Act
            var result = await controller.CreateComment(comment);

            // Assert
            Assert.IsType<CreatedAtActionResult>(result.Result);

            var createdAtActionResult = result.Result as CreatedAtActionResult;
            Assert.Equal(nameof(CommentsController.Details), createdAtActionResult?.ActionName);
            Assert.IsType<Comment>(createdAtActionResult?.Value);
            Comment? c = createdAtActionResult.Value as Comment;
            Assert.Equal(comment.Content, c?.Content);
            
        }
        [Fact]
        public async Task CreateComment_WithNullComment_ReturnsBadRequest()
        {
            // Arrange
            var controller = CreateController();

            // Act
            var result = await controller.CreateComment(new NewCommentRequest());

            // Assert
            Assert.IsType<BadRequestResult>(result.Result);
        }
        [Fact]
        public async Task UpdateComment_WithValidRequest_ReturnsNoContentResult()
        {
            // Arrange
            var editCommentRequest = new EditCommentRequest
            {
                CommentId = Guid.NewGuid(),
                Content = "Updated Test Comment"
            };

            _mapperMock.Setup(m => m.Map<EditCommentRequest, Comment>(It.IsAny<EditCommentRequest>()))
                .Returns(new Comment { CommentId = editCommentRequest.CommentId, Content = editCommentRequest.Content });

            _commentRepositoryMock.Setup(c => c.CommentExistsAsync(editCommentRequest.CommentId)).ReturnsAsync(true);
            _commentRepositoryMock.Setup(c => c.UpdateComment(It.IsAny<Comment>())).Returns(true);
            _commentRepositoryMock.Setup(c => c.SaveAsync()).Returns(Task.CompletedTask);

            var controller = CreateController();

            // Act
            var result = await controller.UpdateComment(editCommentRequest.CommentId, editCommentRequest);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }
        [Fact]
        public async Task UpdateComment_WithInvalidModelState_ReturnsBadRequest()
        {
            // Arrange
            var editCommentRequest = new EditCommentRequest
            {
                CommentId = Guid.NewGuid(),
                Content = "Updated Test Comment"
            };

            var controller = CreateController();
            controller.ModelState.AddModelError("Content", "Required");

            // Act
            var result = await controller.UpdateComment(editCommentRequest.CommentId, editCommentRequest);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }
        [Fact]
        public async Task UpdateComment_WithNonExistingId_ReturnsNotFoundResult()
        {
            // Arrange
            var editCommentRequest = new EditCommentRequest
            {
                CommentId = Guid.NewGuid(),
                Content = "Updated Test Comment"
            };

            _commentRepositoryMock.Setup(c => c.CommentExistsAsync(editCommentRequest.CommentId)).ReturnsAsync(false);

            var controller = CreateController();

            // Act
            var result = await controller.UpdateComment(editCommentRequest.CommentId, editCommentRequest);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
        [Fact]
        public async Task UpdateComment_WithExceptionThrown_ReturnsInternalServerError()
        {
            // Arrange
            var editCommentRequest = new EditCommentRequest
            {
                CommentId = Guid.NewGuid(),
                Content = "Updated Test Comment"
            };

            _mapperMock.Setup(m => m.Map<EditCommentRequest, Comment>(It.IsAny<EditCommentRequest>()))
                .Throws(new Exception("Test exception"));

            var controller = CreateController();

            // Act
            var result = await controller.UpdateComment(editCommentRequest.CommentId, editCommentRequest);

            // Assert
            Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, ((ObjectResult)result).StatusCode);
        }
        [Fact]
        public async Task DeleteComment_WithExistingId_ReturnsNoContentResult()
        {
            // Arrange
            Guid id = Guid.NewGuid();
            var comment = new Comment { CommentId = id, Content = "Test Comment" };
            _commentRepositoryMock.Setup(c => c.GetCommentByIDAsync(id)).ReturnsAsync(comment);
            _commentRepositoryMock.Setup(c => c.DeleteCommentAsync(id)).Returns(Task.CompletedTask);
            _commentRepositoryMock.Setup(c => c.SaveAsync()).Returns(Task.CompletedTask);

            var controller = CreateController();

            // Act
            var result = await controller.DeleteComment(id);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteComment_WithNonExistingId_ReturnsNotFoundResult()
        {
            // Arrange
            Guid id = Guid.NewGuid();
            _commentRepositoryMock.Setup(c => c.GetCommentByIDAsync(id)).ReturnsAsync((Comment?)null);

            var controller = CreateController();

            // Act
            var result = await controller.DeleteComment(id);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteComment_WithExceptionThrown_ReturnsInternalServerError()
        {
            // Arrange
            Guid id = Guid.NewGuid();
            var comment = new Comment { CommentId = id, Content = "Test Comment" };
            _commentRepositoryMock.Setup(c => c.GetCommentByIDAsync(id)).ReturnsAsync(comment);
            _commentRepositoryMock.Setup(c => c.DeleteCommentAsync(id)).Throws(new Exception("Test exception"));

            var controller = CreateController();

            // Act
            var result = await controller.DeleteComment(id);

            // Assert
            Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, ((ObjectResult)result).StatusCode);
        }
    }
}


