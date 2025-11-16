using HRM.Models;

namespace HRM.Interfaces
{
    public interface IAttendanceManualService
    {
        Task<bool> InsertAttendance(AttendanceManual attendance);
        Task<bool> UpdateAttendance(AttendanceManual attendance);
        Task<bool> DeleteAttendance(int id);
        Task<List<AttendanceManual>> GetAllAttendance();
    }
}
