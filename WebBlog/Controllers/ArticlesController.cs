using AutoMapper;
using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Claims;
using WebBlog.Contracts.Models.Request.Article;
using WebBlog.Contracts.Models.Responce.Article;
using WebBlog.DAL.Interfaces;
using WebBlog.DAL.Models;
using WebBlog.Extensions;


namespace WebBlog.Controllers
{
    /// <summary>
    /// Контроллер для модели Article 
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [Authorize] // Защита контроллера от доступа неавторизованных пользователей
    public class ArticlesController : Controller
    {
        private readonly IArticleRepository _articleRepository;
        private readonly ITagRepository _tagRepository;
        private readonly ILogger<ArticlesController> _logger;
        private readonly IMapper _mapper;
        private readonly UserManager<BlogUser> _userManager;

        public ArticlesController(IArticleRepository articleRepository, ITagRepository tagRepository,
            ILogger<ArticlesController> logger, UserManager<BlogUser> userManager, IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
            _articleRepository = articleRepository;
            _tagRepository = tagRepository;
            _userManager = userManager;
        }

        /// <summary>
        /// Просмотр списка всех статей
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                if (await _articleRepository.GetArticlesAsync() is IEnumerable<Article> articles)
                {
                    List<ArticleViewModel> articleView = _mapper.Map<Article[], List<ArticleViewModel>>(articles.ToArray());
                    return Ok(articleView.ToList());
                }
                return Problem("Entity set 'DbContext.Articles'  is null.");
            }
            catch (Exception ex)
            {
                _logger.CommonError(ex, "Error in Index method");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Просмотр информации о статье с указанным Id 
        /// </summary>
        /// <param name="id"></param>
        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(Guid? id)
        {
            try
            {
                if (id is Guid lId)
                {
                    var article = await _articleRepository.GetArticleByIDAsync(lId);
                    if (article is not null)
                    {
                        var articleView = _mapper.Map<Article, ArticleViewModel>(article);
                        return Ok(articleView);
                    }
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.CommonError(ex, "Error in Details method");
                return StatusCode(500, "Internal server error");
            }
        }
        /// <summary>
        /// Возвращает список статей для указанного автора
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("GetArticleByAuthor/{id}")]
        public async Task<IActionResult> GetArticleByAuthor(string? id)
        {
            try
            {
                if (id is string authorId)
                {
                    var articles = await _articleRepository.GetArticlesByAuthorAsync(authorId);
                    if (articles == null)
                        return NotFound();
                    
                    if(!articles.Any())
                        return NotFound();

                    List<ArticleViewModel> articleView = _mapper.Map<Article[], List<ArticleViewModel>>(articles.ToArray());
                    return Ok(articleView.ToList());
  
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.CommonError(ex, "Error in GetArticleByAuthor method");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Начальные данные для создание новой статьи
        /// </summary>
        [HttpGet("Create")]
        public async Task<IActionResult> Create()
        {
            try
            {
                //передаем в теле Id пользователя для которого будет создана статья

                //список тегов
                var tags = await _tagRepository.GetTagsAsync();
                ViewData["Tags"] = new SelectList(tags, "TagId", "Name");
                return Ok(ViewData);
            }
            catch (Exception ex)
            {
                _logger.CommonError(ex, "Error in Create GET method");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Создает новую статью с указанным именем
        /// </summary>
        /// <param name="reguest"></param>
        [HttpPost("Create")]

#if !SWAGGER
        [ValidateAntiForgeryToken] 
#endif
        public async Task<IActionResult> Create([FromBody] NewArticleRequest? request)
        {
            if (request == null)
                return BadRequest(request); // StatusCode(400, );

            try
            {
                if (ModelState.IsValid)
                {
                    var article = _mapper.Map<NewArticleRequest, Article>(request);
                    //найти пользователя
                    if (await _userManager.FindByIdAsync(article.AuthorId) is not BlogUser user)
                        return BadRequest($"User AuthorId = {article.AuthorId} not found");

                    // сохранение статьи в базу данных
                    foreach (var tag in request.Tags)
                    {
                        // создание нового тега или получение существующего из базы данных
                        var existingTag = await _tagRepository.GetByNameAsync(tag.Name);
                        if (existingTag == null)
                        {
                            existingTag = new Tag() { Name = tag.Name };
                            await _tagRepository.InsertTagAsync(existingTag);
                        }
                        // добавление тега к статье
                        article.Tags ??= new List<Tag>();

                        article.Tags.Add(existingTag);
                    }
                    await _tagRepository.SaveAsync();


                    article.Created = DateTime.Now;
                    await _articleRepository.InsertArticleAsync(article);
                    await _articleRepository.SaveAsync();

                    //добавить пользователю утверждение ArticleOwner
                    var claim = new Claim("ArticleOwner", article.ArticleId.ToString());
                    await _userManager.AddClaimAsync(user, claim);
                    
                    return CreatedAtAction(nameof(Details), new { id = article.ArticleId }, article);


                }
                else
                    return BadRequest(request);
            }
            catch (Exception ex)
            {
                _logger.CommonError(ex, "Error in Create POST method");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Просмотр информации по указзаному в параметре Id тегу для формы редактирования
        /// </summary>
        /// <param name="id"></param>
        [HttpGet("Edit/{id}")]
        [Authorize(Policy = "RuleOwnerOrAdminOrModerator")] 
        public async Task<IActionResult> Edit(Guid? id)
        {
            try
            {
                if (id is Guid lId)
                {
                    var article = await _articleRepository.GetArticleByIDAsync(lId);
                    if (article is not null)
                    {
                        var articleView = _mapper.Map<Article, ArticleViewModel>(article);
                        return Ok(articleView);
                    }
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.CommonError(ex, "Error in Edit method");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Редактирование параметров статьи
        /// </summary>
        /// <param name="id">ID редактируемого тега</param>
        /// <param name="reguest">класс EditTagRequest содержащий имя нового  тега</param>
        [HttpPut("Edit/{id}")]
#if !SWAGGER
        [ValidateAntiForgeryToken] 
#endif
        [Authorize(Policy = "RuleOwnerOrAdminOrModerator")]
        public async Task<IActionResult> Edit(Guid id, [FromBody] EditArticleRequest reguest)
        {
            if (reguest is null)
                return NotFound();

            if (id != reguest.ArticleId)
                return NotFound();

            try
            {
                var article = _mapper.Map<EditArticleRequest, Article>(reguest);
               

                //проверяем прошли ли все значения проверку на корректность
                if (ModelState.IsValid)
                { 
                    if (article is null)
                        throw new ArgumentException(nameof(article));

                    try
                    {

                        if (_articleRepository.UpdateArticle(article))
                            await _articleRepository.SaveAsync();
                        else
                            return NotFound();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!await _articleRepository.ArticleExistsAsync(reguest.ArticleId))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    return Ok(); 
                }
               return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.CommonError(ex, "Error in Edit POST method");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Возвращает данные удаляемой статьи
        /// </summary>
        /// <param name="id"></param>
        /// <param name="saveChangesError"></param>

        [HttpGet("Delete/{id}")]
        [Authorize(Policy = "AccsessOwnerOrAdminOrModerator")]
        public async Task<IActionResult> Delete(Guid? id, bool? saveChangesError = false)
        {
            try
            {
                if (saveChangesError.GetValueOrDefault())
                {
                    ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";
                }

                if (id is Guid lId)
                {
                    var article = await _articleRepository.GetArticleByIDAsync(lId);
                    if (article is not null)
                    {
                        var articleView = _mapper.Map<Article, ArticleViewModel>(article);
                        return Ok(articleView);
                    }
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.CommonError(ex, "Error in Delete method");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Удаляет статью с указанным ид
        /// </summary>
        /// <param name="id">Ид тега для удаления</param>
        
        [HttpDelete("Delete")]
#if !SWAGGER
        [ValidateAntiForgeryToken] 
#endif
        [Authorize(Policy = "RuleOwnerOrAdminOrModerator")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                if (await _articleRepository.GetArticleByIDAsync(id) is Article tag)
                {
                    await _articleRepository.DeleteArticleAsync(id);
                    await _articleRepository.SaveAsync();
                }
            }
            catch (DataException dex)
            {
                _logger.CommonError(dex,$"Delete id {id}failed");
                return RedirectToAction(nameof(Delete), new { id, saveChangesError = true });
            }
            catch (Exception ex)
            {
                _logger.CommonError(ex, "Error in Delete POST method");
                return StatusCode(500, "Internal server error");
            }

            return RedirectToAction(nameof(Index));
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
