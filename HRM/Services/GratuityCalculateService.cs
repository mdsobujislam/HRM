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

        public async Task<List<ShowGratuity>> GetAllShowGratuityAsync(ShowGratuity showGratuity)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    var subscriptionId = _baseService.GetSubscriptionId();
                    var userId = _baseService.GetUserId();
                    var branchsId = await _baseService.GetBranchId(subscriptionId, userId);

                    var query = @" SELECT t1.EmpId AS EmpId, t1.UploadPhoto AS Photo, t1.EmployeeName AS Name, t2.Name AS Branch, t3.DesignationName AS Designation, t1.DateOfAppointment AS AppoinmentDate, t4.Value AS LastBasicSalary, CAST( DATEDIFF(YEAR, t1.DateOfAppointment, GETDATE()) - CASE WHEN DATEADD(YEAR, DATEDIFF(YEAR, t1.DateOfAppointment, GETDATE()), t1.DateOfAppointment) > GETDATE() THEN 1 ELSE 0 END AS VARCHAR(10) ) + ' Years ' + CAST( DATEDIFF(MONTH, DATEADD(YEAR, DATEDIFF(YEAR, t1.DateOfAppointment, GETDATE()), t1.DateOfAppointment), GETDATE()) AS VARCHAR(10) ) + ' Months' AS NoOfYears FROM Employees t1 LEFT JOIN Branch t2 ON t1.BranchId = t2.Id LEFT JOIN Designation t3 ON t1.DesignationId = t3.Id OUTER APPLY ( SELECT TOP 1 t4.Value FROM Salary t4 WHERE t1.EmpId = t4.EmployeeId AND t4.Parameter = 'Basic Salary' ORDER BY t4.FromDate DESC ) t4 WHERE t1.SubscriptionId = @SubscriptionId ";

                    if (showGratuity.BranchId != 0)
                    {
                        query += " AND t1.BranchId = @BranchId";
                    }

                    var result = await connection.QueryAsync<ShowGratuity>(
                        query,
                        new { SubscriptionId = subscriptionId, BranchId = showGratuity.BranchId }
                    );

                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching gratuity data", ex);
            }
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
