using HRM.Interfaces;
using HRM.Models;
using HRM.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace HRM.Controllers
{
    public class TaxtSlabSetupController : Controller
    {
        private readonly ITaxtSlabSetupService _taxtSlabSetupService;
        private readonly IBranchService _branchService;
        private readonly IDepartmentService _departmentService;
        private readonly IAdditionalInfoService _additionalInfoService;
        public TaxtSlabSetupController(ITaxtSlabSetupService taxtSlabSetupService, IBranchService branchService, IDepartmentService departmentService, IAdditionalInfoService additionalInfoService)
        {
            _taxtSlabSetupService = taxtSlabSetupService;
            _branchService = branchService;
            _departmentService = departmentService;
            _additionalInfoService = additionalInfoService;
        }
        public async Task<IActionResult> Index()
        {
            var branches = await _branchService.GetAllBranch();

            ViewBag.BranchList = branches.Select(b => new SelectListItem
            {
                Value = b.Id.ToString(),
                Text = b.Name
            }).ToList();

            var departments = await _departmentService.GetAllDepartment();
            ViewBag.DepartmentList = departments.Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = d.DepartmentName
            }).ToList();

            var additionalInfos = await _additionalInfoService.GetAllAsync();
            ViewBag.AdditionalInfoList = additionalInfos.Select(a => new SelectListItem
            {
                Value = a.Id.ToString(),
                Text = a.AdditionalInfoName
            }).ToList();


            var taxtSlabSetups =await _taxtSlabSetupService.GetAllTaxtSlabSetupAsync();
            return View(taxtSlabSetups);
        }
        [HttpPost]
        public async Task<IActionResult> CreateOrUpdateTaxSlabSetup(TaxtSlabSetup taxtSlabSetup)
        {
            if (taxtSlabSetup.Id == 0)
            {
                bool isCreated = await _taxtSlabSetupService.InsertTaxtSlabSetupAsync(taxtSlabSetup);
                if (!isCreated)
                {
                    TempData["ErrorMessage"] = "A TaxtSlabSetup already exists for this SubscriptionId.";
                    return RedirectToAction("INdex");
                }
                TempData["SuccessMessage"] = "TaxtSlabSetup Created Successfully";
            }
            else
            {
                bool isUpdated = await _taxtSlabSetupService.UpdateTaxtSlabSetupAsync(taxtSlabSetup);
                if (!isUpdated)
                {
                    TempData["ErrorMessage"] = "TaxtSlabSetup name already exists or update failed";
                    return RedirectToAction("Index");
                }
                TempData["SuccessMessage"] = "TaxtSlabSetup Updated Successfully";
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> DeleteTaxSlabSetup(int id)
        {
            var result = await _taxtSlabSetupService.DeleteTaxtSlabSetupAsync(id);

            TempData["SuccessMessage"] = "TaxSalab Setup deleted successfully.";
            return RedirectToAction("Index");
        }
    }
}
