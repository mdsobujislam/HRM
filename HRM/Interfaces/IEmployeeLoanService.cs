using HRM.Models;

namespace HRM.Interfaces
{
    public interface IEmployeeLoanService
    {
        Task<IEnumerable<EmployeeLoan>> GetAllEmployeeLoansAsync();
        Task<bool> InsertEmployeeLoanAsync(EmployeeLoan employeeLoan);
        Task<bool> UpdateEmployeeLoanAsync(EmployeeLoan employeeLoan);
        Task<List<LoanApproval>> SearchData(LoanApproval loanApproval);
    }
}
