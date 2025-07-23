using HRM.Models;

namespace HRM.Repository
{
    public interface IUserService
    {
        Task<bool> InsertUserAsync(User user);
        Task<bool> UpdateUserAsync(User user);
        Task<bool> DeleteUserAsync(int userId);
        Task<List<User>> GetAllUserAsync();
        Task<User> GetUserAsync(int userId);
        Task<User> GetUserByEmailAsync(string email);
        Task<List<string>> GetUserRolesAsync(string userId);
        Task<User> GetLoginUserAsync(string userName,string password);
    }
}
