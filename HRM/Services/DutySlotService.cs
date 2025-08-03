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

                    var query = @"SELECT Id AS [Slot ID], SlotName, StartHour, StartMinute, DutyHour, DutyMinute, EndHour, EndMinute, RIGHT('0' + CAST(StartHour AS VARCHAR), 2) + ':' + RIGHT('0' + CAST(StartMinute AS VARCHAR), 2) + ' Hr' AS DisplayTimeRange, RIGHT('0' + CAST(DutyHour AS VARCHAR), 2) + ' Hrs ' + RIGHT('0' + CAST(DutyMinute AS VARCHAR), 2) + ' Mins' AS DutyDuration, RIGHT('0' + CAST(EndHour AS VARCHAR), 2) + ':' + RIGHT('0' + CAST(EndMinute AS VARCHAR), 2) + ' Hr' AS DisplayEndTime FROM DutySlots where SubscriptionId=@subscriptionId ORDER BY Id";

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

                    var queryString = "select SlotName from DutySlots where lower(SlotName)='{0}' ";
                    var query = string.Format(queryString, dutySlot.SlotName.ToLower());
                    var roleObj = await connection.QueryFirstOrDefaultAsync<string>(query);
                    if (roleObj != null && roleObj.Length > 0)
                        return false;
                    queryString = "insert into DutySlots (SlotName, StartHour, StartMinute, DutyHour, DutyMinute, EndHour, EndMinute,BranchId,SubscriptionId,Status,CreatedAt) values ";
                    queryString += "( @SlotName, @StartHour, @StartMinute, @DutyHour, @DutyMinute, @EndHour, @EndMinute,@BranchId,@SubscriptionId,@Status,@CreatedAt)";
                    var parameters = new DynamicParameters();
                    parameters.Add("SlotName", dutySlot.SlotName, DbType.String);
                    parameters.Add("StartHour", dutySlot.StartHour, DbType.Int64);
                    parameters.Add("StartMinute", dutySlot.StartMinute, DbType.Int64);
                    parameters.Add("DutyHour", dutySlot.DutyHour, DbType.Int64);
                    parameters.Add("DutyMinute", dutySlot.DutyMinute, DbType.Int64);
                    parameters.Add("EndHour", dutySlot.EndHour, DbType.Int64);
                    parameters.Add("EndMinute", dutySlot.EndMinute, DbType.Int64);
                    parameters.Add("BranchId", branchId);
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
                    parameters.Add("BranchId", branchId);
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
