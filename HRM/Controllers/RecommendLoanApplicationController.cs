using HRM.Interfaces;
using HRM.Models;
using HRM.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace HRM.Controllers
{
    public class RecommendLoanApplicationController : Controller
    {
        private readonly IRecommendLoanApplicationService _recommendLoanApplicationService;
        public RecommendLoanApplicationController(IRecommendLoanApplicationService recommendLoanApplicationService)
        {
            _recommendLoanApplicationService = recommendLoanApplicationService ?? throw new ArgumentNullException(nameof(recommendLoanApplicationService));
        }
        public async Task<IActionResult> Index(RecommendLoanApplication recommendLoanApplication)
        {

            if (string.IsNullOrEmpty(recommendLoanApplication.FromDate))
                recommendLoanApplication.FromDate = DateTime.Today.ToString("yyyy-MM-dd");

            if (string.IsNullOrEmpty(recommendLoanApplication.ToDate))
                recommendLoanApplication.ToDate = DateTime.Today.ToString("yyyy-MM-dd");

            // Call service to fetch data
            var recommendLoanApplicationList = await _recommendLoanApplicationService.SearchData(recommendLoanApplication);

            // If no data, return empty list of the correct type
            if (recommendLoanApplicationList == null || !recommendLoanApplicationList.Any())
            {
                return View(new List<RecommendLoanApplication>());
            }

            return View(recommendLoanApplicationList);
        }


        public async Task<IActionResult> ShowSearchData(RecommendLoanApplication recommendLoanApplication)
        {
            var recommendLoanApplicationList = await _recommendLoanApplicationService.SearchData(recommendLoanApplication);

            // ✅ If no data, still return the View with empty list
            if (recommendLoanApplicationList == null || !recommendLoanApplicationList.Any())
            {
                return View(new List<BonusCalculate>());
            }

            return View(recommendLoanApplicationList);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(RecommendLoanApplication recommendLoanApplication)
        {
           
                bool isUpdated = await _recommendLoanApplicationService.UpdateRecommendLoanApplication(recommendLoanApplication);
                TempData["SuccessMessage"] = "Recommend Loan Application Updated Successfully";
            return RedirectToAction("Index");
        }
    }
}
