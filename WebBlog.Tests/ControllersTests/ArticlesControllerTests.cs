using AutoMapper;
using BlogWebApp.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Data;
using WebBlog.Contracts.Models.Request.Article;
using WebBlog.Contracts.Models.Request.Tag;
using WebBlog.Contracts.Models.Responce.Article;
using WebBlog.Controllers;
using WebBlog.DAL.Interfaces;
using WebBlog.DAL.Models;


namespace WebBlog.Tests.ControllersTests
{
    public class ArticlesControllerTests
    {
        private readonly Mock<IArticleService> _articleRepositoryMock;
        private readonly Mock<ITagRepository> _tagRepositoryMock;
        private readonly Mock<ILogger<ArticlesController>> _loggerMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<UserManager<BlogUser>> _userManagerMock;

        public ArticlesControllerTests()
        {
            _articleRepositoryMock = new Mock<IArticleService>();
            _tagRepositoryMock = new Mock<ITagRepository>();
            _loggerMock = new Mock<ILogger<ArticlesController>>();
            _mapperMock = new Mock<IMapper>();
            _userManagerMock =  new Mock<UserManager<BlogUser>>(new Mock<IUserStore<BlogUser>>().Object,
            null, null, null, null, null, null, null, null);
        }

        private ArticlesController CreateController()
        {
            return new ArticlesController(
                _articleRepositoryMock.Object,
                _tagRepositoryMock.Object,
                _loggerMock.Object,
                _userManagerMock.Object,
                _mapperMock.Object
            );
        }

        [Fact]
        public async Task Index_ReturnsViewResult_WhenArticlesFound()
        {
            // Arrange
            var controller = CreateController();
            Guid articleId = Guid.NewGuid();
            var articles = new List<Article> { new Article() { Title = "Test Title", Content = "Test Content", ArticleId = articleId } };
            _articleRepositoryMock.Setup(repo => repo.GetArticlesAsync()).ReturnsAsync(articles);
            var viewModel = new List<ArticleViewModel> { new ArticleViewModel() { Title = "Test Title", Content = "Test Content", ArticleId = articleId } };
            _mapperMock.Setup(mapper => mapper.Map<Article[], List<ArticleViewModel>>(articles.ToArray()))
                .Returns(viewModel);

            var result = await controller.Index();
            // Assert
            var viewResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<List<ArticleViewModel>>(viewResult.Value);
            Assert.Single(model);
        }

        [Fact]
        public async Task Details_ReturnsNotFound_WhenIdIsNull()
        {
            // Arrange
            var controller = CreateController();

            // Act
            var result = await controller.Details(null);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Details_ReturnsViewResult_WhenArticleExists()
        {
            // Arrange
            var articleId = Guid.NewGuid();
            var controller = CreateController();
            var article = new Article() { Title = "Test Title", Content = "Test Content", ArticleId = articleId };
            _articleRepositoryMock.Setup(repo => repo.GetArticleByIDAsync(articleId)).ReturnsAsync(article);
            var viewModel = new ArticleViewModel() { Title = "Test Title", Content = "Test Content", ArticleId = articleId };
            _mapperMock.Setup(mapper => mapper.Map<Article, ArticleViewModel>(article))
                .Returns(viewModel);

            // Act
            var result = await controller.Details(articleId);

            // Assert
            var viewResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<ArticleViewModel>(viewResult.Value);
            Assert.Equal(viewModel, model);
        }

        [Fact]
        public async Task Details_ReturnsNotFound_WhenArticleNotFound()
        {
            // Arrange
            var articleId = Guid.NewGuid();
            var controller = CreateController();
            _articleRepositoryMock.Setup(repo => repo.GetArticleByIDAsync(articleId)).ReturnsAsync((Article?)null);
            // Act
            var result = await controller.Details(articleId);
            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetArticlesByAuthor_ReturnsNotFound_WhenIdIsNull()
        {
            // Arrange
            var controller = CreateController();
            // Act
            var result = await controller.GetArticleByAuthor(null);
            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetArticlesByAuthor_ReturnsOkObjectResult_WhenArticlesFound()
        {
            // Arrange
            var controller = CreateController();
            string authorId = "1";
            Guid articleId = Guid.NewGuid();
            var articles = new List<Article> { new Article() { Title = "Test Title", Content = "Test Content", ArticleId = articleId, AuthorId = authorId } };
            _articleRepositoryMock.Setup(repo => repo.GetArticlesByAuthorAsync(authorId)).ReturnsAsync(articles);
            var viewModel = new List<ArticleViewModel> { new ArticleViewModel() { Title = "Test Title", Content = "Test Content", ArticleId = articleId }};
            _mapperMock.Setup(mapper => mapper.Map<Article[], List<ArticleViewModel>>(articles.ToArray()))
                .Returns(viewModel);
            // Act
            var result = await controller.GetArticleByAuthor(authorId);
            // Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<List<ArticleViewModel>>(okObjectResult.Value);
            Assert.Single(model);
        }

        [Fact]
        public async Task GetArticlesByAuthor_ReturnsNotFound_WhenNoArticlesFound()
        {
            // Arrange
            var controller = CreateController();
            string authorId = "1";
            List<Article> emptyList = new();
            _articleRepositoryMock.Setup(repo => repo.GetArticlesByAuthorAsync(authorId)).ReturnsAsync(emptyList);

            // Act
            var result = await controller.GetArticleByAuthor(authorId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Create_ReturnsOkResult_WhenRequestIsValid()
        {
            // Arrange
            var controller = CreateController();
            var newArticleRequest = new NewArticleRequest
            {
                Title = "Test Article",
                Tags = new List<TagRequest> { new() { Name = "tag1" }, new() { Name = "tag2" } }
            }; 
            var userId = Guid.NewGuid().ToString();
            var user = new BlogUser { Id = userId, UserName = "testuser", Email = "testuser@example.com" };

            _tagRepositoryMock.Setup(t => t.InsertTagAsync(It.IsAny<Tag>())).Returns(Task.CompletedTask);
            _mapperMock.Setup(mapper => mapper.Map<NewArticleRequest, Article>(newArticleRequest))
                .Returns(new Article() { Title = "Test Title", Content = "Test Content", ArticleId = Guid.NewGuid() , AuthorId = userId });
            _userManagerMock.Setup(m => m.FindByIdAsync(userId)).ReturnsAsync(user);

            // Act
            var result = await controller.Create(newArticleRequest);

            // Assert
            Assert.IsType<CreatedAtActionResult>(result);
        }

        [Fact]
        public async Task Create_ReturnsInternalServerError_WhenExceptionThrown()
        {
            // Arrange
            var controller = CreateController();
            var newArticleRequest = new NewArticleRequest
            {
                Title = "Test Article",
                Tags = new List<TagRequest> { new() { Name = "tag1" }, new() { Name = "tag2" } }
            };

            _mapperMock.Setup(mapper => mapper.Map<NewArticleRequest, Article>(newArticleRequest))
                .Throws(new Exception());

            // Act
            var result = await controller.Create(newArticleRequest);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

        [Fact]
        public async Task Create_WithValidRequest_ReturnsOkResult()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var newArticleRequest = new NewArticleRequest
            {
                AuthorId = userId,
                Title = "Test Title",
                Content = "Test Content",
                Tags = new List<TagRequest> { new() { Name = "tag1" }, new() { Name = "tag2" } }
            };
            
            var user = new BlogUser { Id = userId, UserName = "testuser", Email = "testuser@example.com" };

            _mapperMock.Setup(m => m.Map<NewArticleRequest, Article>(It.IsAny<NewArticleRequest>()))
                .Returns(new Article() { AuthorId = userId, Title = newArticleRequest.Title, Content = newArticleRequest.Content});

            _tagRepositoryMock.Setup(t => t.GetByNameAsync(It.IsAny<string>())).ReturnsAsync((Tag?)null);
            _tagRepositoryMock.Setup(t => t.InsertTagAsync(It.IsAny<Tag>())).Returns(Task.CompletedTask);
            _tagRepositoryMock.Setup(t => t.SaveAsync()).Returns(Task.CompletedTask);
            _userManagerMock.Setup(m => m.FindByIdAsync(userId)).ReturnsAsync(user);

            var controller = CreateController();

            // Act
            var result = await controller.Create(newArticleRequest);

            // Assert
            Assert.IsType<CreatedAtActionResult>(result);
        }

        [Fact]
        public async Task Create_WithNullRequest_ReturnsBadRequest()
        {
            // Arrange
            var controller = CreateController();
            // Act
            var result = await controller.Create(null);
            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Create_WithExceptionThrown_ReturnsInternalServerError()
        {
            // Arrange
            var newArticleRequest = new NewArticleRequest
            {
                Title = "Test Title",
                Content = "Test Content",
                Tags = new List<TagRequest> { new() { Name = "tag1" }, new() { Name = "tag2" } }
            };

            _mapperMock.Setup(m => m.Map<NewArticleRequest, Article>(It.IsAny<NewArticleRequest>()))
                .Throws(new Exception("Test exception"));

            var controller = CreateController();

            // Act
            var result = await controller.Create(newArticleRequest);

            // Assert
            Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, ((ObjectResult)result).StatusCode);
        }

        [Fact]
        public async Task Edit_ReturnsViewResult_WhenIdIsNotNull()
        {
            // Arrange
            var controller = CreateController();
            Guid articleId = Guid.NewGuid();
            var article = new Article() { Title = "Test Title", Content = "Test Content", ArticleId = articleId, AuthorId = "212" };
            _articleRepositoryMock.Setup(repo => repo.GetArticleByIDAsync(articleId)).ReturnsAsync(article);
            var viewModel = new ArticleViewModel() { Title = "Test Title", Content = "Test Content", ArticleId = articleId };
            _mapperMock.Setup(mapper => mapper.Map<Article, ArticleViewModel>(article))
                .Returns(viewModel);

            // Act
            var result = await controller.Edit(articleId);

            // Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<ArticleViewModel>(okObjectResult.Value);
            Assert.Equal(viewModel, model);
        }

        [Fact]
        public async Task Edit_ReturnsNotFound_WhenIdIsNull()
        {
            //Arrange
            var controller = CreateController();

            //Act
            var result = await controller.Edit(null);

            //Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_ReturnsOk_WhenModelIsValid()
        {
            //Arrange
            var controller = CreateController();
            Guid articleId = Guid.NewGuid();
            var editArticleRequest = new EditArticleRequest { ArticleId = articleId, Title = "NewName" };
            _mapperMock.Setup(mapper => mapper.Map<EditArticleRequest, Article>(editArticleRequest))
                .Returns(new Article() { Title = "Test Title", Content = "Test Content", ArticleId = articleId });

            _articleRepositoryMock.Setup(a => a.UpdateArticle(It.IsAny<Article>())).Returns(true);
            //Act
            var result = await controller.Edit(editArticleRequest.ArticleId, editArticleRequest);

            //Assert
             Assert.IsType<OkResult>(result);
            
        }

        [Fact]
        public async Task Edit_ReturnsNotFound_WhenArticleDoesNotExist()
        {
            // Arrange
            Guid articleId = Guid.NewGuid();
            var controller = CreateController();
            var editArticleRequest = new EditArticleRequest { ArticleId = articleId, Title = "NewName" };
            _mapperMock.Setup(mapper => mapper.Map<EditArticleRequest, Article>(editArticleRequest))
                .Returns(new Article() { Title = "Test Title", Content = "Test Content", ArticleId = articleId });
            // Act
            var result = await controller.Edit(articleId, editArticleRequest);

            //Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_ReturnsStatusCode500_WhenExceptionIsThrown()
        {
            //Arrange
            Guid articleId = Guid.NewGuid();
            var controller = CreateController();
            var editArticleRequest = new EditArticleRequest { ArticleId = articleId, Title = "NewName" };

            //Act
            var result = await controller.Edit(articleId, editArticleRequest);

            //Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

        [Fact]
        public async Task Delete_WithValidId_ReturnsViewResult()
        {
            // Arrange
            Guid articleId = Guid.NewGuid();
            var article = new Article() { Title = "Test Title", Content = "Test Content", ArticleId = articleId , Author = new(),
                Comments = new List<Comment>(), Tags = new List<Tag>() };
            var viewModel = new ArticleViewModel() { Title = "Test Title", Content = "Test Content", ArticleId = articleId };
            _mapperMock.Setup(mapper => mapper.Map<Article, ArticleViewModel>(article))
                .Returns(viewModel);
            _articleRepositoryMock.Setup(a => a.GetArticleByIDAsync(articleId))
               .ReturnsAsync(article);

            var controller = CreateController();

            // Act
            var result = await controller.Delete(articleId);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.Equal(viewModel, ((OkObjectResult)result).Value);
        }

        [Fact]
        public async Task Delete_WithInvalidId_ReturnsNotFoundResult()
        {
            // Arrange
            Guid articleId = Guid.NewGuid();

            _articleRepositoryMock.Setup(a => a.GetArticleByIDAsync(articleId))
                .ReturnsAsync((Article?)null);

            var controller = CreateController();

            // Act
            var result = await controller.Delete(articleId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_WithExceptionThrown_ReturnsInternalServerError()
        {
            // Arrange
            Guid articleId = Guid.NewGuid();

            _articleRepositoryMock.Setup(a => a.GetArticleByIDAsync(articleId))
                .Throws(new Exception("Test exception"));

            var controller = CreateController();

            // Act
            var result = await controller.Delete(articleId);

            // Assert
            Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, ((ObjectResult)result).StatusCode);
        }

        [Fact]
        public async Task DeleteConfirmed_WithValidId_ReturnsRedirectToActionResult()
        {
            // Arrange
            Guid articleId = Guid.NewGuid();
            var article = new Article() { Title = "Test Title", Content = "Test Content", ArticleId = articleId };

            _articleRepositoryMock.Setup(a => a.GetArticleByIDAsync(articleId))
                .ReturnsAsync(article);
            _articleRepositoryMock.Setup(a => a.DeleteArticleAsync(articleId))
                .Returns(Task.CompletedTask);
            _articleRepositoryMock.Setup(a => a.SaveAsync())
                .Returns(Task.CompletedTask);

            var controller = CreateController();

            // Act
            var result = await controller.DeleteConfirmed(articleId);

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", ((RedirectToActionResult)result).ActionName);
        }

        [Fact]
        public async Task DeleteConfirmed_WithDataException_ReturnsRedirectToActionResultWithSaveChangesError()
        {
            // Arrange
            Guid articleId = Guid.NewGuid();
            var article = new Article(){ Title = "Test Title", Content =  "Test Content", ArticleId = articleId };

            _articleRepositoryMock.Setup(a => a.GetArticleByIDAsync(articleId))
                .ReturnsAsync(article);
            _articleRepositoryMock.Setup(a => a.DeleteArticleAsync(articleId))
                .Throws(new DataException());

            var controller = CreateController();

            // Act
            var result = await controller.DeleteConfirmed(articleId);

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Delete", ((RedirectToActionResult)result).ActionName);
            if(result is RedirectToActionResult r)
                Assert.Equal(true, r.RouteValues?["saveChangesError"]);
        }

        [Fact]
        public async Task DeleteConfirmed_WithExceptionThrown_ReturnsInternalServerError()
        {
            // Arrange
            Guid articleId = Guid.NewGuid();
            var article = new Article() { Title = "Test Title", Content = "Test Content", ArticleId = articleId };
            _articleRepositoryMock.Setup(a => a.GetArticleByIDAsync(articleId)).ReturnsAsync(article);
            _articleRepositoryMock.Setup(a => a.DeleteArticleAsync(articleId)).Throws(new Exception("Test exception"));
            var controller = CreateController();

            // Act
            var result = await controller.DeleteConfirmed(articleId);

            // Assert
            Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, ((ObjectResult)result).StatusCode);
        }
    }
}
