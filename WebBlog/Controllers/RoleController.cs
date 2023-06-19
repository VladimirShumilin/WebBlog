using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBlog.Contracts.Models.Request.Role;
using WebBlog.Contracts.Models.Responce.Role;
using WebBlog.Contracts.Models.Responce.Tag;
using WebBlog.DAL.Models;

namespace WebBlog.Controllers
{
    /// <summary>
    /// Контроллер для управления ролями пользователей
    /// </summary>
    [Route("[controller]")]
    [Authorize(Roles = "Administrator")]
    public class RolesController : Controller
    {
        private readonly RoleManager<BlogRole> _roleManager;
        private readonly IMapper _mapper;

        public RolesController(RoleManager<BlogRole> roleManager, IMapper mapper)
        {
            _roleManager = roleManager;
            _mapper = mapper;
        }

        /// <summary>
        /// Возвращает все имеющиеся роли
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var roles = await _roleManager.Roles.ToListAsync(CancellationToken.None);
            var models = _mapper.Map<BlogRole[], List<RoleViewModel>>(roles.ToArray()); 
            return View(models);                
        }
        /// <summary>
        /// Возвращает роль по Id
        /// </summary>
        /// <returns></returns>
        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest();

            if (await _roleManager.FindByIdAsync(id) is BlogRole role)
            {
                var model = _mapper.Map<BlogRole, RoleViewModel>(role);
                return View(model);
            }
            
            return Problem($"Role Id:{id} not found");
        }

        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View();
        }
        /// <summary>
        /// Добавление роли
        /// </summary>   
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<TagViewModel>> CreateRole([Bind("Name,Description")]  NewRoleRequest request)
        {
            if (ModelState.IsValid)
            {
                var role = _mapper.Map<NewRoleRequest, BlogRole>(request);
                var result = await _roleManager.CreateAsync(role);
                if (result.Succeeded)
                    return View("Create",request);
                else
                    return BadRequest(result.Errors.ToArray());
            }
            return BadRequest();
        }

        /// <summary>
        /// GET: NewRoleRequests/Edit/5
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest();

            if (await _roleManager.FindByIdAsync(id) is BlogRole role)
            {
                var model = _mapper.Map<BlogRole, EditRoleRequest>(role);
                return View(model);
            }

            return Problem($"Role Id:{id} not found");
        }
        /// <summary>
        /// Редактирование роли
        /// </summary>    
        [HttpPost("Edit/{id}")]
        public async Task<IActionResult> EditRole([Bind("Id,Name,Description")] EditRoleRequest request)
        {

            if (ModelState.IsValid)
            {
                //var newRole = _mapper.Map<EditRoleRequest, BlogRole>(request);

                if (await _roleManager.FindByIdAsync(request.Id) is BlogRole role)
                {
                    role.Name = request.Name;
                    role.Description = request.Description;
                    var result = await _roleManager.UpdateAsync(role);
                    return RedirectToAction(nameof(Index));
                }
                return BadRequest($"Role Id:{request.Id} not found");
            }
            return BadRequest();
        }

        /// <summary>
        /// GET: NewRoleRequests/Delete/5
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("Delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest();

            if (await _roleManager.FindByIdAsync(id) is BlogRole role)
            {
                var model = _mapper.Map<BlogRole, RoleViewModel>(role);
                return View(model);
            }

            return Problem($"Role Id:{id} not found");
        }

        /// <summary>
        /// Удаление роли
        /// </summary>       
        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest();

            if (await _roleManager.FindByIdAsync(id) is BlogRole role)
            {
                await _roleManager.DeleteAsync(role);
                return RedirectToAction(nameof(Index));
            }
            return BadRequest($"Role Id:{id} not found");
        }
    }
}
