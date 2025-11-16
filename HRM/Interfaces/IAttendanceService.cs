using HRM.Models.Attendance;

namespace HRM.Interfaces
{
    public interface IAttendanceService
    {
        Task<bool> SaveAttendanceAsync(AttendanceBatchDto dto);
        Task<List<AttendanceData>> GetAllData(int userId, bool isAdmin);
        Task<List<AttendanceData>> GetAttendanceByFilter(int userId, DateTime fromDate, DateTime toDate, bool isAdmin);
        Task<List<AttendanceReport>> GetAttendanceReport(int userId, DateTime fromDate, DateTime toDate, bool isAdmin);
        Task<bool> AddManualAttendanceAsync(ManualAttendanceDto dto, int adminId);
    }
}
