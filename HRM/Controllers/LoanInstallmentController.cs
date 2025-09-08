using HRM.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HRM.Controllers
{
    public class LoanInstallmentController : Controller
    {
        private readonly ILoanInstallmentService _loanInstallmentService;
        public LoanInstallmentController(ILoanInstallmentService loanInstallmentService)
        {
            _loanInstallmentService = loanInstallmentService;
        }
        public async Task<IActionResult> Index(int empId)
        {
            var loanInstallments =await _loanInstallmentService.GetLoanInstallmentsAsync(empId);
            return View(loanInstallments);
        }
    }
}
