using HRM.Models;

namespace HRM.Interfaces
{
    public interface ILeaveApplicationService
    {
        Task<List<LeaveApplication>> GetAllLeaveApplicationByAsync();
        Task<bool> InsertLeaveApplication(LeaveApplication leaveApplication, List<IFormFile> Documents);
        Task<List<LeaveApplication>> GetAllInitialApproved();
        Task<bool> UpdateLeaveApplicationByImmediateBoss(LeaveApplication leaveApplication);
        Task<List<LeaveApplication>> GetAllApproved();
        Task<bool> UpdateLeaveApplicationbyHR(LeaveApplication leaveApplication);


        //Available Leave Days
        Task<List<bool>> GetAllAvailableLeave();

    }
}
