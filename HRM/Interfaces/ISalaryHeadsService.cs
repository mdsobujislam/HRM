using HRM.Models;

namespace HRM.Interfaces
{
    public interface ISalaryHeadsService
    {
        Task<List<SalaryHeads>> GetAllSalaryHeads();
        Task<bool> InsertSalaryHeads(SalaryHeads salaryHeads);
        Task<bool> UpdateSalaryHeads(SalaryHeads salaryHeads);
        Task<bool> DeleteSalaryHeads(int id);
    }
}
