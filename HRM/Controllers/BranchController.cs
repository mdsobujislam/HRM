using HRM.Interfaces;
using HRM.Models;
using HRM.Services;
using Microsoft.AspNetCore.Mvc;

namespace HRM.Controllers
{
    public class BranchController : Controller
    {
        private readonly IBranchService _branchService;
        public BranchController(IBranchService branchService)
        {
            this._branchService = branchService;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var brnach=await _branchService.GetAllBranch();
            return View(brnach);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> InserBranch(Branch branch)
        {
            try
            {
                if (branch.Id == 0)
                {
                    bool isCreated = await _branchService.InsertBranch(branch);
                    if (!isCreated)
                    {
                        TempData["ErrorMessage"] = "A Branch already exists for this SubscriptionId.";
                        return View(branch);
                    }
                }
                else
                {
                    bool isUpdated = await _branchService.InsertBranch(branch);
                    if (!isUpdated)
                    {
                        TempData["ErrorMessage"] = "Branch name already exists or update failed";
                        return View(branch);
                    }
                }
                TempData["SuccessMessage"] = "Branch Created Successfully";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred: " + ex.Message);
                return View(branch);
            }
        }
        public async Task<IActionResult> Update()
        {
            return View();
        }
    }
}
