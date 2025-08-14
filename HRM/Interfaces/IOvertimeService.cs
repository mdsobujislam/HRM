using HRM.Models;

namespace HRM.Interfaces
{
    public interface IOvertimeService
    {
        Task<List<Overtime>> GetAllOvertime();
        Task<bool> InsertOvertime(Overtime overtime);
        Task<bool> UpdateOvertime(Overtime overtime);
        Task<bool> DeleteOvertime(int id);
    }
}
