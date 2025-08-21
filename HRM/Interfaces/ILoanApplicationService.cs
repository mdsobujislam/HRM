using HRM.Models;

namespace HRM.Interfaces
{
    public interface ILoanApplicationService
    {
        Task<List<LoanApplication>> GetAllLoanApplication();
        Task<bool> InsertLoanApplication(LoanApplication loanApplication);
        Task<bool> UpdateLoanApplication(LoanApplication loanApplication);
        Task<bool> DeleteLoanApplication(int id);
    }
}
