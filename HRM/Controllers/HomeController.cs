using System.Diagnostics;
using System.Threading.Tasks;
using HRM.Interfaces;
using HRM.Models;
using Microsoft.AspNetCore.Mvc;

namespace HRM.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICompaniesService _companiesService;

        public HomeController(ILogger<HomeController> logger, ICompaniesService companiesService)
        {
            _logger = logger;
            _companiesService = companiesService;
        }

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Dashboard()
        {
            var companyList = await _companiesService.GetAllCompanies();
            var company = companyList.FirstOrDefault() ?? new Companies();
            return View(company);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
