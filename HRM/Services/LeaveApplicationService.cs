using Dapper;
using HRM.Interfaces;
using HRM.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace HRM.Services
{
    public class LeaveApplicationService : ILeaveApplicationService
    {
        private readonly string _connectionString;
        private readonly BaseService _baseService;

        public LeaveApplicationService(IConfiguration configuration, BaseService baseService)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(_connectionString));
            _baseService = baseService;
        }
        public async Task<List<LeaveApplication>> GetAllLeaveApplicationByAsync()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var subscriptionId = _baseService.GetSubscriptionId();
                    var userId = _baseService.GetUserId();
                    var branchId = await _baseService.GetBranchId(subscriptionId, userId);

                    var query = @"Select t1.EmployeeId,t1.LeaveTypeId from LeaveRecords t1 where t1.SubscriptionId= @subscriptionId";

                    var result = await connection.QueryAsync<LeaveApplication>(query, new { subscriptionId });
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> InsertLeaveApplication(LeaveApplication leaveApplication)
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

                    var queryString = "insert into TaxSlab (EmployeeId,LeaveTypeId,StartDate,EndDate,NumberOfDays,Reason,CreatedBy,CreatedAt,BranchId,SubscriptionId,CompanyId) values ";
                    queryString += "( @EmployeeId,@LeaveTypeId,@StartDate,@EndDate,@NumberOfDays,@Reason,@CreatedBy,@CreatedAt,@BranchId,@SubscriptionId,@CompanyId)";
                    var parameters = new DynamicParameters();
                    parameters.Add("EmployeeId", userId, DbType.Int64);
                    parameters.Add("LeaveTypeId", leaveApplication.LeaveTypeId, DbType.Int64);
                    parameters.Add("StartDate", leaveApplication.StartDate, DbType.String);
                    parameters.Add("EndDate", leaveApplication.EndDate, DbType.String);
                    parameters.Add("NumberOfDays", leaveApplication.NumberOfDays, DbType.Int64);
                    parameters.Add("Reason", leaveApplication.Reason, DbType.String);
                    parameters.Add("CreatedBy", userId, DbType.Int64);
                    parameters.Add("CreatedAt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DbType.String);
                    parameters.Add("BranchId", branchId, DbType.Int64);
                    parameters.Add("SubscriptionId", subscriptionId);
                    parameters.Add("CompanyId", companyId);
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
