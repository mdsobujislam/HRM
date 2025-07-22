using Dapper;
using HRM.Models;
using Microsoft.Data.SqlClient;
using HRM.Interfaces;
using System.Data;

namespace HRM.Services
{
    public class UserTypeService : IUserTypeService
    {
        private readonly string _connectionString;
        public UserTypeService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(_connectionString));
        }
        public async Task<bool> DeleteUserTypeAsync(Guid userTypeId)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var queryString = "select count(*) from UserType where userTypeId=@id";
                    var parameters = new DynamicParameters();
                    parameters.Add("id", userTypeId.ToString(), DbType.String);
                    var result = await connection.ExecuteAsync(queryString, parameters);
                    if (result > 0)
                        return false;
                    queryString = "delete from UserType where id=@id";
                    parameters = new DynamicParameters();
                    parameters.Add("id", userTypeId.ToString(), DbType.String);
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

        public async Task<List<UserType>> GetAllUserTypeAsync()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var queryString = "select id,usertypes ,(case when status=1 then 'Active' else 'InActive' end) StatusName from UserType  ";
                    var query = string.Format(queryString);
                    var userTypeList = await connection.QueryAsync<UserType>(query);
                    return userTypeList.ToList();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<UserType> GetUserTypeByIdAsync(Guid userTypeId)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var queryString = "select * from UserType where id='{0}' order by usertypes  ";
                    var query = string.Format(queryString, userTypeId);
                    var userType = await connection.QueryFirstOrDefaultAsync<UserType>(query);
                    return userType;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> InsertUserTypeAsync(UserType userType)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var queryString = "select UserTypes from UserType where lower(UserTypes)='{0}' ";
                    var query = string.Format(queryString, userType.UserTypes.ToLower());
                    var userTypeObj = await connection.QueryFirstOrDefaultAsync<string>(query);
                    if (userTypeObj != null && userTypeObj.Length > 0)
                        return false;
                    queryString = "insert into UserType (id,UserTypes,status) values ";
                    queryString += "( @id,@UserTypes,@status)";
                    var parameters = new DynamicParameters();
                    parameters.Add("id", Guid.NewGuid().ToString(), DbType.String);
                    parameters.Add("UserTypes", userType.UserTypes, DbType.String);
                    parameters.Add("status", 1, DbType.Boolean);
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

        public async Task<bool> UpdateUserTypeAsync(UserType userType)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    var queryString = "update usertype set usertypes=@usertypes,status=@status where id=@id";
                    var parameters = new DynamicParameters();
                    parameters.Add("usertypes", userType.UserTypes, DbType.String);
                    parameters.Add("status", userType.Status, DbType.Boolean);
                    parameters.Add("id", userType.Id.ToString(), DbType.String);
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

    }
}
