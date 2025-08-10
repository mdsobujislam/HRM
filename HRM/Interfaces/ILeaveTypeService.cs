using HRM.Models;

namespace HRM.Interfaces
{
    public interface ILeaveTypeService
    {
        Task<List<LeaveType>> GetAllTeaveType();
        Task<bool> InsertTeaveType(LeaveType teaveType);
        Task<bool> UpdateTeaveType(LeaveType teaveType);
        Task<bool> DeleteTeaveType(int id);
    }
}
