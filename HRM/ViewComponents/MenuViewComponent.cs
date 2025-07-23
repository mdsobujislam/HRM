using HRM.Models;
using HRM.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HRM.ViewComponents
{
    [ViewComponent(Name = "Menu")]
    public class MenuViewComponent : ViewComponent
    {
        private readonly IRoleService _roleService;
        private readonly IUserService _userService;
        public MenuViewComponent(IRoleService roleService, IUserService userService)
        {
            _roleService = roleService;
            _userService = userService;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = await _userService.GetUserRolesAsync(userId);
            List<Menu> menuList = new List<Menu>();

            var roleName = new List<string>();
            foreach (var item in userRole)
            {
                int result = int.Parse(item);
                var name = await _roleService.GetRoleByIdAsync(result);
                roleName.Add(name.Name);
            }

            foreach (var item in userRole)
            {

                var permitedMenu = await _roleService.GetAllMenusAsync(int.Parse(item));
                if (roleName.Contains("Super Admin"))
                {
                    menuList.AddRange(permitedMenu);
                    break;
                }
                foreach (var menu in permitedMenu)
                {
                    if (menu.Status == true)
                    {
                        menuList.Add(menu);
                    }
                }
            }
            return View("Menu", menuList);
        }
    }
}
