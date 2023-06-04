using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBlog.Contracts.Models.Query.User;
using WebBlog.Contracts.Models.Request.User;
using WebBlog.DAL.Models;
using WebBlog.Extensions;

namespace WebBlog.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Roles = "Administrator")]
    public class UserController : ControllerBase
    {
        private readonly UserManager<BlogUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<UserController> _logger;
        private readonly IMapper _mapper;
        public UserController(UserManager<BlogUser> userManager, RoleManager<IdentityRole> roleManager
            , IMapper mapper, ILogger<UserController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Добавляет пользователя 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
#if !SWAGGER
        [ValidateAntiForgeryToken] 
#endif
        [HttpPost]
        public async Task<IActionResult> AddUser([FromBody] NewUserRequest model)
        {
            if (ModelState.IsValid)
            {
                var user = _mapper.Map<NewUserRequest,BlogUser> (model); ;
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    return Ok();
                }
                return BadRequest(result.Errors);
            }
            return BadRequest();
        }

        /// <summary>
        /// Удаляет пользователя
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                return Ok();
            }

            return BadRequest(result.Errors);
        }

        /// <summary>
        /// Возвращает всех пользователей
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                if (await _userManager.Users
                    .Include(u=>u.UserRoles)
                    .Include(u=>u.Claims).ToListAsync() is IEnumerable<BlogUser> users)
                {
                    List<UserViewModel> userView = _mapper.Map<BlogUser[], List<UserViewModel>>(users.ToArray());
                    //foreach (var user in users)
                    //{
                    //    if (await _userManager.GetRolesAsync(user) is List<string> roles)
                    //    {
                    //        ;
                    //    }
                    //}
                    return Ok(userView.ToList());
                }

                _logger.CommonError(null, "_userManager.Users.ToListAsync() is not IEnumerable<BlogUser>");
                return Problem("Internal server error", "", 500);
            }
            catch (Exception ex)
            {
                _logger.CommonError(ex, "Error in GetAllUsers method");
                return Problem("Internal server error","",500);
            }
        }

        /// <summary>
        /// Возвращает пользователя по Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        /// <summary>
        /// Обновляет данные по пользователю с указанным ID
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
#if !SWAGGER
        [ValidateAntiForgeryToken] 
#endif
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserRequest model)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            user.UserName = model.UserName;
            user.Email = model.Email;
            user.CustomField = model.CustomField;
            user.PhoneNumber= model.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return Ok();
            }

            return BadRequest(result.Errors);
        }

        /// <summary>
        /// Добавляет роль по ее имени для пользлователя с указанным Id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roleName"></param>
        /// <returns></returns>
        [HttpPost("{userId}/roles/{roleName}")]
        public async Task<IActionResult> AddRoleToUser(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound($"User id {userId} not found") ;;
            }

            var role = await _roleManager.FindByNameAsync(roleName);

            if (role == null)
            {
                return NotFound($"Role name {roleName} not found");
            }
            if (role.Name is null)
                return NotFound();

            var result = await _userManager.AddToRoleAsync(user, role.Name);

            if (result.Succeeded)
            {
                return Ok();
            }

            return BadRequest(result.Errors);
        }

        /// <summary>
        /// Удаляет казанную роль у пользователя с указанным Id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roleName"></param>
        /// <returns></returns>
        [HttpDelete("{userId}/roles/{roleName}")]
        public async Task<IActionResult> RemoveRoleFromUser(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return NotFound();


            var role = await _roleManager.FindByNameAsync(roleName);

            if (role == null)
                return NotFound();

            if (string.IsNullOrEmpty(role.Name))
                return NotFound();

            var result = await _userManager.RemoveFromRoleAsync(user, role.Name);

            if (result.Succeeded)
                return Ok();


            return BadRequest(result.Errors);
        }

        /// <summary>
        /// Возвращает список ролей для пользователя
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("{userId}/roles")]
        public async Task<IActionResult> GetRolesForUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();


            var role = await _userManager.GetRolesAsync(user);
            if (role == null)
                return NotFound();

                return Ok(role);
        }
    }
}
