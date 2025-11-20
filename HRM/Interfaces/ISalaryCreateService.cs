using HRM.Models;

namespace HRM.Interfaces
{
    public interface ISalaryCreateService
    {
        Task<List<SalaryCreate>> GetAllSalaryCreateAsync(int branchId, string monthName);
        Task<List<SalaryCreate>> GetSalaryReportCreateAsync(int branchId, string monthName);
        //Task<bool> InsertSalaryCreateAsync(SalaryCreate salaryCreate);
        Task<bool> InsertSalaryListAsync(IEnumerable<SalaryCreate> salaryList);
    }
}
