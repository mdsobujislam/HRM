using Dapper;
using HRM.Interfaces;
using HRM.Models;
using HRM.Models.Autocomplete;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
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

                    if (!string.IsNullOrEmpty(gratuityCalculate.EmployeeId))
                    {
                        // Example input: "1002 Rahim"
                        var value = gratuityCalculate.EmployeeId.Trim();
                        var idOnly = value.Split(' ')[0]; // Extract "1002"
                        gratuityCalculate.EmployeeId = idOnly; // keep only the numeric ID
                    }

                    var basicSalaryQuery = @"SELECT t1.BranchId AS BranchId, t4.Value AS LastBasicSalary, CAST( DATEDIFF(YEAR, t1.DateOfAppointment, GETDATE()) - CASE WHEN DATEADD(YEAR, DATEDIFF(YEAR, t1.DateOfAppointment, GETDATE()), t1.DateOfAppointment) > GETDATE() THEN 1 ELSE 0 END AS VARCHAR(10) ) + ' Years ' + CAST( DATEDIFF(MONTH, DATEADD(YEAR, DATEDIFF(YEAR, t1.DateOfAppointment, GETDATE()), t1.DateOfAppointment), GETDATE()) AS VARCHAR(10) ) + ' Months' AS EmploymentYears FROM Employees t1 LEFT JOIN Branch t2 ON t1.BranchId = t2.Id LEFT JOIN Designation t3 ON t1.DesignationId = t3.Id OUTER APPLY ( SELECT TOP 1 t4.Value FROM Salary t4 WHERE t4.Parameter = 'Basic Salary' ORDER BY t4.FromDate DESC ) t4 WHERE t1.EmpId=@EmployeeId and t1.SubscriptionId = @SubscriptionId";

                    var basicSalary = await connection.QueryFirstOrDefaultAsync<GratuityCalculate>(
                        basicSalaryQuery,
                        new { EmployeeId = gratuityCalculate.EmployeeId, SubscriptionId = subscriptionId }
                    );
                    double EmploymentYears = 0;
                    double LastBasicSalary = 0;
                    var BranchId = 0;
                    if (basicSalary != null)
                    {
                        BranchId = basicSalary.BranchId;
                        LastBasicSalary = Convert.ToDouble(basicSalary.LastBasicSalary);
                        string employmentYearsText = basicSalary.EmploymentYears; // e.g. "11 Years 2 Months"


                        if (!string.IsNullOrEmpty(employmentYearsText))
                        {
                            // Extract numbers from text using regex
                            var match = System.Text.RegularExpressions.Regex.Match(employmentYearsText, @"(\d+)\s*Years\s*(\d+)?");
                            if (match.Success)
                            {
                                int years = int.Parse(match.Groups[1].Value);
                                int months = 0;

                                if (match.Groups[2].Success)
                                    months = int.Parse(match.Groups[2].Value);

                                // Convert months to a fraction of a year
                                EmploymentYears = years + (months / 12.0);
                            }
                        }

                    }

                    var workingQuery = @"SELECT OffDay FROM OffDays WHERE BranchId = @BranchId AND SubscriptionId = @SubscriptionId";

                    var workingDays = await connection.QueryFirstOrDefaultAsync<string>(
                        workingQuery,
                        new { BranchId = BranchId, SubscriptionId = subscriptionId }
                    );

                    int offDayCount = 0;

                    if (!string.IsNullOrEmpty(workingDays))
                    {
                        int weeklyOffDays = workingDays.Split(',').Length; // e.g. "Saturday,Sunday" = 2
                        offDayCount = weeklyOffDays * 4; // Approximate 4 weeks per month → 8 off-days

                    }
                    int monthDays = 30;
                    int workingDaysCount = monthDays - offDayCount;

                    var tAmount = (LastBasicSalary * workingDaysCount) / monthDays;

                    var totalGratuityAmount = tAmount * EmploymentYears;


                    var queryString = "insert into GratuityCalculate (EmployeeId,LastBasicSalary,EmploymentYears,TotalGratuityAmount,BranchId,SubscriptionId,CompanyId,CreatedAt) values ";
                    queryString += "( @EmployeeId,@LastBasicSalary,@EmploymentYears,@TotalGratuityAmount,@BranchId,@SubscriptionId,@CompanyId,@CreatedAt)";
                    var parameters = new DynamicParameters();
                    parameters.Add("EmployeeId", gratuityCalculate.EmployeeId, DbType.Int64);
                    parameters.Add("LastBasicSalary", LastBasicSalary, DbType.Double);
                    parameters.Add("EmploymentYears", EmploymentYears, DbType.Double);
                    parameters.Add("TotalGratuityAmount", totalGratuityAmount, DbType.Int64);
                    parameters.Add("BranchId", BranchId, DbType.Int64);
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



        public async Task<IEnumerable<object>> SearchEmployees(string term)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var subscriptionId = _baseService.GetSubscriptionId();
                var userId = _baseService.GetUserId();
                var branchId = await _baseService.GetBranchId(subscriptionId, userId);
                var companyId = await _baseService.GetCompanyId(subscriptionId);

                await connection.OpenAsync();

                //var query = @" SELECT EmpId, EmployeeName AS EmpName, UploadPhoto as EmployeeImage FROM Employees WHERE EmployeeName LIKE @Term OR CAST(EmpId AS NVARCHAR) LIKE @Term and SubscriptionId='" + subscriptionId + "' and Status=0";
                var query = @" SELECT e.EmpId AS EmpId, e.EmployeeName as EmpName, e.UploadPhoto AS EmployeeImage FROM Employees e WHERE e.EmpId NOT IN ( SELECT EmployeeId FROM GratuityCalculate ) AND e.SubscriptionId = '" + subscriptionId + "' AND e.Status = 0";

                var result = await connection.QueryAsync<EmployeeDto>(query, new { Term = $"%{term}%" });

                return result.Select(e => new
                {
                    id = e.EmpId,
                    label = $"{e.EmpId} {e.EmpName}",        // shown in dropdown
                    value = $"{e.EmpId} {e.EmpName}",        // sets in textbox
                    image = string.IsNullOrEmpty(e.EmployeeImage)
                            ? "/profile/default/defaultPic.jpg"        // default image
                            : e.EmployeeImage.StartsWith("http") ? e.EmployeeImage : $"/profile/{e.EmployeeImage}"
                }).ToList();
            }
        }
    }
}
