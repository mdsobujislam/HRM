using Attendance.API.Models;

namespace Attendance.API.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetLoginUserAsync(string email, string password);
    }
}
