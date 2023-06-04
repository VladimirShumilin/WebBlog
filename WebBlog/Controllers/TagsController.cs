using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Xml.Linq;
using WebBlog.Contracts.Models.Request.Tag;
using WebBlog.Contracts.Models.Responce.Comment;
using WebBlog.Contracts.Models.Responce.Tag;
using WebBlog.DAL.Interfaces;
using WebBlog.DAL.Models;
using WebBlog.DAL.Repositories;
using WebBlog.Extensions;

namespace WebBlog.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize] // Защита контроллера от доступа неавторизованных пользователей
    public class TagsController : Controller
    {
      
        private readonly IMapper _mapper;
        private readonly ITagRepository _TagRepository;
        private readonly ILogger<TagsController> _logger;

        public TagsController(ITagRepository TagRepository, ILogger<TagsController> logger
            , IMapper mapper)
        {
            _TagRepository = TagRepository;
            _logger = logger;
            _mapper = mapper;
        }

        
        /// <summary>
        /// возвращает все коментарии
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetTags")]
        public async Task<ActionResult<IEnumerable<TagViewModel>>> GetTags()
        {
            try
            {
                var Tags = await _TagRepository.GetTagsAsync();
                var viewModel = _mapper.Map<Tag[], List<TagViewModel>>(Tags.ToArray());
                return Ok(viewModel);
            }
            catch (Exception ex)
            {
                _logger.CommonError(ex, "Error in GetTags method");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Возвращает коментарий по Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("Details/{id}")]
        public async Task<ActionResult<TagViewModel>> Details(Guid id)
        {
            try
            {
                var Tag = await _TagRepository.GetTagByIDAsync(id);

                if (Tag == null)
                {
                    return NotFound();
                }

                var viewModel = _mapper.Map<Tag, TagViewModel>(Tag);
                return Ok(viewModel);
            }
            catch (Exception ex)
            {
                _logger.CommonError(ex, "Error in Details method");
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
        public async Task<ActionResult<TagViewModel>> CreateTag([FromBody] NewTagRequest request)
        {
            if (request == null)
            {
                return BadRequest("Tag is null");
            }

            try
            {
                if (ModelState.IsValid)
                {
                    var Tag = _mapper.Map<NewTagRequest, Tag>(request);
                    if(Tag is null)
                        return BadRequest();

                    await _TagRepository.InsertTagAsync(Tag);
                    await _TagRepository.SaveAsync();
                    return CreatedAtAction(nameof(Details), new { id = Tag.TagId }, Tag);
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
        /// <param name="Tag"></param>
        /// <returns></returns>
        [HttpPut("Edit/{id}")]
#if !SWAGGER
        [ValidateAntiForgeryToken] 
#endif
        public async Task<IActionResult> UpdateTag(Guid id, [FromBody] EditTagRequest request)
        {
            if (id != request.TagId)
            {
                return BadRequest("Tag ID mismatch");
            }

            try
            {
                if (ModelState.IsValid)
                {
                    var Tag = _mapper.Map<EditTagRequest, Tag>(request);
                    if (!await _TagRepository.TagExistsAsync(id))
                    {
                        return NotFound();
                    }

                    bool isUpdated = _TagRepository.UpdateTag(Tag);

                    if (!isUpdated)
                    {
                        return BadRequest("Update failed");
                    }

                    await _TagRepository.SaveAsync();
                    return NoContent();
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.CommonError(ex, "Error in UpdateTag method");
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
#endif
        public async Task<IActionResult> DeleteTag(Guid id)
        {
            try
            {
                var Tag = await _TagRepository.GetTagByIDAsync(id);

                if (Tag == null)
                {
                    return NotFound();
                }

                await _TagRepository.DeleteTagAsync(id);
                await _TagRepository.SaveAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.CommonError(ex, "Error in DeleteTag method");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
