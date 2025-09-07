using HRM.Interfaces;
using HRM.Models;
using HRM.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HRM.Controllers
{
    public class EmployeeLoanController : Controller
    {
        private readonly IEmployeeLoanService _employeeLoanService;
        public EmployeeLoanController(IEmployeeLoanService employeeLoanService)
        {
            _employeeLoanService = employeeLoanService;
        }

        public async Task<IActionResult> Index(LoanApproval loanApproval)
        {
            if (string.IsNullOrEmpty(loanApproval.FromDate))
                loanApproval.FromDate = DateTime.Today.ToString("yyyy-MM-dd");

            if (string.IsNullOrEmpty(loanApproval.ToDate))
                loanApproval.ToDate = DateTime.Today.ToString("yyyy-MM-dd");

            var approvalLoanApplicationList = await _employeeLoanService.SearchData(loanApproval)
                                              ?? new List<LoanApproval>();

            return View(approvalLoanApplicationList);
        }





        //public async Task<IActionResult> Index(LoanApproval loanApproval)
        //{
        //    if (string.IsNullOrEmpty(loanApproval.FromDate))
        //        loanApproval.FromDate = DateTime.Today.ToString("yyyy-MM-dd");

        //    if (string.IsNullOrEmpty(loanApproval.ToDate))
        //        loanApproval.ToDate = DateTime.Today.ToString("yyyy-MM-dd");

        //    // Call service to fetch data
        //    var approvalLoanApplicationList = await _employeeLoanService.SearchData(loanApproval);

        //    return View(approvalLoanApplicationList);
        //}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrUpdate(LoanApproval loanApproval)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please check the form fields.";
                return View(loanApproval);
            }

            bool isInserted = await _employeeLoanService.InsertEmployeeLoanAsync(loanApproval);

            if (isInserted)
            {
                TempData["SuccessMessage"] = "Employee Loan created successfully.";
                return RedirectToAction("Index"); // Redirect to loan list page
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to create Employee Loan. Please try again.";
                return View(loanApproval);
            }
        }

        [HttpGet]
        public IActionResult DownloadDocument(int loanId, string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return NotFound();

            // Normalize file path (avoid double slashes from DB)
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var fullPath = Path.Combine(uploadsFolder, filePath.TrimStart('/'));

            if (!System.IO.File.Exists(fullPath))
                return NotFound();

            var fileBytes = System.IO.File.ReadAllBytes(fullPath);
            var fileName = Path.GetFileName(fullPath);

            return File(fileBytes, "application/octet-stream", fileName);
        }

    }
}
