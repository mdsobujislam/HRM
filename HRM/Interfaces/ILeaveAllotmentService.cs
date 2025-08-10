using HRM.Models;

namespace HRM.Interfaces
{
    public interface ILeaveAllotmentService
    {
        Task<List<LeaveAllotment>> GetAllLeaveAllotment();
        Task<bool> InsertLeaveAllotment(LeaveAllotment leaveAllotment);
        Task<bool> UpdateLeaveAllotment(LeaveAllotment leaveAllotment);
        Task<bool> DeleteLeaveAllotment(int id);
    }
}
