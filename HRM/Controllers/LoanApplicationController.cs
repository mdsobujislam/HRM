using HRM.Interfaces;
using HRM.Models;
using HRM.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HRM.Controllers
{
    public class LoanApplicationController : Controller
    {
        private readonly ILoanApplicationService _loanApprovalService;
        public LoanApplicationController(ILoanApplicationService loanApprovalService)
        {
            _loanApprovalService = loanApprovalService ?? throw new ArgumentNullException(nameof(loanApprovalService));
        }

        public async Task<IActionResult> Index()
        {
            var loanApplications =await _loanApprovalService.GetAllLoanApplication();
            return View(loanApplications);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LoanApplication loanApplication)
        {
            try
            {
                if (loanApplication.Id == 0)
                {
                    await _loanApprovalService.InsertLoanApplication(loanApplication);
                    TempData["SuccessMessage"] = "Loan Application created successfully.";
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error: " + ex.Message;
                return RedirectToAction("Index");
            }
        }
    }
}
