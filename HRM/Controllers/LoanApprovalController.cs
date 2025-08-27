using HRM.Interfaces;
using HRM.Models;
using HRM.Services;
using Microsoft.AspNetCore.Mvc;

namespace HRM.Controllers
{
    public class LoanApprovalController : Controller
    {
        private readonly ILoanApprovalService _loanApprovalService;
        public LoanApprovalController(ILoanApprovalService loanApprovalService)
        {
            _loanApprovalService = loanApprovalService;
        }
        public async Task<IActionResult> Index(LoanApproval loanApproval)
        {

            if (string.IsNullOrEmpty(loanApproval.FromDate))
                loanApproval.FromDate = DateTime.Today.ToString("yyyy-MM-dd");

            if (string.IsNullOrEmpty(loanApproval.ToDate))
                loanApproval.ToDate = DateTime.Today.ToString("yyyy-MM-dd");

            // Call service to fetch data
            var approvalLoanApplicationList = await _loanApprovalService.SearchData(loanApproval);

            // If no data, return empty list of the correct type
            if (approvalLoanApplicationList == null || !approvalLoanApplicationList.Any())
            {
                return View(new List<LoanApproval>());
            }

            return View(approvalLoanApplicationList);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(LoanApproval loanApproval)
        {

            bool isUpdated = await _loanApprovalService.UpdateLoanApproval(loanApproval);
            TempData["SuccessMessage"] = "Recommend Loan Application Updated Successfully";
            return RedirectToAction("Index");
        }
    }
}
