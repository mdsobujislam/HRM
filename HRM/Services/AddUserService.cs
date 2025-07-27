using Dapper;
using HRM.Interfaces;
using HRM.Models;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.Data.SqlClient;
using System.Data;

namespace HRM.Services
{
    public class AddUserService : IAddUserService
    {
        private readonly string _connectionString;
        private readonly BaseService _baseService;

        public AddUserService(IConfiguration configuration, BaseService baseService)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(_connectionString));
            _baseService = baseService;
        }
        public async Task<bool> DeleteAddUser(int addUserId)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var queryString = "delete from Users where id=@id";
                    var parameters = new DynamicParameters();
                    parameters.Add("id", addUserId.ToString(), DbType.String);
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

        public async Task<List<AddUser>> GetAllAddUser()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var subscriptionId = _baseService.GetSubscriptionId();
                    var userId = _baseService.GetUserId();
                    var branchId = await _baseService.GetBranchId(subscriptionId, userId);

                    var query = @"Select t1.Id,t1.Name,t1.MobileNo,t1.Email,t1.Password,t2.Name from Users t1 JOIN Branch t2 on t1.BranchId=t2.Id WHERE t1.SubscriptionId=@subscriptionId";

                    var result = await connection.QueryAsync<AddUser>(query, new { subscriptionId });
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> InsertAddUser(AddUser addUser)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    var subscriptionId = _baseService.GetSubscriptionId();
                    var userId = _baseService.GetUserId();
                    var branchId = await _baseService.GetBranchId(subscriptionId, userId);
                    var companyId = await _baseService.GetCompanyId(subscriptionId);

                    var queryString = "insert into Users (Name,MobileNo,Email,Password,SubscriptionId,BranchId,CreatedAt) values ";
                    queryString += "( @Name,@MobileNo,@Email,@Password,@SubscriptionId,@BranchId,@CreatedAt)";
                    var parameters = new DynamicParameters();
                    parameters.Add("Name", addUser.Name, DbType.String);
                    parameters.Add("MobileNo", addUser.MobileNo, DbType.String);
                    parameters.Add("Email", addUser.Email, DbType.String);
                    parameters.Add("Password", addUser.Password, DbType.String);
                    parameters.Add("SubscriptionId", subscriptionId);
                    parameters.Add("BranchId", addUser.BranchId);
                    parameters.Add("CreatedAt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DbType.String);
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

        public async Task<bool> UpdateAddUser(AddUser addUser)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    var subscriptionId = _baseService.GetSubscriptionId();
                    var userId = _baseService.GetUserId();
                    var branchId = await _baseService.GetBranchId(subscriptionId, userId);
                    var companyId = await _baseService.GetCompanyId(subscriptionId);

                    var queryString = "Update Users set Name,MobileNo,Email,Password,SubscriptionId,BranchId,UpdatedAt where Id='" + addUser.Id + "' ";
                    var parameters = new DynamicParameters();
                    parameters.Add("Name", addUser.Name, DbType.String);
                    parameters.Add("MobileNo", addUser.MobileNo, DbType.String);
                    parameters.Add("Email", addUser.Email, DbType.String);
                    parameters.Add("Password", addUser.Password, DbType.String);
                    parameters.Add("SubscriptionId", subscriptionId);
                    parameters.Add("BranchId", addUser.BranchId);
                    parameters.Add("UpdatedAt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DbType.String);
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
