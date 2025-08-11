using Dapper;
using HRM.Interfaces;
using HRM.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace HRM.Services
{
    public class OffDaysService : IOffDaysService
    {
        private readonly string _connectionString;
        private readonly BaseService _baseService;

        public OffDaysService(IConfiguration configuration, BaseService baseService)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(_connectionString));
            _baseService = baseService;
        }
        public async Task<bool> DeleteOffDays(int id)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var queryString = "delete from OffDays where id=@id";
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

        public async Task<List<OffDays>> GetAllOffDays()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var subscriptionId = _baseService.GetSubscriptionId();
                    var userId = _baseService.GetUserId();
                    var branchId = await _baseService.GetBranchId(subscriptionId, userId);

                    var query = @"Select t1.OffDay,(select t2.Name from Branch t2 where t1.BranchId=t2.Id ) as Branch,(Select t3.DesignationName from Designation t3 where t1.DesignationId=t3.Id) as Designation,(select t4.DepartmentName from Department t4 where t1.DepartmentId=t4.Id) as Department from OffDays t1 where t1.SubscriptionId=@subscriptionId ";

                    var result = await connection.QueryAsync<OffDays>(query, new { subscriptionId });
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> InsertOffDays(OffDays offDays)
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

                    var queryString = "insert into OffDays (OffDay,DesignationId,DepartmentId,BranchId,SubscriptionId,CompanyId,CreatedAt) values ";
                    queryString += "( @OffDay,@DesignationId,@DepartmentId,@BranchId,@SubscriptionId,@CompanyId,@CreatedAt)";
                    var parameters = new DynamicParameters();
                    parameters.Add("OffDay", offDays.OffDay, DbType.String);
                    parameters.Add("DesignationId", offDays.DesignationId, DbType.Int64);
                    parameters.Add("DepartmentId", offDays.DesignationId, DbType.Int64);
                    parameters.Add("BranchId", offDays.BranchId, DbType.Int64);
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

        public async Task<bool> UpdateOffDays(OffDays offDays)
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

                    var queryString = "Update OffDays set OffDay=@OffDay,DesignationId=@DesignationId,DepartmentId=@DepartmentId,BranchId=@BranchId,SubscriptionId=@SubscriptionId,CompanyId=@CompanyId,UpdatedAt=@UpdatedAt where Id='" + offDays.Id + "' ";
                    var parameters = new DynamicParameters();
                    parameters.Add("OffDay", offDays.OffDay, DbType.String);
                    parameters.Add("DesignationId", offDays.DesignationId, DbType.String);
                    parameters.Add("DepartmentId", offDays.DepartmentId, DbType.String);
                    parameters.Add("BranchId", offDays.BranchId, DbType.Int64);
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
