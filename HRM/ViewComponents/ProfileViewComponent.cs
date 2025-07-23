using HRM.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HRM.ViewComponents
{
    [ViewComponent(Name = "Profile")]
    public class ProfileViewComponent : ViewComponent
    {
        private readonly IUserService _userService;
        public ProfileViewComponent(IUserService userService)
        {
            _userService = userService;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userService.GetUserAsync(int.Parse(userId));
            return View("Profile", user);
        }
    }
}
