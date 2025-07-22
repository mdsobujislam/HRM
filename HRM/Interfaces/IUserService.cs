using HRM.Models;

namespace HRM.Interfaces
{
    public interface IUserService
    {
        Task<string> InsertUserAsync(User user);
        Task<bool> UpdateUserAsync(User user);
        Task<bool> DeleteUserAsync(Guid userId);
        Task<List<User>> GetAllUserAsync();
        Task<User> GetUserAsync(Guid userId);
        Task<List<string>> GetUserRolesAsync(string userId);
        Task<User> GetLoginUserAsync(string userName, string password);
        Task<User> GetUserTypeCheckAsync(string userName);
    }
}
