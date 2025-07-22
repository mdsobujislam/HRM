using Dapper;
using HRM.Models;
using Microsoft.Data.SqlClient;
using HRM.Interfaces;

namespace HRM.Services
{
    public class UserVMService:IUserVMService
    {
        private readonly string _connectionString;
        public UserVMService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(_connectionString));
        }

        public async Task<List<Uservm>> GetAllUserAsync()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var queryString = "select id,name,(case when status=1 then 'Active' else 'InActive' end) StatusName,";
                    queryString += "Email,MobileNo,Password,School,Class,Dob from Users where name<>'Super Admin' order  by name";
                    var query = string.Format(queryString);
                    var userList = await connection.QueryAsync<Uservm>(query);
                    var users = userList.ToList();

                    foreach (var uservm in users)
                    {
                        queryString = "select (select name from roles where id=ur.roleid) from UserRole ur where userid='{0}'";
                        query = string.Format(queryString, uservm.Id);
                        var roles = await connection.QueryAsync<string>(query);
                        var rolesName = "";
                        foreach (var item in roles)
                        {
                            rolesName += item + ",";
                        }
                        uservm.Roles = rolesName;
                    }
                    return users;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
