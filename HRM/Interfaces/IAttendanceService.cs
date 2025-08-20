using HRM.Models;

namespace HRM.Interfaces
{
    public interface IAttendanceService
    {
        Task<bool> InsertAttendance(Attendance attendance);
        Task<bool> UpdateAttendance(Attendance attendance);
        Task<bool> DeleteAttendance(int id);
        Task<List<Attendance>> GetAllAttendance();
    }
}
