using HRM.Interfaces;
using HRM.Models;
using Microsoft.AspNetCore.Mvc;

namespace HRM.Controllers
{
    public class CompaniesController : Controller
    {
        private readonly ICompaniesService _companiesService;
        public CompaniesController(ICompaniesService companiesService)
        {
            _companiesService = companiesService;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var companyList = await _companiesService.GetAllCompanies();
            var company = companyList.FirstOrDefault() ?? new Companies();
            return View(company);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Company(Companies company)
        {
            if (!ModelState.IsValid)
            {
                return View(company);
            }

            try
            {
                if (company.Id == 0)
                {
                    bool isCreated = await _companiesService.InsertCompanies(company);
                    if (!isCreated)
                    {
                        TempData["ErrorMessage"] = "A company already exists for this SubscriptionId.";
                        return View(company);
                    }
                }
                else
                {
                    bool isUpdated = await _companiesService.InsertCompanies(company);
                    if (!isUpdated)
                    {
                        TempData["ErrorMessage"] = "Company name already exists or update failed";
                        return View(company);
                    }
                }
                TempData["SuccessMessage"] = "Company Created Successfully";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred: " + ex.Message);
                return View(company);
            }
        }
    }
}
