using HRM.Models;

namespace HRM.Interfaces
{
    public interface IDutySlotService
    {
        Task<List<DutySlot>> GetAllDutySlot();
        Task<bool> InsertDutySlot(DutySlot dutySlot);
        Task<bool> UpdateDutySlot(DutySlot dutySlot);
        Task<bool> DeleteDutySlot(int id);
    }
}
