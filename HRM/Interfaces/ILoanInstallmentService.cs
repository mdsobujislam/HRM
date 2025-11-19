using HRM.Models;

namespace HRM.Interfaces
{
    public interface ILoanInstallmentService
    {
        Task<List<LoanInstallment>> GetLoanInstallmentsAsync(string empId);
        Task<bool> UpdateLoanInstallmentAsync(PayInstallment payInstallment);
    }
}
