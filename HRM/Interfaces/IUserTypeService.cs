using HRM.Models;

namespace HRM.Interfaces
{
    public interface IUserTypeService
    {
        Task<List<UserType>> GetAllUserTypeAsync();
        Task<bool> InsertUserTypeAsync(UserType userType);
        Task<bool> UpdateUserTypeAsync(UserType userType);
        Task<UserType> GetUserTypeByIdAsync(Guid roleId);
        Task<bool> DeleteUserTypeAsync(Guid userTypeId);
    }
}
