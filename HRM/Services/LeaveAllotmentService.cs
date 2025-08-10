using Dapper;
using HRM.Interfaces;
using HRM.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace HRM.Services
{
    public class LeaveAllotmentService : ILeaveAllotmentService
    {
        private readonly string _connectionString;
        private readonly BaseService _baseService;

        public LeaveAllotmentService(IConfiguration configuration, BaseService baseService)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(_connectionString));
            _baseService = baseService;
        }
        public async Task<bool> DeleteLeaveAllotment(int id)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var queryString = "delete from NoOFLeave where id=@id";
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

        public async Task<List<LeaveAllotment>> GetAllLeaveAllotment()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var subscriptionId = _baseService.GetSubscriptionId();
                    var userId = _baseService.GetUserId();
                    var branchId = await _baseService.GetBranchId(subscriptionId, userId);

                    var query = @"Select t1.Id, t2.TypeName as LeaveType,t1.NoOfLeaves as NoOfLeaves,t1.YearOrMonth as YearOrMonth,t1.LeaveCarry as LeaveCarry,t1.NoOfYears as NoOfYears,t3.Name as Branch from NoOFLeave t1 JOIN LeaveType t2 on t1.LeaveTypeId=t2.Id Join Branch t3 on t1.BranchId=t3.Id WHERE t1.SubscriptionId=@subscriptionId ";

                    var result = await connection.QueryAsync<LeaveAllotment>(query, new { subscriptionId });
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> InsertLeaveAllotment(LeaveAllotment leaveAllotment)
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

                    var queryString = "insert into NoOFLeave (LeaveTypeId,NoOfLeaves,YearOrMonth,LeaveCarry,NoOfYears,BranchId,SubscriptionId,CompanyId,CreatedAt) values ";
                    queryString += "( @LeaveTypeId,@NoOfLeaves,@YearOrMonth,@LeaveCarry,@NoOfYears,@BranchId,@SubscriptionId,@CompanyId,@CreatedAt)";
                    var parameters = new DynamicParameters();
                    parameters.Add("LeaveTypeId", leaveAllotment.LeaveTypeId, DbType.Int64);
                    parameters.Add("NoOfLeaves", leaveAllotment.NoOfLeaves, DbType.String);
                    parameters.Add("YearOrMonth", leaveAllotment.YearOrMonth, DbType.String);
                    parameters.Add("LeaveCarry", leaveAllotment.LeaveCarry, DbType.String);
                    parameters.Add("NoOfYears", leaveAllotment.NoOfYears, DbType.Int64);
                    parameters.Add("BranchId", leaveAllotment.BranchId, DbType.Int64);
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

        public async Task<bool> UpdateLeaveAllotment(LeaveAllotment leaveAllotment)
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

                    var queryString = "Update NoOFLeave set LeaveTypeId=@LeaveTypeId,NoOfLeaves=@NoOfLeaves,YearOrMonth=@YearOrMonth,LeaveCarry=@LeaveCarry,NoOfYears=@NoOfYears,BranchId=@BranchId,SubscriptionId=@SubscriptionId,CompanyId=@CompanyId,UpdatedAt=@UpdatedAt where Id='" + leaveAllotment.Id + "' ";
                    var parameters = new DynamicParameters();
                    parameters.Add("LeaveTypeId", leaveAllotment.LeaveTypeId, DbType.String);
                    parameters.Add("NoOfLeaves", leaveAllotment.NoOfLeaves, DbType.String);
                    parameters.Add("YearOrMonth", leaveAllotment.YearOrMonth, DbType.String);
                    parameters.Add("LeaveCarry", leaveAllotment.LeaveCarry, DbType.String);
                    parameters.Add("NoOfYears", leaveAllotment.NoOfYears, DbType.Int64);
                    parameters.Add("BranchId", leaveAllotment.BranchId, DbType.Int64);
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
