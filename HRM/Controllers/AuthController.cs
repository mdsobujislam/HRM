using HRM.Interfaces;
using HRM.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HRM.Controllers
{
    public class AuthController : Controller
    {
        private readonly IUserService _userService;
        public AuthController(IUserService userService)         
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        //[Route("")]
        //[Route("Login")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Login(Login login,User user)
        {
            if (ModelState.IsValid)
            {
                var userType = await _userService.GetUserTypeCheckAsync(login.Email);

                if (userType.Name== "Student")
                {
                    var studentSser = await _userService.GetLoginUserAsync(login.Email, login.Password);
                    if (studentSser != null && studentSser.Email.Length > 0)
                    {
                        var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name,studentSser.Name),
                    new Claim(ClaimTypes.NameIdentifier,studentSser.Id.ToString()),
                };
                        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var principal = new ClaimsPrincipal(identity);
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                        TempData["LoginSuccess"] = "Successfully Login";
                        return RedirectToAction("Index", "StudentDashboard");
                    }
                    else
                    {
                        TempData["LoginSuccess"] = "Invalid User";
                        ModelState.AddModelError(string.Empty, "Invalied Login attempt");
                        return View(login);
                    }
                }
                else if(userType.Name == "Teacher")
                {
                    var studentSser = await _userService.GetLoginUserAsync(login.Email, login.Password);
                    if (studentSser != null && studentSser.Email.Length > 0)
                    {
                        var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name,studentSser.Name),
                    new Claim(ClaimTypes.NameIdentifier,studentSser.Id.ToString()),
                };
                        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var principal = new ClaimsPrincipal(identity);
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                        TempData["LoginSuccess"] = "Successfully Login";
                        return RedirectToAction("Index", "TeacherDashboard");
                    }
                    else
                    {
                        TempData["LoginSuccess"] = "Invalid User";
                        ModelState.AddModelError(string.Empty, "Invalied Login attempt");
                        return View(login);
                    }
                }
                else
                {
                    var studentSser = await _userService.GetLoginUserAsync(login.Email, login.Password);
                    if (studentSser != null && studentSser.Email.Length > 0)
                    {
                        var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name,studentSser.Name),
                    new Claim(ClaimTypes.NameIdentifier,studentSser.Id.ToString()),
                };
                        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var principal = new ClaimsPrincipal(identity);
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                        TempData["LoginSuccess"] = "Successfully Login";
                        return RedirectToAction("Index", "AdminDashboard");
                    }
                    else
                    {
                        TempData["LoginSuccess"] = "Invalid User";
                        ModelState.AddModelError(string.Empty, "Invalied Login attempt");
                        return View(login);
                    }
                }

                
            }
            TempData["LoginSuccess"] = "Invalid User";
            return View(login);
        }
        public IActionResult Logout()
        {
            var login = HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["LoginSuccess"] = "Successfully LogOut";
            return RedirectToAction("Index","Home");
        }
    }

}
