using HRM.Interfaces;
using HRM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace S_EDex365.Controllers
{
    public class UserController : Controller
    {
        
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly IUserTypeService _userTypeService;
        public UserController(IUserService service, IRoleService roleService, IUserTypeService userTypeService)
        {
            _userService = service ??
                throw new ArgumentNullException(nameof(service));
            _roleService = roleService ??
                throw new ArgumentNullException(nameof(roleService));
            _userTypeService= userTypeService ?? throw new ArgumentNullException(nameof(userTypeService));
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> GetAllUsers()
        {
            var userList = await _userService.GetAllUserAsync();
            return Json(new { Data = userList });
        }
		public async Task<IActionResult> Signup()
		{
			var roleList = await _roleService.GetAllRolesAsync();
            ViewBag.RoleList = roleList.Select(x => new SelectListItem
			{
                Text = x.Name,
                Value = x.Id.ToString()
            });
			return View();
		}
        [HttpPost]
        public async Task<IActionResult> InsertSignUp(User user)
        {
            try
            {
                var success = await _userService.InsertUserAsync(user);
                if (string.IsNullOrEmpty(success))
                {
                    return Json(new { result = false, userId = string.Empty });
                }
                return Json(new { result = true, userId = success });


            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<IActionResult> UpdateUser(Guid userId)
        {
            var user = await _userService.GetUserAsync(userId);
            return PartialView("_UpdateUser", user);
        }
    }
}
