using HRM.Models;

namespace HRM.Interfaces
{
    public interface ILateAttendanceService
    {
        Task<List<LateAttendance>> GetAllLateAttendance();
        Task<bool> InsertLateAttendance(LateAttendance lateAttendance);
        Task<bool> UpdateLateAttendance(LateAttendance lateAttendance);
        Task<bool> DeleteLateAttendance(int id);
    }
}
