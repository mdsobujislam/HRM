using HRM.Interfaces;
using HRM.Models;
using HRM.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace HRM.Controllers
{
    public class SeparationReasonsController : Controller
    {
        private readonly ISeparationReasonsService _separationReasonsService;
        private readonly IBranchService _branchService;
        public SeparationReasonsController(ISeparationReasonsService separationReasonsService, IBranchService branchService)
        {
            _separationReasonsService = separationReasonsService;
            _branchService = branchService;
        }
        public async Task<IActionResult> Index()
        {
            var branchList = await _branchService.GetAllBranch();
            ViewBag.BranchList = branchList.Select(b => new SelectListItem
            {
                Value = b.Id.ToString(),
                Text = b.Name
            }).ToList();

            var separationReasons =await _separationReasonsService.GetAllSeparationReasonsAsync();
            return View(separationReasons);
        }
        [HttpPost]
        public async Task<IActionResult> CreateOrUpdateSeparationReason(SeparationReasons separationReasons)
        {
            if (separationReasons.Id == 0)
            {
                bool isCreated = await _separationReasonsService.CreateSeparationReasonAsync(separationReasons);
                if (!isCreated)
                {
                    TempData["ErrorMessage"] = "A separation Reasons already exists for this SubscriptionId.";
                    return RedirectToAction("Index");
                }
                TempData["SuccessMessage"] = "separation Reasons Created Successfully";
            }
            else
            {
                bool isUpdated = await _separationReasonsService.UpdateSeparationReasonAsync(separationReasons);
                if (!isUpdated)
                {
                    TempData["ErrorMessage"] = "separation Reasons name already exists or update failed";
                    return RedirectToAction("Index");
                }
                TempData["SuccessMessage"] = "separation Reasons Updated Successfully";
            }

            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> DeleteAction(int id)
        {
            var result = await _separationReasonsService.DeleteSeparationReasonAsync(id);

            TempData["SuccessMessage"] = "separation Reasons deleted successfully.";
            return RedirectToAction("Index");
        }
    }
}
