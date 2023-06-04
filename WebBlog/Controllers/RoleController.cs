using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBlog.Contracts.Models.Request.Role;
using WebBlog.DAL.Models;

namespace WebBlog.Controllers
{
    /// <summary>
    /// Контроллер для управления ролями пользователей
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [Authorize(Roles = "Administrator")]
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;

        public RoleController(RoleManager<IdentityRole> roleManager, IMapper mapper)
        {
            _roleManager = roleManager;
            _mapper = mapper;
        }

        /// <summary>
        /// Возвращает все имеющиеся роли
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<IdentityRole>> GetRoles()
        {
            var roles = await _roleManager.Roles.ToListAsync(CancellationToken.None);
            return roles;
        }
        /// <summary>
        /// Возвращает роль по Id
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRole(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest();

            if (await _roleManager.FindByIdAsync(id) is IdentityRole role)
                return Ok(role);
            
            return BadRequest($"Role Id:{id} not found");
        }

        /// <summary>
        /// Добавление роли
        /// </summary>   
        [HttpPost]
        public async Task<IActionResult> CreateRole([FromBody] NewRoleRequest request)
        {
            if (ModelState.IsValid)
            {
                var role = _mapper.Map<NewRoleRequest, IdentityRole>(request);
                var result = await _roleManager.CreateAsync(role);
                if (result.Succeeded)
                    return Ok();
                else
                    return BadRequest(result.Errors.ToArray());
            }
            return BadRequest();
        }

        /// <summary>
        /// Редактирование роли
        /// </summary>    
        [HttpPut("{id}")]
        public async Task<IActionResult> EditRole([FromBody] EditRoleRequest request)
        {

            if (ModelState.IsValid)
            {
                if (await _roleManager.FindByIdAsync(request.Id) is IdentityRole role)
                {
                    await _roleManager.UpdateAsync(role);
                    return Ok();
                }
                return BadRequest($"Role Id:{request.Id} not found");
            }
            return BadRequest();
        }

        /// <summary>
        /// Удаление роли
        /// </summary>       
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest();

            if (await _roleManager.FindByIdAsync(id) is IdentityRole role)
            {
                await _roleManager.DeleteAsync(role);
                return Ok();
            }
            return BadRequest($"Role Id:{id} not found");
        }
    }
}
