using Dapper;
using HRM.Interfaces;
using HRM.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace HRM.Services
{
    public class OvertimeService : IOvertimeService
    {
        private readonly string _connectionString;
        private readonly BaseService _baseService;

        public OvertimeService(IConfiguration configuration, BaseService baseService)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(_connectionString));
            _baseService = baseService;
        }
        public async Task<bool> DeleteOvertime(int id)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var queryString = "delete from Overtime where id=@id";
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

        public async Task<List<Overtime>> GetAllOvertime()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var subscriptionId = _baseService.GetSubscriptionId();
                    var userId = _baseService.GetUserId();
                    var branchId = await _baseService.GetBranchId(subscriptionId, userId);

                    var query = @"select overhour as [Awardable Overtime (Hour)],adjustmenttype as [Overtime Award Method], (case when LeavetypeId<>'0' then (select l.typename from LeaveType l where l.id=overtime.leavetypeId) else '-' end) as [Awardable Leave Type],(case when Basic='True' then 'Yes'else 'No' end) as [Awardable with Basic Salary] from overtime Join Branch t3 on overtime.BranchId=t3.Id WHERE overtime.SubscriptionId=@subscriptionId";

                    var result = await connection.QueryAsync<Overtime>(query, new { subscriptionId });
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> InsertOvertime(Overtime overtime)
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

                    var queryString = "insert into Overtime (OverHour,AdjustmentType,Basic,LeaveTypeId,BranchId,SubscriptionId,CompanyId,CreatedAt) values ";
                    queryString += "( @OverHour,@AdjustmentType,@Basic,@LeaveTypeId,@BranchId,@SubscriptionId,@CompanyId,@CreatedAt)";
                    var parameters = new DynamicParameters();
                    parameters.Add("OverHour", overtime.OverHour, DbType.Int64);
                    parameters.Add("AdjustmentType", overtime.AdjustmentType, DbType.String);
                    parameters.Add("Basic", overtime.LeaveTypeId, DbType.String);
                    parameters.Add("LeaveTypeId", overtime.LeaveTypeId, DbType.Int64);
                    parameters.Add("BranchId", overtime.BranchId, DbType.Int64);
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

        public async Task<bool> UpdateOvertime(Overtime overtime)
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

                    var queryString = "Update LateAttendance set OverHour=@OverHour,AdjustmentType=@AdjustmentType,Basic=@Basic,LeaveTypeId=@LeaveTypeId,BranchId,SubscriptionId,CompanyId,UpdatedAt=@UpdatedAt where Id='" + overtime.Id + "' ";
                    var parameters = new DynamicParameters();
                    parameters.Add("OverHour", overtime.OverHour, DbType.Int64);
                    parameters.Add("AdjustmentType", overtime.AdjustmentType, DbType.String);
                    parameters.Add("Basic", overtime.Basic, DbType.String);
                    parameters.Add("LeaveTypeId", overtime.LeaveTypeId, DbType.Int64);
                    parameters.Add("BranchId", overtime.BranchId, DbType.Int64);
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
