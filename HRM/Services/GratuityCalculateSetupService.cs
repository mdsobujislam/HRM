using Dapper;
using HRM.Interfaces;
using HRM.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace HRM.Services
{
    public class GratuityCalculateSetupService : IGratuityCalculateSetupService
    {
        private readonly string _connectionString;
        private readonly BaseService _baseService;
        public GratuityCalculateSetupService(IConfiguration configuration, BaseService baseService)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(_connectionString));
            _baseService = baseService;
        }
        public async Task<bool> DeleteGratuityCalculateSetupAsync(int id)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var queryString = "delete from GratuityCalculateSetup where id=@id";
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

        public async Task<List<GratuityCalculateSetup>> GetAllGratuityCalculateSetupAsync()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var subscriptionId = _baseService.GetSubscriptionId();
                    var userId = _baseService.GetUserId();
                    var branchId = await _baseService.GetBranchId(subscriptionId, userId);

                    var query = @"SELECT t1.Id as Id, t1.SalaryHeadId as SalaryHeadId, t3.Salaryitems as SalaryHead, t1.BranchId as BranchId, t2.Name as Branch, t1.MinYear as MinYear FROM GratuityCalculateSetup t1 LEFT JOIN Branch t2 on t1.BranchId = t2.Id LEFT JOIN SalaryHeads t3 on t1.SalaryHeadId = t3.Id WHERE t1.SubscriptionId = @subscriptionId";

                    var result = await connection.QueryAsync<GratuityCalculateSetup>(query, new { subscriptionId });
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> InsertGratuityCalculateSetupAsync(GratuityCalculateSetup gratuityCalculateSetup)
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

                    var workingQuery = @"SELECT OffDay FROM OffDays WHERE BranchId = @BranchId AND SubscriptionId = @SubscriptionId";

                    var workingDays = await connection.QueryFirstOrDefaultAsync<string>(
                        workingQuery,
                        new { BranchId = gratuityCalculateSetup.BranchId, SubscriptionId = subscriptionId }
                    );

                    int offDayCount = 0;

                    if (!string.IsNullOrEmpty(workingDays))
                    {
                        offDayCount = workingDays.Split(',').Length;
                    }
                    int monthDays = 30;
                    int workingDaysCount = monthDays - offDayCount;


                    var queryString = "insert into GratuityCalculateSetup (SalaryHeadId,MinYear,MonthDays,WorkingDays,BranchId,SubscriptionId,CompanyId,CreatedAt) values ";
                    queryString += "( @SalaryHeadId,@MinYear,@MonthDays,@WorkingDays,@BranchId,@SubscriptionId,@CompanyId,@CreatedAt)";
                    var parameters = new DynamicParameters();
                    parameters.Add("SalaryHeadId", gratuityCalculateSetup.SalaryHeadId, DbType.Int64);
                    parameters.Add("MinYear", gratuityCalculateSetup.MinYear, DbType.Int64);
                    parameters.Add("MonthDays", monthDays, DbType.Int64);
                    parameters.Add("WorkingDays", workingDaysCount, DbType.Int64);
                    parameters.Add("BranchId", gratuityCalculateSetup.BranchId, DbType.Int64);
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

        public async Task<bool> UpdateGratuityCalculateSetupAsync(GratuityCalculateSetup gratuityCalculateSetup)
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

                    var workingQuery = @"SELECT OffDay FROM OffDays WHERE BranchId = @BranchId AND SubscriptionId = @SubscriptionId";

                    var workingDays = await connection.QueryFirstOrDefaultAsync<string>(
                        workingQuery,
                        new { BranchId = gratuityCalculateSetup.BranchId, SubscriptionId = subscriptionId }
                    );

                    int offDayCount = 0;

                    if (!string.IsNullOrEmpty(workingDays))
                    {
                        offDayCount = workingDays.Split(',').Length;
                    }
                    int monthDays = 30;
                    int workingDaysCount = monthDays - offDayCount;

                    var queryString = "Update GratuityCalculateSetup set SalaryHeadId=@SalaryHeadId,MinYear=@MinYear,MonthDays=@MonthDays,WorkingDays=@WorkingDays,BranchId=@BranchId,SubscriptionId=@SubscriptionId,CompanyId=@CompanyId,UpdatedAt=@UpdatedAt where Id='" + gratuityCalculateSetup.Id + "' ";
                    var parameters = new DynamicParameters();
                    parameters.Add("SalaryHeadId", gratuityCalculateSetup.SalaryHeadId, DbType.Int64);
                    parameters.Add("MinYear", gratuityCalculateSetup.MinYear, DbType.Int64);
                    parameters.Add("MonthDays", monthDays, DbType.Int64);
                    parameters.Add("WorkingDays", workingDaysCount, DbType.Int64);
                    parameters.Add("BranchId", gratuityCalculateSetup.BranchId, DbType.Int64);
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
