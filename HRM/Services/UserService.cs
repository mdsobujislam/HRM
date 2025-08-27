using Dapper;
using HRM.Models;
using HRM.Repository;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ServiceManagementSystem.Services
{
    public class UserService : IUserService
    {
        private readonly string _connectionString;
        public UserService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(_connectionString));
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var queryString = "delete from users where id=@id";
                    var parameters = new DynamicParameters();
                    parameters.Add("id", userId.ToString(), DbType.String);
                    var success = await connection.ExecuteAsync(queryString, parameters);
                    if (success > 0)
                    {
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<User>> GetAllUserAsync()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var queryString = "select id,name,(case when status=1 then 'Active' else 'InActive' end) StatusName,";
                    queryString+= "Email,MobileNo from Users where name<>'Super Admin' order  by name";
                    var query = string.Format(queryString);
                    var userList = await connection.QueryAsync<User>(query);
                    var users= userList.ToList();

                    foreach (var user in users)
                    {
                        queryString = "select (select name from roles where id=ur.roleid) from UserRole ur where userid='{0}'";
                        query = string.Format(queryString, user.Id); 
                        var roles=await connection.QueryAsync<string>(query);
                        var rolesName = "";
                        foreach (var item in roles)
                        {
                            rolesName += item + ",";
                        }
                        user.Roles = rolesName;
                    }
                    return users; 
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<User> GetLoginUserAsync(string userName, string password)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var queryString = "select EmployeeId,id,name,status,";
                    queryString += "Email,MobileNo,SubscriptionId from Users where email='{0}' and password='{1}' and status=1";
                    var query = string.Format(queryString, userName,password);
                    var user = await connection.QueryFirstOrDefaultAsync<User>(query);
                    return user;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<User> GetUserAsync(int userId)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var queryString = "select id,name,status,";
                    queryString += "Email,MobileNo from Users where id='{0}'";
                    var query = string.Format(queryString,userId.ToString());
                    var user = await connection.QueryFirstOrDefaultAsync<User>(query);
                    queryString = "select roleid from UserRole where userid='{0}'";
                    query = string.Format(queryString, userId.ToString());
                    var roles = await connection.QueryAsync<Guid>(query);
                    List<string> roleList = new List<string>();
                    foreach (var item in roles)
                    {
                        roleList.Add(item.ToString());
                    }
                    user.Role = roleList;
                    return user;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task<User> GetUserByEmailAsync(string email)
        {
            throw new NotImplementedException();
        }

        public async Task<List<string>> GetUserRolesAsync(string userId)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var queryString = "select roleid";
                    queryString += " from UserRole where UserId='{0}'";
                    var query = string.Format(queryString, userId.ToString());
                    var userRoles = await connection.QueryAsync<Guid>(query);
                    var roles = new List<string>();
                    foreach (var item in userRoles)
                    {
                        roles.Add(item.ToString());
                    }
                    return roles;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> InsertUserAsync(User user)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var queryString = "select email from users where lower(email)='{0}' ";
                    var query = string.Format(queryString, user.Email.ToLower());
                    var userObj = await connection.QueryFirstOrDefaultAsync<string>(query);
                    if (userObj != null && userObj.Length > 0)
                        return false;

                    var sql = "SELECT ISNULL(MAX(SubscriptionId), 0) + 1 FROM Users";
                    int nextSubId = connection.ExecuteScalar<int>(sql);

                    queryString = "insert into Users (name,mobileno,email,status,password,SubscriptionId,CreatedAt) values ";
                    queryString += "( @name,@mobileno,@email,@status,@password,@SubscriptionId,@CreatedAt);SELECT CAST(scope_identity() AS int)";
                    var parameters = new DynamicParameters();
                    parameters.Add("name", user.Name, DbType.String);
                    parameters.Add("mobileno", user.MobileNo, DbType.String);
                    parameters.Add("email", user.Email, DbType.String);
                    parameters.Add("status", 1, DbType.Boolean);
                    parameters.Add("password", user.Password, DbType.String);
                    parameters.Add("SubscriptionId", nextSubId);
                    parameters.Add("CreatedAt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DbType.String);
                    //var success = await connection.ExecuteAsync(queryString, parameters);
                    int insertedId = await connection.ExecuteScalarAsync<int>(queryString, parameters);

                        var Id = "select Id from Roles where Name='Super Admin'";
                        int RoleId = connection.ExecuteScalar<int>(Id);

                        queryString = "insert into UserRole (UserId,RoleId,SubscriptionId) values ";
                        queryString += "( @UserId,@RoleId,@SubscriptionId)";
                        parameters = new DynamicParameters();
                        parameters.Add("UserId", insertedId);
                        parameters.Add("RoleId", RoleId);
                        parameters.Add("SubscriptionId", nextSubId);

                        var success = await connection.ExecuteAsync(queryString, parameters);
                    if (success > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    var queryString = "update Users set name=@name,mobileno=@mobileno,email=@email,status=@status ";
                    queryString += " ,password=@password where id=@id";
                    var parameters = new DynamicParameters();
                    parameters.Add("name", user.Name, DbType.String);
                    parameters.Add("mobileno", user.MobileNo, DbType.String);
                    parameters.Add("email", user.Email, DbType.String);
                    parameters.Add("status", user.Status, DbType.Boolean);
                    parameters.Add("password", user.Password, DbType.String);
                    parameters.Add("id", user.Id.ToString(), DbType.String);
                    var success = await connection.ExecuteAsync(queryString, parameters);

                    if (user.Role.Count > 0)
                    {
                        queryString = "delete from UserRole where UserId=@UserId";
                        parameters = new DynamicParameters();
                        parameters.Add("UserId", user.Id.ToString(), DbType.String) ;
                        success = await connection.ExecuteAsync(queryString, parameters);
                    }

                    foreach (var item in user.Role)
                    {
                        queryString = "insert into UserRole (UserId,RoleId) values ";
                        queryString += "( @UserId,@RoleId)";
                        parameters = new DynamicParameters();
                        parameters.Add("UserId", user.Id.ToString(), DbType.String);
                        parameters.Add("RoleId", item, DbType.String);
                        success = await connection.ExecuteAsync(queryString, parameters);
                    }
                    if (success > 0)
                    {
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
