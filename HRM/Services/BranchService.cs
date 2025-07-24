using Dapper;
using HRM.Interfaces;
using HRM.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace HRM.Services
{
    public class BranchService : IBranchService
    {
        private readonly string _connectionString;
        private readonly BaseService _baseService;

        public BranchService(IConfiguration configuration, BaseService baseService)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(_connectionString));
            _baseService = baseService;
        }
        public async Task<bool> DeleteBranch(int branchId)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var queryString = "delete from Branch where id=@id";
                    var parameters = new DynamicParameters();
                    parameters.Add("id", branchId.ToString(), DbType.String);
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

        public async Task<List<Branch>> GetAllBranch()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var subscriptionId = _baseService.GetSubscriptionId();
                    var userId = _baseService.GetUserId();
                    var branchId = await _baseService.GetBranchId(subscriptionId, userId);

                    var query = @"Select Name,Address from Branch WHERE SubscriptionId = @subscriptionId";

                    var result = await connection.QueryAsync<Branch>(query, new { subscriptionId });
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Branch> GetAllBranchById(int branchId)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var queryString = "select * from Branch where id='{0}' ";
                    var query = string.Format(queryString, branchId);
                    var classType = await connection.QueryFirstOrDefaultAsync<Branch>(query);
                    return classType;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> InsertBranch(Branch branch)
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

                        var queryString = "insert into Branch (Name,Address,SubscriptionId,CompanyId,CreatedAt) values ";
                        queryString += "( @Name,@Address,@SubscriptionId,@CompanyId,@CreatedAt)";
                        var parameters = new DynamicParameters();
                        parameters.Add("Name", branch.Name, DbType.String);
                        parameters.Add("Address", branch.Address, DbType.String);
                        parameters.Add("SubscriptionId", subscriptionId);
                        parameters.Add("CompanyId", companyId);
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

        public async Task<bool> UpdateBranch(Branch branch)
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

                    var queryString = "Update Branch set Name=@Name,Address=@Address,SubscriptionId=@SubscriptionId,CompanyId=@CompanyId,UpdatedAt=@UpdatedAt where SubscriptionId='" + subscriptionId + "' and CompanyId='"+ companyId + "' ";
                        var parameters = new DynamicParameters();
                        parameters.Add("Name", branch.Name, DbType.String);
                        parameters.Add("Address", branch.Address, DbType.String);
                        parameters.Add("SubscriptionId", subscriptionId);
                        parameters.Add("CompanyId", companyId);
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
