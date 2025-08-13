using HRM.Models;

namespace HRM.Interfaces
{
    public interface ISalaryService
    {
        Task<List<Salary>> GetAllSalary();
        Task<bool> InsertSalary(Salary salary);
        Task<bool> UpdateSalary(Salary salary);
        Task<bool> DeleteSalary(int id);

    }
}
