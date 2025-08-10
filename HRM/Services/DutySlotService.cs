using Dapper;
using HRM.Interfaces;
using HRM.Models;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.Data.SqlClient;
using System.Data;

namespace HRM.Services
{
    public class DutySlotService : IDutySlotService
    {
        private readonly string _connectionString;
        private readonly BaseService _baseService;

        public DutySlotService(IConfiguration configuration, BaseService baseService)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(_connectionString));
            _baseService = baseService;
        }
        public async Task<bool> DeleteDutySlot(int id)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var queryString = "delete from DutySlots where id=@id";
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

        public async Task<List<DutySlot>> GetAllDutySlot()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var subscriptionId = _baseService.GetSubscriptionId();
                    var userId = _baseService.GetUserId();
                    var branchId = await _baseService.GetBranchId(subscriptionId, userId);

                    var query = @"SELECT t1.Id AS [Slot ID], t1.SlotName, t1.StartHour, t1.StartMinute, t1.DutyHour, t1.DutyMinute, t1.EndHour, t1.EndMinute, RIGHT('0' + CAST(t1.StartHour AS VARCHAR), 2) + ':' + RIGHT('0' + CAST(t1.StartMinute AS VARCHAR), 2) + ' Hr' AS DisplayTimeRange, RIGHT('0' + CAST(t1.DutyHour AS VARCHAR), 2) + ' Hrs ' + RIGHT('0' + CAST(t1.DutyMinute AS VARCHAR), 2) + ' Mins' AS DutyDuration, RIGHT('0' + CAST(t1.EndHour AS VARCHAR), 2) + ':' + RIGHT('0' + CAST(t1.EndMinute AS VARCHAR), 2) + ' Hr' AS DisplayEndTime, case when t1.BranchId is null then 'null' else (select Name from Branch t2 where t2.Id=t1.BranchId) end as Branch, case when t1.DesignationId is null then 'null' else (select DesignationName from Designation t3 where t3.Id=t1.DesignationId) end as Designation,case when t1.DepartmentId is null then 'null' else (select DepartmentName from Department t4 where t4.Id=t1.DepartmentId) end as Department FROM DutySlots t1 where t1.SubscriptionId=@subscriptionId ORDER BY t1.Id";

                    var result = await connection.QueryAsync<DutySlot>(query, new { subscriptionId });
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> InsertDutySlot(DutySlot dutySlot)
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

                    //var queryString = "select SlotName from DutySlots where lower(SlotName)='{0}' ";
                    //var query = string.Format(queryString, dutySlot.SlotName.ToLower());
                    //var roleObj = await connection.QueryFirstOrDefaultAsync<string>(query);
                    //if (roleObj != null && roleObj.Length > 0)
                    //    return false;
                    var queryString = "insert into DutySlots (SlotName,StartHour, StartMinute, DutyHour, DutyMinute, EndHour, EndMinute,BranchId,SubscriptionId,Status,CreatedAt) values ";
                    queryString += "( @SlotName, @StartHour, @StartMinute, @DutyHour, @DutyMinute, @EndHour, @EndMinute,@BranchId,@SubscriptionId,@Status,@CreatedAt)";
                    var parameters = new DynamicParameters();
                    parameters.Add("SlotName", dutySlot.SlotName, DbType.String);
                    parameters.Add("BranchId", dutySlot.BranchId, DbType.Int64);
                    parameters.Add("StartHour", dutySlot.StartHour, DbType.Int64);
                    parameters.Add("StartMinute", dutySlot.StartMinute, DbType.Int64);
                    parameters.Add("DutyHour", dutySlot.DutyHour, DbType.Int64);
                    parameters.Add("DutyMinute", dutySlot.DutyMinute, DbType.Int64);
                    parameters.Add("EndHour", dutySlot.EndHour, DbType.Int64);
                    parameters.Add("EndMinute", dutySlot.EndMinute, DbType.Int64);
                    parameters.Add("SubscriptionId", subscriptionId);
                    parameters.Add("status", 1, DbType.Boolean);
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

        public async Task<bool> UpdateDutySlot(DutySlot dutySlot)
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

                    var queryString = "update DutySlots set SlotName=@SlotName,StartHour=@StartHour,StartMinute=@StartMinute,DutyHour=@DutyHour,DutyMinute=@DutyMinute,EndHour=@EndHour,EndMinute=@EndMinute,BranchId=@BranchId,SubscriptionId=@SubscriptionId,UpdatedAt=@UpdatedAt where id=@id";
                    var parameters = new DynamicParameters();
                    parameters.Add("SlotName", dutySlot.SlotName, DbType.String);
                    parameters.Add("StartHour", dutySlot.StartHour, DbType.Int64);
                    parameters.Add("StartMinute", dutySlot.StartMinute, DbType.Int64);
                    parameters.Add("DutyHour", dutySlot.DutyHour, DbType.Int64);
                    parameters.Add("DutyMinute", dutySlot.DutyMinute, DbType.Int64);
                    parameters.Add("EndHour", dutySlot.EndHour, DbType.Int64);
                    parameters.Add("EndMinute", dutySlot.EndMinute, DbType.Int64);
                    parameters.Add("BranchId", dutySlot.BranchId,DbType.Int64);
                    parameters.Add("SubscriptionId", subscriptionId);
                    parameters.Add("UpdatedAt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DbType.String);
                    parameters.Add("id", dutySlot.Id.ToString(), DbType.Int64);
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
