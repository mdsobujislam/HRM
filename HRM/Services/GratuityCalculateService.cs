using Dapper;
using HRM.Interfaces;
using HRM.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace HRM.Services
{
    public class GratuityCalculateService : IGratuityCalculateService
    {
        private readonly string _connectionString;
        private readonly BaseService _baseService;
        public GratuityCalculateService(IConfiguration configuration, BaseService baseService)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(_connectionString));
            _baseService = baseService;
        }

        public Task<List<ShowGratuity>> GetAllShowGratuityAsync(int branchId, string date)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> InsertGratuityCalculateAsysnc(GratuityCalculate gratuityCalculate)
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
                        new { BranchId = gratuityCalculate.BranchId, SubscriptionId = subscriptionId }
                    );

                    int offDayCount = 0;

                    if (!string.IsNullOrEmpty(workingDays))
                    {
                        offDayCount = workingDays.Split(',').Length;
                    }
                    int monthDays = 30;
                    int workingDaysCount = monthDays - offDayCount;


                    var queryString = "insert into GratuityCalculate (EmployeeId,LastBasicSalary,EmploymentYears,TotalGratuityAmount,BranchId,SubscriptionId,CompanyId,CreatedAt) values ";
                    queryString += "( @EmployeeId,@LastBasicSalary,@EmploymentYears,@TotalGratuityAmount,@BranchId,@SubscriptionId,@CompanyId,@CreatedAt)";
                    var parameters = new DynamicParameters();
                    parameters.Add("EmployeeId", gratuityCalculate.EmployeeId, DbType.Int64);
                    parameters.Add("LastBasicSalary", gratuityCalculate.LastBasicSalary, DbType.Double);
                    parameters.Add("EmploymentYears", gratuityCalculate.EmploymentYears, DbType.Double);
                    parameters.Add("TotalGratuityAmount", workingDaysCount, DbType.Int64);
                    parameters.Add("BranchId", gratuityCalculate.BranchId, DbType.Int64);
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
    }
}
