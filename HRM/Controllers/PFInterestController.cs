using HRM.Interfaces;
using HRM.Models;
using HRM.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace HRM.Controllers
{
    public class PFInterestController : Controller
    {
        private readonly IPFInterestService _pFInterestService;
        private readonly IBranchService _branchService;
        public PFInterestController(IPFInterestService pFInterestService, IBranchService branchService)
        {
            _pFInterestService = pFInterestService;
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

            var pFInterests =await _pFInterestService.GetAllPFInterestsAsync();
            return View(pFInterests);
        }
        [HttpPost]
        public async Task<IActionResult> CreateOrUpdatePFContri(PFInterest pFInterest)
        {
            if (pFInterest.Id == 0)
            {
                bool isCreated = await _pFInterestService.InsertPFInterestAsync(pFInterest);
                if (!isCreated)
                {
                    TempData["ErrorMessage"] = "A pFInterest already exists for this SubscriptionId.";
                    return RedirectToAction("Index");
                }
                TempData["SuccessMessage"] = "pF Interest Created Successfully";
            }
            else
            {
                bool isUpdated = await _pFInterestService.UpdatePFInterestAsync(pFInterest);
                if (!isUpdated)
                {
                    TempData["ErrorMessage"] = "pFInterest name already exists or update failed";
                    return RedirectToAction("Index");
                }
                TempData["SuccessMessage"] = "pF Interest Updated Successfully";
            }

            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> DeletePFContri(int id)
        {
            var result = await _pFInterestService.DeletePFInterestAsync(id);

            TempData["SuccessMessage"] = "PF Interest deleted successfully.";
            return RedirectToAction("Index");
        }
    }
}
