using HRM.Interfaces;
using HRM.Models;
using HRM.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace HRM.Controllers
{
    public class GratuityCalculateSetupController : Controller
    {
        private readonly IGratuityCalculateSetupService _gratuityCalculateSetupService;
        private readonly IBranchService _branchService;
        private readonly ISalaryHeadsService _salaryHeadsService;
        public GratuityCalculateSetupController(IGratuityCalculateSetupService gratuityCalculateSetupService, IBranchService branchService, ISalaryHeadsService salaryHeadsService)
        {
            _gratuityCalculateSetupService = gratuityCalculateSetupService;
            _branchService = branchService;
            _salaryHeadsService = salaryHeadsService;
        }
        public async Task<IActionResult> Index()
        {
            var branchList = await _branchService.GetAllBranch();
            ViewBag.BranchList = branchList.Select(b => new SelectListItem
            {
                Value = b.Id.ToString(),
                Text = b.Name
            }).ToList();

            var salaryHeadList = await _salaryHeadsService.GetAllSalaryHeads();
            ViewBag.SalaryHeadList = salaryHeadList.Select(s => new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = s.Salaryitems
            }).ToList();

            var gratuityCalculateSetups = await _gratuityCalculateSetupService.GetAllGratuityCalculateSetupAsync();
            return View(gratuityCalculateSetups);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrUpdatePFContri(GratuityCalculateSetup gratuityCalculateSetup)
        {
            if (gratuityCalculateSetup.Id == 0)
            {
                bool isCreated = await _gratuityCalculateSetupService.InsertGratuityCalculateSetupAsync(gratuityCalculateSetup);
                if (!isCreated)
                {
                    TempData["ErrorMessage"] = "A Gratuity Calculate Setup already exists for this SubscriptionId.";
                    return RedirectToAction("Index");
                }
                TempData["SuccessMessage"] = "Gratuity Calculate Setup Created Successfully";
            }
            else
            {
                bool isUpdated = await _gratuityCalculateSetupService.UpdateGratuityCalculateSetupAsync(gratuityCalculateSetup);
                if (!isUpdated)
                {
                    TempData["ErrorMessage"] = "Gratuity Calculate Setup name already exists or update failed";
                    return RedirectToAction("Index");
                }
                TempData["SuccessMessage"] = "Gratuity Calculate Setup Updated Successfully";
            }

            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> DeletePFContri(int id)
        {
            var result = await _gratuityCalculateSetupService.DeleteGratuityCalculateSetupAsync(id);

            TempData["SuccessMessage"] = "Gratuity Calculate Setup deleted successfully.";
            return RedirectToAction("Index");
        }
    }
}
