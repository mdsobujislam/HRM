using Dapper;
using HRM.Interfaces;
using HRM.Models;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.Data.SqlClient;
using System.Data;

namespace HRM.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly string _connectionString;
        private readonly BaseService _baseService;

        public DepartmentService(IConfiguration configuration, BaseService baseService)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(_connectionString));
            _baseService = baseService;
        }
        public async Task<bool> DeleteDepartment(int departmentId)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var queryString = "delete from Department where id=@id";
                    var parameters = new DynamicParameters();
                    parameters.Add("id", departmentId.ToString(), DbType.String);
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

        public async Task<List<Department>> GetAllDepartment()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var subscriptionId = _baseService.GetSubscriptionId();
                    var userId = _baseService.GetUserId();
                    var branchId = await _baseService.GetBranchId(subscriptionId, userId);

                    var query = @"Select Id,DepartmentName from Department WHERE SubscriptionId = @subscriptionId";

                    var result = await connection.QueryAsync<Department>(query, new { subscriptionId });
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> InsertDepartment(Department department)
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

                    var queryString = "insert into Department (DepartmentName,SubscriptionId,CompanyId,CreatedAt) values ";
                    queryString += "( @DepartmentName,@SubscriptionId,@CompanyId,@CreatedAt)";
                    var parameters = new DynamicParameters();
                    parameters.Add("DepartmentName", department.DepartmentName, DbType.String);
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

        public async Task<bool> UpdateDepartment(Department department)
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

                    var queryString = "Update Department set DepartmentName=@DepartmentName,SubscriptionId=@SubscriptionId,CompanyId=@CompanyId,UpdatedAt=@UpdatedAt where Id='"+department.Id+"' ";
                    var parameters = new DynamicParameters();
                    parameters.Add("DepartmentName", department.DepartmentName, DbType.String);
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
