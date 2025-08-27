using HRM.Models;

namespace HRM.Interfaces
{
    public interface ILoanApprovalService
    {
        Task<bool> UpdateLoanApproval(LoanApproval loanApproval);
        Task<List<LoanApproval>> SearchData(LoanApproval loanApproval);
    }
}
