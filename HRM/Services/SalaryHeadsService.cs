using Dapper;
using HRM.Interfaces;
using HRM.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace HRM.Services
{
    public class SalaryHeadsService : ISalaryHeadsService
    {
        private readonly string _connectionString;
        private readonly BaseService _baseService;

        public SalaryHeadsService(IConfiguration configuration, BaseService baseService)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(_connectionString));
            _baseService = baseService;
        }
        public async Task<bool> DeleteSalaryHeads(int id)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var queryString = "delete from SalaryHeads where id=@id";
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

        public async Task<List<SalaryHeads>> GetAllSalaryHeads()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var subscriptionId = _baseService.GetSubscriptionId();
                    var userId = _baseService.GetUserId();
                    var branchId = await _baseService.GetBranchId(subscriptionId, userId);

                    var query = @"Select t1.Id,t1.Salaryitems,t2.Name from SalaryHeads t1 Join Branch t2 on t1.BranchId=t2.Id where t1.SubscriptionId=@subscriptionId ";

                    var result = await connection.QueryAsync<SalaryHeads>(query, new { subscriptionId });
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> InsertSalaryHeads(SalaryHeads salaryHeads)
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

                    var queryString = "insert into SalaryHeads (Salaryitems,DBranchId,SubscriptionId,CompanyId,CreatedAt) values ";
                    queryString += "( @Salaryitems,,@BranchId,@SubscriptionId,@CompanyId,@CreatedAt)";
                    var parameters = new DynamicParameters();
                    parameters.Add("Salaryitems", salaryHeads.Salaryitems, DbType.String);
                    parameters.Add("BranchId", salaryHeads.BranchId, DbType.Int64);
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

        public async Task<bool> UpdateSalaryHeads(SalaryHeads salaryHeads)
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

                    var queryString = "Update OffDays set Salaryitems=@Salaryitems,BranchId=@BranchId,SubscriptionId=@SubscriptionId,CompanyId=@CompanyId,UpdatedAt=@UpdatedAt where Id='" + salaryHeads.Id + "' ";
                    var parameters = new DynamicParameters();
                    parameters.Add("Salaryitems", salaryHeads.Salaryitems, DbType.String);
                    parameters.Add("BranchId", salaryHeads.BranchId, DbType.Int64);
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
