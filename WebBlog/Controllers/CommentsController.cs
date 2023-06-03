using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;
using WebBlog.Contracts.Models.Request.Comment;
using WebBlog.Contracts.Models.Responce.Article;
using WebBlog.Contracts.Models.Responce.Comment;
using WebBlog.DAL.Interfaces;
using WebBlog.DAL.Models;
using WebBlog.Extensions;

namespace WebBlog.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CommentsController : Controller
    {
      
        private readonly IMapper _mapper;
        private readonly ICommentRepository _commentRepository;
        private readonly ILogger<CommentsController> _logger;

        public CommentsController(ICommentRepository commentRepository, ILogger<CommentsController> logger
            , IMapper mapper)
        {
            _commentRepository = commentRepository;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// возвращает все коментарии
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetComments")]
        public async Task<ActionResult<IEnumerable<CommentViewModel>>> GetComments()
        {
            try
            {
                var comments = await _commentRepository.GetCommentsAsync();
                var viewModel = _mapper.Map<Comment[], List<CommentViewModel>>(comments.ToArray());
                return Ok(viewModel);
            }
            catch (Exception ex)
            {
                _logger.CommonError(ex, "Error in GetComments method");
                return StatusCode(500, "Internal server error");
            }
        }
        /// <summary>
        /// Возвращает все коментарии для статью с указанным Ид
        /// </summary>
        /// <param name="articleId"></param>
        /// <returns></returns>
        [HttpGet("GetCommentsForTheArticle")]
        public async Task<ActionResult<IEnumerable<CommentViewModel>>> GetCommentsForTheArticle(Guid articleId)
        {
            try
            {
                var comments = await _commentRepository.GetCommentsForTheArticleAsync(articleId);
                var viewModel = _mapper.Map<Comment[], List<CommentViewModel>>(comments.ToArray());
                return Ok(viewModel);
            }
            catch (Exception ex)
            {
                _logger.CommonError(ex, "Error in GetComments method");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Возвращает коментарий по Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("Details/{id}")]
        public async Task<ActionResult<CommentViewModel>> Details(Guid id)
        {
            try
            {
                var comment = await _commentRepository.GetCommentByIDAsync(id);

                if (comment == null)
                {
                    return NotFound();
                }

                var viewModel = _mapper.Map<Comment, CommentViewModel>(comment);
                return Ok(viewModel);
            }
            catch (Exception ex)
            {
                _logger.CommonError(ex, "Error in GetComment method");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Создает новый коментарий
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("Create")]
#if !SWAGGER
        [ValidateAntiForgeryToken] 
#endif
        public async Task<ActionResult<CommentViewModel>> CreateComment([FromBody] NewCommentRequest request)
        {
            if (request == null)
            {
                return BadRequest("Comment is null");
            }

            try
            {
                if (ModelState.IsValid)
                {
                    var comment = _mapper.Map<NewCommentRequest, Comment>(request);
                    if(comment is null)
                        return BadRequest();

                    await _commentRepository.InsertCommentAsync(comment);
                    await _commentRepository.SaveAsync();
                    return CreatedAtAction(nameof(Details), new { id = comment.CommentId }, comment);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.CommonError(ex, "Error in Details method");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Обновляет информцию переданного коментария
        /// </summary>
        /// <param name="id"></param>
        /// <param name="comment"></param>
        /// <returns></returns>
        [HttpPut("Edit/{id}")]
#if !SWAGGER
        [ValidateAntiForgeryToken] 
#endif
        public async Task<IActionResult> UpdateComment(Guid id, [FromBody] EditCommentRequest request)
        {
            if (id != request.CommentId)
            {
                return BadRequest("Comment ID mismatch");
            }

            try
            {
                if (ModelState.IsValid)
                {
                    var comment = _mapper.Map<EditCommentRequest, Comment>(request);
                    if (!await _commentRepository.CommentExistsAsync(id))
                    {
                        return NotFound();
                    }

                    bool isUpdated = _commentRepository.UpdateComment(comment);

                    if (!isUpdated)
                    {
                        return BadRequest("Update failed");
                    }

                    await _commentRepository.SaveAsync();
                    return NoContent();
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.CommonError(ex, "Error in UpdateComment method");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Удаляет комментарий
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("Delete{id}")]
#if !SWAGGER
        [ValidateAntiForgeryToken] 
        [Authorize(Roles = "Administrator")]
#endif
        
        public async Task<IActionResult> DeleteComment(Guid id)
        {
            try
            {
                var comment = await _commentRepository.GetCommentByIDAsync(id);

                if (comment == null)
                {
                    return NotFound();
                }

                await _commentRepository.DeleteCommentAsync(id);
                await _commentRepository.SaveAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.CommonError(ex, "Error in DeleteComment method");
                return StatusCode(500, "Internal server error");
            }
        }



    }
}
