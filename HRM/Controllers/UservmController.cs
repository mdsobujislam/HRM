using HRM.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace S_EDex365.Controllers
{
    public class UservmController : Controller
    {
        private readonly IUserVMService _uservmservice;
        public UservmController(IUserVMService uservmservice)
        {
            _uservmservice = uservmservice ??
                throw new ArgumentNullException(nameof(uservmservice));
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> GetAllUsers()
        {
            var userList = await _uservmservice.GetAllUserAsync();
            return Json(new { Data = userList });
        }
    }
}
