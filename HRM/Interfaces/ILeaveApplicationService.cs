using HRM.Models;

namespace HRM.Interfaces
{
    public interface ILeaveApplicationService
    {
        Task<List<LeaveApplication>> GetAllLeaveApplicationByAsync();
        Task<bool> InsertLeaveApplication(LeaveApplication leaveApplication);

    }
}
