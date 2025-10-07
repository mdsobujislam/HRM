using HRM.Interfaces;
using HRM.Models;
using HRM.Services;
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

            return View();
        }
        [HttpGet]
        public async Task<JsonResult> SearchEmployees(string term)
        {
            var employees = await _gratuityCalculateService.SearchEmployees(term);
            return Json(employees);
        }
        [HttpPost]
        public async Task<IActionResult> CreateGratuityCalculate(GratuityCalculate gratuityCalculate)
        {
            if (gratuityCalculate.Id == 0)
            {
                bool isCreated = await _gratuityCalculateService.InsertGratuityCalculateAsysnc(gratuityCalculate);
                if (!isCreated)
                {
                    TempData["SuccessMessage"] = "Employee Separation Created Successfully";
                    return RedirectToAction("Index");
                }

            }
            return RedirectToAction("Index");
        }

    }
}
