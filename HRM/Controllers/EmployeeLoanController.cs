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

            // Call service to fetch data
            var approvalLoanApplicationList = await _employeeLoanService.SearchData(loanApproval);

            return View(approvalLoanApplicationList);
        }
    }
}
