using Microsoft.AspNetCore.Mvc;

namespace HRM.Controllers
{
    public class SalaryHeadsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
