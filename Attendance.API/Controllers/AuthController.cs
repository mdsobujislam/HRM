using Attendance.API.Interfaces;
using Attendance.API.Models;
using Attendance.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Attendance.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepo;
        private readonly JwtService _jwtService;

        public AuthController(IUserRepository userRepo, JwtService jwtService)
        {
            _userRepo = userRepo;
            _jwtService = jwtService;
        }

        //[HttpPost("login")]
        //public async Task<IActionResult> Login(Login model)
        //{
        //    var user = await _userRepo.GetLoginUserAsync(model.Email, model.Password);

        //    if (user == null)
        //        return Unauthorized(new { message = "Invalid email or password" });

        //    var token = _jwtService.GenerateToken(user);

        //    return Ok(new
        //    {
        //        token = token,
        //        user = user
        //    });
        //}

    }
}
