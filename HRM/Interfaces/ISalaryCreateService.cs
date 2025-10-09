using HRM.Models;

namespace HRM.Interfaces
{
    public interface ISalaryCreateService
    {
        Task<List<SalaryCreate>> GetAllSalaryCreateAsync(int empId);
    }
}
