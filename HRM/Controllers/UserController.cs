using HRM.Authorization;
using HRM.Models;
using HRM.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ServiceManagementSystem.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        public UserController(IUserService service,IRoleService roleService)
        {
            _userService = service ??
                throw new ArgumentNullException(nameof(service));
            _roleService = roleService ??
                throw new ArgumentNullException(nameof(roleService));
        }
        [AuthorizePermission("User")]
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetAllUsers()
        {
            var userList = await _userService.GetAllUserAsync();
            return Json(new { Data = userList });
        }

        public async Task<IActionResult> AddUser()
        {
            var roleList = await _roleService.GetAllRolesAsync();
            ViewBag.RoleList = roleList.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            });
            return PartialView("_AddUser");
        }

        [HttpPost]
        public async Task<IActionResult> InsertUser(User user)
        {
            try
            {
                var success = await _userService.InsertUserAsync(user);
                return Json(new { result = success });
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<IActionResult> UpdateUser(int userId)
        {
            var roleList = await _roleService.GetAllRolesAsync();
            ViewBag.RoleList = roleList.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            });

            var user = await _userService.GetUserAsync(userId);
            return PartialView("_UpdateUser", user);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUserPartail(User user)
        {
            try
            {
                var success = await _userService.UpdateUserAsync(user);
                return Json(new { result = success });
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        [HttpPost]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            try
            {
                var result = await _userService.DeleteUserAsync(userId);
                return Json(new { result });
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
