using HRM.Models;

namespace HRM.Interfaces
{
    public interface ILoanApprovalService
    {
        Task<List<LoanApproval>> GetAllLoanApproval();
        Task<bool> InsertLoanApproval(LoanApproval loanApproval);
        Task<bool> UpdateLoanApproval(LoanApproval loanApproval);
        Task<bool> DeleteLoanApproval(int id);
    }
}
