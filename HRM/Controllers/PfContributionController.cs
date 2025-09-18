using HRM.Interfaces;
using HRM.Models;
using HRM.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace HRM.Controllers
{
    public class PfContributionController : Controller
    {
        private readonly IPfContributionService _pfContributionService;
        private readonly IBranchService _branchService;
        private readonly IDesignationService _designationService;
        public PfContributionController(IPfContributionService pfContributionService, IBranchService branchService, IDesignationService designationService)
        {
            _pfContributionService = pfContributionService;
            _branchService = branchService;
            _designationService = designationService;
        }
        public async Task<IActionResult> Index()
        {
            var branches = await _branchService.GetAllBranch();
            ViewBag.BranchList = branches.Select(b => new SelectListItem
            {
                Value = b.Id.ToString(),
                Text = b.Name
            }).ToList();

            var designations = await _designationService.GetAllDesignation();
            ViewBag.DesignationList = designations.Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = d.DesignationName
            }).ToList();

            var pfContributions = await _pfContributionService.GetAllPfContributionsAsync();
            return View(pfContributions);
        }



        [HttpPost]
        public async Task<IActionResult> CreateOrUpdatePFContri(PfContribution pfContribution)
        {
            if (pfContribution.Id == 0)
            {
                bool isCreated = await _pfContributionService.InsertPfContributionasync(pfContribution);
                if (!isCreated)
                {
                    TempData["ErrorMessage"] = "A pfContribution already exists for this SubscriptionId.";
                    return RedirectToAction("Index");
                }
                TempData["SuccessMessage"] = "Pf Contribution Created Successfully";
            }
            else
            {
                bool isUpdated = await _pfContributionService.UpdatePfContributionAsync(pfContribution);
                if (!isUpdated)
                {
                    TempData["ErrorMessage"] = "pfContribution name already exists or update failed";
                    return RedirectToAction("Index");
                }
                TempData["SuccessMessage"] = "Pf Contribution Updated Successfully";
            }

            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> DeletePFContri(int id)
        {
            var result = await _pfContributionService.DeletePfContributionAsync(id);

            TempData["SuccessMessage"] = "PF Contribution deleted successfully.";
            return RedirectToAction("Index");
        }
    }
}
