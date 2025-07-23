using HRM.Models;
using HRM.Repository;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ServiceManagementSystem.Controllers
{
    public class AuthController : Controller
    {
        private readonly IUserService _userService;
        public AuthController(IUserService service)
        {
            _userService = service;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(User user)
        {
            if (ModelState.IsValid)
            {
                var d = _userService.InsertUserAsync(user);
                return RedirectToAction("Login", "Auth");
            }
            return RedirectToAction("Register", "Auth");
        }



        [Route("")]
        [Route("Login")]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }
        [Route("")]
        [Route("Login")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Login(Login login)
        {
            if (ModelState.IsValid)
            {
                var user = await _userService.GetLoginUserAsync(login.Email, login.Password);       
                if (user!=null && user.Email.Length>0)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.Name),
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new Claim("SubscriptionId", user.SubscriptionId.ToString())
                    };
                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                    TempData["LoginSuccess"] = "Successfully Login";
                    return RedirectToAction("Dashboard", "Home");
                }
                else
                {
                    TempData["LoginSuccess"] = "Invalid User";
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(login);
                }           
            }
            TempData["LoginSuccess"] = "Invalid User";
            return View(login);
        }
        [Authorize]
        public IActionResult Logout()
        {
            var login = HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["LoginSuccess"] = "Successfully LogOut";
            return RedirectToAction("Login");
        }
    }

}
