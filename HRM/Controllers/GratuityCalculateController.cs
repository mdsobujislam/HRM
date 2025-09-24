using HRM.Interfaces;
using HRM.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HRM.Controllers
{
    public class GratuityCalculateController : Controller
    {
        private readonly IGratuityCalculateService _gratuityCalculateService;
        private readonly IBranchService _branchService;
        public GratuityCalculateController(IGratuityCalculateService gratuityCalculateService, IBranchService branchService)
        {
            _gratuityCalculateService = gratuityCalculateService;
            _branchService = branchService;
        }
        public async Task<IActionResult> Index(ShowGratuity showGratuity)
        {
            var branches =await _branchService.GetAllBranch();
            ViewBag.BranchList = branches.Select(b => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Value = b.Id.ToString(),
                Text = b.Name
            }).ToList();

            var gratuityCalculates =await _gratuityCalculateService.GetAllShowGratuityAsync(showGratuity);
            return View(gratuityCalculates);
        }
    }
}
