using HRM.Interfaces;
using HRM.Models;
using HRM.Services;
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
        //public async Task<IActionResult> Index(int empId)
        //{
        //    var loanInstallments =await _loanInstallmentService.GetLoanInstallmentsAsync(empId);
        //    return View(loanInstallments);
        //}

        public async Task<IActionResult> Index(int empId)
        {
            var loanInstallments = await _loanInstallmentService.GetLoanInstallmentsAsync(empId);

            // Get first unpaid installment
            var unpaidInstallment = loanInstallments
                .FirstOrDefault(x => x.InstallmentStatus != "Paid"); // or "Unpaid" depending on your DB values

            if (unpaidInstallment != null)
            {
                ViewBag.InstallmentFrom = unpaidInstallment.Installment_No;
                ViewBag.InstallmentTo = unpaidInstallment.Installment_No;
                ViewBag.NetAmount = unpaidInstallment.Installment_Amount;
                ViewBag.EmployeeId = unpaidInstallment.EmployeeId;
                ViewBag.LoanId = unpaidInstallment.LoanId;
                int installmentFrom = unpaidInstallment.Installment_No;
                int installmentTo = unpaidInstallment.Installment_No;
                double netAmount = unpaidInstallment.Installment_Amount;

                ViewBag.TotalAmount = (installmentTo - installmentFrom + 1) * netAmount;
            }

            return View(loanInstallments);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(PayInstallment payInstallment)
        {


            bool isInserted = await _loanInstallmentService.UpdateLoanInstallmentAsync(payInstallment);

            if (isInserted)
            {
                TempData["SuccessMessage"] = "Loan payed successfully.";
                return RedirectToAction("Index"); // Redirect to loan list page
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to Loan payed. Please try again.";
                return View(payInstallment);
            }
        }

    }
}
