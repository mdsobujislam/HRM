using HRM.Models;

namespace HRM.Interfaces
{
    public interface IOffDaysService
    {
        Task<List<OffDays>> GetAllOffDays();
        Task<bool> InsertOffDays(OffDays offDays);
        Task<bool> UpdateOffDays(OffDays offDays);
        Task<bool> DeleteOffDays(int id);
    }
}
