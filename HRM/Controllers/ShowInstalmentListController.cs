using HRM.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HRM.Controllers
{
    public class ShowInstalmentListController : Controller
    {
        private readonly IShowInstalmentListService _showInstalmentListService;
        public ShowInstalmentListController(IShowInstalmentListService showInstalmentListService)
        {
            _showInstalmentListService = showInstalmentListService;
        }
        public async Task<IActionResult> Index()
        {
            var details = await _showInstalmentListService.GetAllShowInstalmentListAsync();
            return View(details);
        }


    }
}
