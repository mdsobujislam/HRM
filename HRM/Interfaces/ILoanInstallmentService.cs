using HRM.Models;

namespace HRM.Interfaces
{
    public interface ILoanInstallmentService
    {
        Task<List<LoanInstallment>> GetLoanInstallmentsAsync(int empId);
        Task<bool> UpdateLoanInstallmentAsync(PayInstallment payInstallment);
    }
}
