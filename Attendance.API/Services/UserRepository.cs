using Attendance.API.Interfaces;
using Attendance.API.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Attendance.API.Services
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connection;
        public UserRepository(IConfiguration configuration)
        {
            _connection = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(_connection));
        }
        public async Task<User> GetLoginUserAsync(string email, string password)
        {
            using (var connection = new SqlConnection(_connection))
            {
                string sql = @"
                SELECT Id, Name, EmployeeId, MobileNo, Status, Email, SubscriptionId
                FROM Users
                WHERE Email = @Email AND Password = @Password AND Status = 1";

                return await connection.QueryFirstOrDefaultAsync<User>(sql, new
                {
                    Email = email,
                    Password = password
                });
            }
        }
    }
}
