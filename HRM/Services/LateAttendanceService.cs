using Dapper;
using HRM.Interfaces;
using HRM.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace HRM.Services
{
    public class LateAttendanceService : ILateAttendanceService
    {
        private readonly string _connectionString;
        private readonly BaseService _baseService;

        public LateAttendanceService(IConfiguration configuration, BaseService baseService)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(_connectionString));
            _baseService = baseService;
        }
        public async Task<bool> DeleteLateAttendance(int id)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var queryString = "delete from LateAttendance where id=@id";
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

        public async Task<List<LateAttendance>> GetAllLateAttendance()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var subscriptionId = _baseService.GetSubscriptionId();
                    var userId = _baseService.GetUserId();
                    var branchId = await _baseService.GetBranchId(subscriptionId, userId);

                    var query = @"Select t1.Id,t1.LateMin as LateMin,t1.NoOfLate as NoOfLate,t1.AdjustmentType as AdjustmentType,t1.Basic as Basic,t2.TypeName as TypeName,t3.Name as Branch from LateAttendance t1 JOIN LeaveType t2 on t1.LeaveTypeId=t2.Id Join Branch t3 on t1.BranchId=t3.Id WHERE t1.SubscriptionId=@subscriptionId ";

                    var result = await connection.QueryAsync<LateAttendance>(query, new { subscriptionId });
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> InsertLateAttendance(LateAttendance lateAttendance)
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

                    var queryString = "insert into LateAttendance (LateMin,NoOfLate,AdjustmentType,Basic,LeaveTypeId,BranchId,SubscriptionId,CompanyId,CreatedAt) values ";
                    queryString += "( @LateMin,@NoOfLate,@AdjustmentType,@Basic,@LeaveTypeId,@BranchId,@SubscriptionId,@CompanyId,@CreatedAt)";
                    var parameters = new DynamicParameters();
                    parameters.Add("LateMin", lateAttendance.LateMin, DbType.Int64);
                    parameters.Add("NoOfLate", lateAttendance.NoOfLate, DbType.Int64);
                    parameters.Add("AdjustmentType", lateAttendance.AdjustmentType, DbType.String);
                    parameters.Add("Basic", lateAttendance.Basic, DbType.String);
                    parameters.Add("LeaveTypeId", lateAttendance.LeaveTypeId, DbType.Int64);
                    parameters.Add("BranchId", lateAttendance.BranchId, DbType.Int64);
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

        public async Task<bool> UpdateLateAttendance(LateAttendance lateAttendance)
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

                    var queryString = "Update LateAttendance set LateMin=@LateMin,NoOfLate=@NoOfLate,AdjustmentType=@AdjustmentType,Basic=@Basic,LeaveTypeId=@LeaveTypeId,BranchId,SubscriptionId,CompanyId,UpdatedAt=@UpdatedAt where Id='" + lateAttendance.Id + "' ";
                    var parameters = new DynamicParameters();
                    parameters.Add("LateMin", lateAttendance.LateMin, DbType.Int64);
                    parameters.Add("NoOfLate", lateAttendance.NoOfLate, DbType.Int64);
                    parameters.Add("AdjustmentType", lateAttendance.AdjustmentType, DbType.String);
                    parameters.Add("Basic", lateAttendance.Basic, DbType.String);
                    parameters.Add("LeaveTypeId", lateAttendance.LeaveTypeId, DbType.Int64);
                    parameters.Add("BranchId", lateAttendance.BranchId, DbType.Int64);
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
