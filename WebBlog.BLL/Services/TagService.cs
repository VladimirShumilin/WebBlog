using AutoMapper;
using Azure.Core;
using Microsoft.Extensions.Logging;
using WebBlog.BLL.Services.Interfaces;
using WebBlog.Contracts.Models.Request.Tag;
using WebBlog.Contracts.Models.Responce.Tag;
using WebBlog.DAL.Interfaces;
using WebBlog.DAL.Models;

namespace WebBlog.BLL.Services
{
    /// <summary>
    ///  
    /// </summary>
    public class TagService : ITagService
    {
        private readonly ITagRepository _tagsRepository;
        private readonly IArticleRepository _articlesRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<TagService> _logger;
        
        public TagService(ITagRepository tagsRepository, IArticleRepository blogArticlesRepository, IMapper mapper, ILogger<TagService> logger)
        {
            _tagsRepository = tagsRepository;
            _articlesRepository = blogArticlesRepository;
            _mapper = mapper;
            _logger = logger;
        }
        /// <summary>
        /// Добавляет новый Тег в бд 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<Tag> InsertTagAsync(NewTagRequest request)
        {

            var newTag = _mapper.Map<NewTagRequest, Tag>(request);
            //проверить на налицие уже тега с таким именем
            if (await _tagsRepository.TagExistsAsync(newTag.Name))
                throw new ArgumentException($"Тег '{request.Name}' уже присутствует в БД. ", nameof(request.Name));

            await _tagsRepository.InsertTagAsync(newTag);
            await _tagsRepository.SaveAsync();

            _logger.LogInformation($"Тег '{newTag.Name}' добавлен.");
            return newTag;
            
        }
        /// <summary>
        /// Обновляет параметры указанного тега
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<Tag> UpdateTagAsync(EditTagRequest request)
        {
            var tag = _mapper.Map<EditTagRequest, Tag>(request);
            //проверить на налицие уже тега с таким именем
            if (!await _tagsRepository.TagExistsAsync(tag.TagId))
                throw new ArgumentException($"Тег '{request.Name}' отсутствует в БД", nameof(request.Name));

            if (!_tagsRepository.UpdateTag(tag))
                throw new Exception("Обновление данных завершилось с ошибкой");

            await _tagsRepository.SaveAsync();

            _logger.LogInformation($"Тег '{tag.Name}' добавлен.");
            return tag;

        }

        public async Task<bool> DeleteTagAsync(Guid tagId)
        {
            try
            {
                var Tag = await _tagsRepository.GetTagByIDAsync(tagId);

                if (Tag == null)
                    return false;

                await _tagsRepository.DeleteTagAsync(tagId);
                await _tagsRepository.SaveAsync();

                _logger.LogInformation($"Тег '{tagId}' удален.");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении тега.");
                return false;
            }            
        }

        public Task<Tag?> GetTagByIDAsync(Guid id)
        {
            return _tagsRepository.GetTagByIDAsync(id);
        }
        public Task<Tag?> GetByNameAsync(string name)
        {
            return _tagsRepository.GetByNameAsync(name);
        }

        public Task<IEnumerable<Tag>> GetTagsAsync()
        {
            return _tagsRepository.GetTagsIncludeArticles();
        }

        public Task<IEnumerable<Tag>> GetAllIncludeBlogArticles()
        {
            return _tagsRepository.GetTagsIncludeArticles();
        }

        //public async Task<TagViewModel> GetListTagsViewModel()
        //{
        //    var tags = (await GetAllIncludeBlogArticles()).OrderBy(o => o.Name).ToList();

        //    var viewModel = _mapper.Map<Tag[], List<TagViewModel>>(tags.ToArray());
        //    return viewModel;
        //}

        //public async Task<TagViewModel> GetListTagsViewModelForUser(string userId) 
        //{
        //    var tags = (await GetAllIncludeBlogArticles()).OrderBy(o => o.Name).ToList();
            
        //    var model = new ListTagsViewModel(tags, user: user);
        //    return model;
        //}
    }
}
