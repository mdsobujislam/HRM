using Dapper;
using HRM.Interfaces;
using HRM.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace HRM.Services
{
    public class BonusCalculateService : IBonusCalculateService
    {
        private readonly string _connectionString;
        private readonly BaseService _baseService;

        public BonusCalculateService(IConfiguration configuration, BaseService baseService)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(_connectionString));
            _baseService = baseService;
        }
        public async Task<bool> DeleteBonusCalculate(int id)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var queryString = "delete from BonusCalculate where id=@id";
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

        public async Task<List<BonusCalculate>> GetAllAsync()
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

                    var result = await connection.QueryAsync<BonusCalculate>(query, new { subscriptionId });
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> InsertBonusCalculate(BonusCalculate bonusCalculate)
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

                    var queryString = "insert into Overtime (BonusTypeId,BonusDate,Percentage,EmployeeId,BranchId,DesignationId,DepartmentId,SubscriptionId,CompanyId,CreatedAt) values ";
                    queryString += "( @BonusTypeId,@BonusDate,@Percentage,@EmployeeId,@BranchId,@DesignationId,@DepartmentId,@SubscriptionId,@CompanyId,@CreatedAt)";
                    var parameters = new DynamicParameters();
                    parameters.Add("BonusTypeId", bonusCalculate.BonusTypeId, DbType.Int64);
                    parameters.Add("BonusDate", bonusCalculate.BonusDate, DbType.String);
                    parameters.Add("Percentage", bonusCalculate.Percentage, DbType.Double);
                    parameters.Add("EmployeeId", bonusCalculate.EmployeeId, DbType.Int64);
                    parameters.Add("BranchId", bonusCalculate.BranchId, DbType.Int64);
                    parameters.Add("DesignationId", bonusCalculate.DesignationId, DbType.Int64);
                    parameters.Add("DepartmentId", bonusCalculate.DepartmentId, DbType.Int64);
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

        public Task<bool> UpdateBonusCalculate(BonusCalculate bonusCalculate)
        {
            throw new NotImplementedException();
        }
    }
}
