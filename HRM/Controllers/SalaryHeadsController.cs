using HRM.Interfaces;
using HRM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HRM.Controllers
{
    public class SalaryHeadsController : Controller
    {
        private readonly ISalaryHeadsService _salaryHeadsService;
        private readonly IBranchService _branchService;

        public SalaryHeadsController(ISalaryHeadsService salaryHeadsService, IBranchService branchService)
        {
            _salaryHeadsService = salaryHeadsService;
            _branchService = branchService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var branchList = await _branchService.GetAllBranch();
            ViewBag.BranchList = branchList.Select(b => new SelectListItem
            {
                Value = b.Id.ToString(),
                Text = b.Name
            }).ToList();

            var salaryHeadsList = await _salaryHeadsService.GetAllSalaryHeads();
            return View(salaryHeadsList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrUpdate(SalaryHeads salaryHeads)
        {
            try
            {
                if (salaryHeads.Id == 0)
                {
                    await _salaryHeadsService.InsertSalaryHeads(salaryHeads);
                    TempData["SuccessMessage"] = "Salary Head created successfully.";
                }
                else
                {
                    await _salaryHeadsService.UpdateSalaryHeads(salaryHeads);
                    TempData["SuccessMessage"] = "Salary Head updated successfully.";
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _salaryHeadsService.DeleteSalaryHeads(id);
            TempData[result ? "SuccessMessage" : "ErrorMessage"] =
                result ? "Salary Head deleted successfully." : "Failed to delete Salary Head.";

            return RedirectToAction("Index");
        }
    }
}
