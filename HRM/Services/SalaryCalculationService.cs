using Dapper;
using HRM.Interfaces;
using HRM.Models;
using HRM.Models.Autocomplete;
using Microsoft.Data.SqlClient;
using System.Data;

namespace HRM.Services
{
    public class SalaryCalculationService : ISalaryCalculationService
    {
        private readonly string _connectionString;
        private readonly BaseService _baseService;

        public SalaryCalculationService(IConfiguration configuration, BaseService baseService)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(_connectionString));
            _baseService = baseService;
        }
        public async Task<List<SalaryHeads>> GetAllSalaryHeadsAsync()
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var subscriptionId = _baseService.GetSubscriptionId();
                var userId = _baseService.GetUserId();

                // Get employee branch
                var empBranchQuery = "SELECT BranchId FROM Employees WHERE EmpId = @userId";
                int empBranchId = await connection.ExecuteScalarAsync<int>(empBranchQuery, new { userId });

                // Fetch salary heads
                var query = @" SELECT Id, SalaryItems FROM SalaryHeads WHERE SubscriptionId = @subscriptionId AND BranchId = @empBranchId ORDER BY Id";

                var result = await connection.QueryAsync<SalaryHeads>(query, new { subscriptionId, empBranchId });

                return result.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving salary heads", ex);
            }
        }

        public async Task<bool> InsertSalaryCalculationAsync(SalaryCalculationMaster salaryCalculation)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var subscriptionId = _baseService.GetSubscriptionId();
                    var userId = _baseService.GetUserId();
                    var companyId = await _baseService.GetCompanyId(subscriptionId);

                    if (salaryCalculation.BranchId > 0)
                    {
                        // Use provided branch
                    }
                    else
                    {
                        // Get employee branch
                        var empBranchQuery = "SELECT BranchId FROM Employees WHERE EmpId = @userId";
                        salaryCalculation.BranchId = await connection.ExecuteScalarAsync<int>(empBranchQuery, new { userId });
                    }


                    // 🔹 Step 1: Get all employees under selected Branch, Department, and Designation
                    var employeeQuery = @" SELECT EmpId FROM Employees WHERE BranchId = @BranchId AND DepartmentId = @DepartmentId AND DesignationId = @DesignationId AND SubscriptionId = @SubscriptionId AND Status = 1";  // optional if you want only active employees

                    var employees = await connection.QueryAsync<string>(employeeQuery, new
                    {
                        BranchId = salaryCalculation.BranchId,
                        DepartmentId = salaryCalculation.DepartmentId,
                        DesignationId = salaryCalculation.DesignationId,
                        SubscriptionId = subscriptionId
                    });

                    if (employees == null || !employees.Any())
                        return false;

                    int success = 0;

                    // 🔹 Step 2: Loop through each employee
                    foreach (var empId in employees)
                    {
                        // 🔹 Step 3: Loop through each salary head item
                        foreach (var item in salaryCalculation.SalaryItems)
                        {
                            var insertQuery = @"
                        INSERT INTO SalaryCalculation 
                        (SL, Parameter, Value, ApplyDate, BranchId, DepartmentId, DesignationId, EmployeeId, CompanyId, SubscriptionId)
                        VALUES 
                        (@SL, @Parameter, @Value, @ApplyDate, @BranchId, @DepartmentId, @DesignationId, @EmployeeId, @CompanyId, @SubscriptionId)";

                            var parameters = new DynamicParameters();
                            parameters.Add("@SL", item.SL, DbType.Int32);
                            parameters.Add("@Parameter", item.Parameter, DbType.String);
                            parameters.Add("@Value", item.Value, DbType.Double);
                            parameters.Add("@ApplyDate", salaryCalculation.ApplyDate, DbType.Date);
                            parameters.Add("@BranchId", salaryCalculation.BranchId, DbType.Int64);
                            parameters.Add("@DepartmentId", salaryCalculation.DepartmentId, DbType.Int64);
                            parameters.Add("@DesignationId", salaryCalculation.DesignationId, DbType.Int64);
                            parameters.Add("@EmployeeId", empId, DbType.String);
                            parameters.Add("@CompanyId", companyId, DbType.Int64);
                            parameters.Add("@SubscriptionId", subscriptionId, DbType.Int64);

                            success += await connection.ExecuteAsync(insertQuery, parameters);
                        }
                    }

                    return success > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error inserting salary calculation data", ex);
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
                var query = @" SELECT e.EmpId AS EmpId, e.EmployeeName as EmpName, e.UploadPhoto AS EmployeeImage FROM Employees e WHERE e.EmpId NOT IN ( SELECT EmployeeId FROM GratuityCalculate ) AND e.SubscriptionId = '" + subscriptionId + "' AND e.Status = 1";

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
