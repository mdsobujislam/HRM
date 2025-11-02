using Dapper;
using HRM.Interfaces;
using HRM.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace HRM.Services
{
    public class DesignationService : IDesignationService
    {
        private readonly string _connectionString;
        private readonly BaseService _baseService;

        public DesignationService(IConfiguration configuration, BaseService baseService)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(_connectionString));
            _baseService = baseService;
        }
        public async Task<bool> DeleteDesignation(int id)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var queryString = "delete from Designation where id=@id";
                    var parameters = new DynamicParameters();
                    parameters.Add("id", id.ToString(), DbType.String);
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

        public async Task<List<Designation>> GetAllDesignation()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var subscriptionId = _baseService.GetSubscriptionId();
                    var userId = _baseService.GetUserId();
                    var branchId = await _baseService.GetBranchId(subscriptionId, userId);

                    var query = @"Select t1.Id, t1.DesignationName,t2.Name as Branch,t2.Id as BranchId,t3.DepartmentName as Department,t1.DepartmentId as DepartmentId from Designation t1 JOIN Branch t2 on t1.BranchId=t2.Id JOIN Department t3 on t3.Id=t1.DepartmentId WHERE t1.SubscriptionId=@subscriptionId and t2.Id=@branchId";

                    var result = await connection.QueryAsync<Designation>(query, new { subscriptionId, branchId });
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<IEnumerable<Designation>> GetDesignationsByDepartmentId(int departmentId)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var subscriptionId = _baseService.GetSubscriptionId();
                    var userId = _baseService.GetUserId();
                    //var branchId = await _baseService.GetBranchId(subscriptionId, userId);

                    var query = @"Select t1.Id, t1.DesignationName,t2.Name as Branch,t2.Id as BranchId,t3.DepartmentName as Department,t1.DepartmentId as DepartmentId from Designation t1 JOIN Branch t2 on t1.BranchId=t2.Id JOIN Department t3 on t3.Id=t1.DepartmentId WHERE t1.SubscriptionId=@subscriptionId and t1.DepartmentId=@departmentId";

                    var result = await connection.QueryAsync<Designation>(query, new { subscriptionId, departmentId });
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> InsertDesignation(Designation designation)
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

                    var queryString = "insert into Designation (DesignationName,BranchId,DepartmentId,SubscriptionId,CompanyId,CreatedAt) values ";
                    queryString += "( @DesignationName,@BranchId,@DepartmentId,@SubscriptionId,@CompanyId,@CreatedAt)";
                    var parameters = new DynamicParameters();
                    parameters.Add("DesignationName", designation.DesignationName, DbType.String);
                    parameters.Add("BranchId", designation.BranchId, DbType.Int64);
                    parameters.Add("DepartmentId", designation.DepartmentId, DbType.Int64);
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

        public async Task<bool> UpdateDesignation(Designation designation)
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

                    var queryString = "Update Designation set DesignationName=@DesignationName,BranchId=@BranchId,DepartmentId=@DepartmentId,SubscriptionId=@SubscriptionId,CompanyId=@CompanyId,UpdatedAt=@UpdatedAt where Id='" + designation.Id + "' ";
                    var parameters = new DynamicParameters();
                    parameters.Add("DesignationName", designation.DesignationName, DbType.String);
                    parameters.Add("BranchId", designation.BranchId, DbType.Int64);
                    parameters.Add("DepartmentId", designation.DepartmentId, DbType.Int64);
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
