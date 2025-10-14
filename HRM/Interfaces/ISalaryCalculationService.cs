using HRM.Models;

namespace HRM.Interfaces
{
    public interface ISalaryCalculationService
    {
        Task<List<SalaryHeads>> GetAllSalaryHeadsAsync();
        Task<bool> InsertSalaryCalculationAsync(SalaryCalculationMaster salaryCalculation);
        Task<IEnumerable<object>> SearchEmployees(string term);
    }
}
