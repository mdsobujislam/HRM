using Dapper;
using HRM.Interfaces;
using HRM.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Data.SqlTypes;

namespace HRM.Services
{
    public class SalaryService : ISalaryService
    {
        private readonly string _connectionString;
        private readonly BaseService _baseService;
        private readonly ILogger<SalaryService> _logger;

        public SalaryService(IConfiguration configuration,
                             BaseService baseService,
                             ILogger<SalaryService> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(_connectionString));
            _baseService = baseService;
            _logger = logger;
        }

        public async Task<bool> DeleteSalary(int id)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var result = await connection.ExecuteAsync(
                    "DELETE FROM Salary WHERE Id = @Id",
                    new { Id = id });

                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting salary with ID {SalaryId}", id);
                throw;
            }
        }

        public async Task<List<Salary>> GetAllSalary()
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var subscriptionId = _baseService.GetSubscriptionId();

                // Updated query to include AccountsPayable
                var query = @"SELECT s.*, sh.Salaryitems as SalaryHeadName,
                        b.Name as BranchName, e.EmployeeName,
                        d.DepartmentName, des.DesignationName
                        FROM Salary s
                        LEFT JOIN SalaryHeads sh ON s.SalaryHeadsId = sh.Id
                        LEFT JOIN Branch b ON s.BranchId = b.Id
                        LEFT JOIN Employees e ON s.EmployeeId = e.EmpId
                        LEFT JOIN Department d ON s.DepartmentId = d.Id
                        LEFT JOIN Designation des ON s.DesignationId = des.Id
                        WHERE s.SubscriptionId = @subscriptionId";

                return (await connection.QueryAsync<Salary>(query, new { subscriptionId })).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all salaries");
                throw;
            }
        }
        public async Task<bool> InsertSalary(Salary salary)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var subscriptionId = _baseService.GetSubscriptionId();
                var companyId = await _baseService.GetCompanyId(subscriptionId);

                var query = @"INSERT INTO Salary
                        (SalaryHeadsId, BranchId, DepartmentId, DesignationId, EmployeeId, 
                         Value, MonthIndex, Month, FromDate, ToDate, GenDate, Sl, FinalAmount, 
                         SubscriptionId, CompanyId)
                        VALUES
                        (@SalaryHeadsId, @BranchId, @DepartmentId, @DesignationId, @EmployeeId, 
                         @Value, @MonthIndex, @Month, @FromDate, @ToDate, @GenDate, @Sl, @FinalAmount, 
                         @SubscriptionId, @CompanyId)";

                var parameters = new DynamicParameters(salary);
                parameters.Add("SubscriptionId", subscriptionId);
                parameters.Add("CompanyId", companyId);

                var rows = await connection.ExecuteAsync(query, parameters);
                return rows > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inserting salary for employee {EmployeeId}", salary?.EmployeeId);
                throw;
            }
        }
        public async Task<bool> InsertSalary(List<Salary> salaries)
        {
            if (salaries == null || !salaries.Any())
            {
                _logger.LogWarning("InsertSalary called with null or empty salaries list");
                return false;
            }

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                var subscriptionId = _baseService.GetSubscriptionId();
                var companyId = await _baseService.GetCompanyId(subscriptionId);
                var currentYear = DateTime.Now.Year;

                // Check for existing salaries first
                var existingSalariesQuery = @"
            SELECT EmployeeId, MonthIndex, Year 
            FROM Salary 
            WHERE SubscriptionId = @SubscriptionId 
            AND CompanyId = @CompanyId 
            AND Year = @Year";

                var existingSalaries = await connection.QueryAsync<(int EmployeeId, int MonthIndex, int Year)>(
                    existingSalariesQuery,
                    new { SubscriptionId = subscriptionId, CompanyId = companyId, Year = currentYear },
                    transaction);

                // Create a hashset for quick lookup
                var existingSalarySet = new HashSet<string>();
                foreach (var existing in existingSalaries)
                {
                    var key = $"{existing.EmployeeId}-{existing.MonthIndex}-{existing.Year}";
                    existingSalarySet.Add(key);
                }

                var query = @"INSERT INTO Salary
            (SalaryHeadsId, BranchId, DepartmentId, DesignationId, EmployeeId, 
             Value, MonthIndex, Month, Year, FromDate, ToDate, GenDate, Sl, FinalAmount, Parameter, AccountsPayable,
             SubscriptionId, CompanyId)
            VALUES
            (@SalaryHeadsId, @BranchId, @DepartmentId, @DesignationId, @EmployeeId, 
             @Value, @MonthIndex, @Month, @Year, @FromDate, @ToDate, @GenDate, @Sl, @FinalAmount, @Parameter, @AccountsPayable,
             @SubscriptionId, @CompanyId)";

                var insertedCount = 0;
                var skippedCount = 0;
                var salaryHeadNames = await GetAllSalaryHeadNames();

                _logger.LogInformation("Starting to insert {SalaryCount} salary records", salaries.Count);

                // Get ALL salary heads for the company (not just branch-specific)
                var allSalaryHeads = await GetAllSalaryHeadsForCompany(subscriptionId, companyId);

                foreach (var salary in salaries)
                {
                    // Check if this employee already has a salary for this month/year
                    var salaryKey = $"{salary.EmployeeId}-{salary.MonthIndex}-{currentYear}";
                    if (existingSalarySet.Contains(salaryKey))
                    {
                        _logger.LogWarning("Skipping duplicate salary for employee {EmployeeId} in {Month}-{Year}",
                            salary.EmployeeId, salary.Month, currentYear);
                        skippedCount++;
                        continue;
                    }

                    // Calculate total amount from the provided salary items
                    double employeeTotalAmount = salary.SalaryItems.Sum(item => item.Value);

                    // Validate and ensure dates are within SQL Server range
                    var fromDate = IsValidSqlDateTime(salary.FromDate) ? salary.FromDate : DateTime.Now;
                    var toDate = IsValidSqlDateTime(salary.ToDate) ? salary.ToDate : DateTime.Now;
                    var genDate = IsValidSqlDateTime(salary.GenDate) ? salary.GenDate : DateTime.Now;
                    int slCounter = 1;

                    // Get valid salary heads for this employee's branch
                    var validSalaryHeads = await GetSalaryHeadsByBranch(salary.BranchId);
                    var validSalaryHeadIds = validSalaryHeads.Select(sh => sh.Id).ToList();

                    // Create a dictionary of provided salary values for quick lookup
                    var providedSalaryValues = salary.SalaryItems.ToDictionary(item => item.Id, item => item.Value);

                    // Insert rows for ALL valid salary heads, even if value is 0
                    foreach (var salaryHead in validSalaryHeads)
                    {
                        // Get the value from provided items, or 0 if not provided
                        double value = providedSalaryValues.ContainsKey(salaryHead.Id)
                            ? providedSalaryValues[salaryHead.Id]
                            : 0;

                        var parameters = new DynamicParameters();
                        parameters.Add("SalaryHeadsId", salaryHead.Id, DbType.Int32);
                        parameters.Add("BranchId", salary.BranchId, DbType.Int32);
                        parameters.Add("DepartmentId", salary.DepartmentId, DbType.Int32);
                        parameters.Add("DesignationId", salary.DesignationId, DbType.Int32);
                        parameters.Add("EmployeeId", salary.EmployeeId, DbType.Int32);
                        parameters.Add("Value", value, DbType.Decimal);
                        parameters.Add("FinalAmount", value, DbType.Decimal);
                        parameters.Add("MonthIndex", salary.MonthIndex, DbType.Int32);
                        parameters.Add("Month", salary.Month, DbType.String);
                        parameters.Add("Year", currentYear, DbType.Int32);
                        parameters.Add("FromDate", fromDate, DbType.DateTime);
                        parameters.Add("ToDate", toDate, DbType.DateTime);
                        parameters.Add("GenDate", genDate, DbType.DateTime);
                        parameters.Add("Sl", slCounter++, DbType.Int32);
                        parameters.Add("Parameter", salaryHeadNames.GetValueOrDefault(salaryHead.Id, "Unknown"), DbType.String);
                        parameters.Add("AccountsPayable", employeeTotalAmount, DbType.Decimal);
                        parameters.Add("SubscriptionId", subscriptionId, DbType.Int32);
                        parameters.Add("CompanyId", companyId, DbType.Int32);

                        await connection.ExecuteAsync(query, parameters, transaction);
                        insertedCount++;
                    }

                    // Add to existing set to prevent duplicates in the same batch
                    existingSalarySet.Add(salaryKey);
                }

                await transaction.CommitAsync();
                _logger.LogInformation("Successfully inserted {Count} salary records, skipped {Skipped} duplicates",
                    insertedCount, skippedCount);

                return insertedCount > 0;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error inserting salaries. Details: {ErrorMessage}", ex.Message);
                _logger.LogError("Stack trace: {StackTrace}", ex.StackTrace);
                throw;
            }
        }

        // Add this helper method to get all salary heads for the company
        private async Task<List<SalaryHeads>> GetAllSalaryHeadsForCompany(int subscriptionId, int companyId)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var sql = @"SELECT * FROM SalaryHeads 
                   WHERE SubscriptionId = @SubscriptionId AND CompanyId = @CompanyId
                   ORDER BY Id";

                return (await connection.QueryAsync<SalaryHeads>(sql, new { subscriptionId, companyId })).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all salary heads for company");
                return new List<SalaryHeads>();
            }
        }
        private async Task<Dictionary<int, string>> GetAllSalaryHeadNames()
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var sql = "SELECT Id, Salaryitems FROM SalaryHeads";
                var results = await connection.QueryAsync<(int Id, string Salaryitems)>(sql);

                return results.ToDictionary(x => x.Id, x => x.Salaryitems);
            }
            catch
            {
                return new Dictionary<int, string>();
            }
        }

        // Insert salary items if they exist
        //if (salary.SalaryItems != null && salary.SalaryItems.Any())
        //{
        //    var itemsQuery = @"INSERT INTO SalaryItems
        //(SalaryId, SalaryHeadsId, Value, SubscriptionId, CompanyId)
        //VALUES
        //(@SalaryId, @SalaryHeadsId, @Value, @SubscriptionId, @CompanyId)";

        //    // Get the last inserted ID
        //    var salaryId = await connection.ExecuteScalarAsync<int>(
        //        "SELECT SCOPE_IDENTITY()", transaction: transaction);

        //    foreach (var item in salary.SalaryItems)
        //    {
        //        var itemParams = new DynamicParameters();
        //        itemParams.Add("SalaryId", salaryId, DbType.Int32);
        //        itemParams.Add("SalaryHeadsId", item.Id, DbType.Int32);
        //        itemParams.Add("Value", item.Value, DbType.Decimal);
        //        itemParams.Add("SubscriptionId", subscriptionId, DbType.Int32);
        //        itemParams.Add("CompanyId", companyId, DbType.Int32);

        //        await connection.ExecuteAsync(itemsQuery, itemParams, transaction);
        //    }
        //}
        //        }

        //        await transaction.CommitAsync();
        //        return insertedCount > 0;
        //    }
        //    catch (Exception ex)
        //    {
        //        await transaction.RollbackAsync();
        //        _logger.LogError(ex, "Error inserting salaries");
        //        throw;
        //    }
        //}

        // Helper method to validate SQL Server datetime range
        private bool IsValidSqlDateTime(DateTime dateTime)
        {
            var minSqlDateTime = new DateTime(1753, 1, 1);
            var maxSqlDateTime = new DateTime(9999, 12, 31, 23, 59, 59);

            return dateTime >= minSqlDateTime && dateTime <= maxSqlDateTime;
        }
        public (DateTime fromDate, DateTime toDate) CalculateMonthDates(int monthIndex, int year)
        {
            if (monthIndex < 1 || monthIndex > 12)
            {
                throw new ArgumentException("Month index must be between 1 and 12", nameof(monthIndex));
            }

            // First day of the month
            DateTime fromDate = new DateTime(year, monthIndex, 1);

            // Last day of the month
            DateTime toDate = fromDate.AddMonths(1).AddDays(-1);

            return (fromDate, toDate);
        }

        public async Task<bool> UpdateSalary(Salary salary)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var query = @"UPDATE Salary SET 
                            SalaryHeadsId = @SalaryHeadsId,
                            Value = @Value,
                            FinalAmount = @FinalAmount,
                            FromDate = @FromDate,
                            ToDate = @ToDate
                            WHERE Id = @Id";

                var rows = await connection.ExecuteAsync(query, salary);
                return rows > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating salary with ID {SalaryId}", salary?.Id);
                throw;
            }
        }

        public async Task<List<Employee>> GetEmployeesByBranch(int branchId)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var sql = @"
                    SELECT e.EmpId, e.EmployeeName, e.DepartmentId, e.DesignationId, e.BranchId
                    FROM Employees e
                    WHERE e.BranchId = @BranchId";

                var rows = await connection.QueryAsync<Employee>(sql, new { BranchId = branchId });
                return rows.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting employees for branch {BranchId}", branchId);
                throw;
            }
        }
    

        public async Task<List<Employee>> GetEmployeesByCriteria(int? branchId = null, int? departmentId = null,
     int? designationId = null, int? employeeId = null)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var subscriptionId = _baseService.GetSubscriptionId();
                var companyId = await _baseService.GetCompanyId(subscriptionId);

                var sql = @"
            SELECT e.EmpId, e.EmployeeName, e.DepartmentId, e.DesignationId, e.BranchId,
                   d.DepartmentName as Department, 
                   des.DesignationName as Designation, 
                   b.Name as Branch
            FROM Employees e
            LEFT JOIN Department d ON e.DepartmentId = d.Id
            LEFT JOIN Designation des ON e.DesignationId = des.Id
            LEFT JOIN Branch b ON e.BranchId = b.Id
            WHERE e.SubscriptionId = @SubscriptionId AND e.CompanyId = @CompanyId";

                var parameters = new DynamicParameters();
                parameters.Add("SubscriptionId", subscriptionId);
                parameters.Add("CompanyId", companyId);

                if (branchId.HasValue && branchId > 0)
                {
                    sql += " AND e.BranchId = @BranchId";
                    parameters.Add("BranchId", branchId.Value);
                }

                if (departmentId.HasValue && departmentId > 0)
                {
                    sql += " AND e.DepartmentId = @DepartmentId";
                    parameters.Add("DepartmentId", departmentId.Value);
                }

                if (designationId.HasValue && designationId > 0)
                {
                    sql += " AND e.DesignationId = @DesignationId";
                    parameters.Add("DesignationId", designationId.Value);
                }

                if (employeeId.HasValue && employeeId > 0)
                {
                    sql += " AND e.EmpId = @EmployeeId";
                    parameters.Add("EmployeeId", employeeId.Value);
                }

                var employees = await connection.QueryAsync<Employee>(sql, parameters);
                return employees.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting employees by criteria");
                throw;
            }
        }

        public async Task<List<SalaryHeads>> GetActiveSalaryHeads()
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var subscriptionId = _baseService.GetSubscriptionId();
                var companyId = await _baseService.GetCompanyId(subscriptionId);

                var sql = @"SELECT * FROM SalaryHeads 
                   WHERE SubscriptionId = @SubscriptionId AND CompanyId = @CompanyId
                   ORDER BY Id";

                return (await connection.QueryAsync<SalaryHeads>(sql, new { subscriptionId, companyId })).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active salary heads");
                throw;
            }
        }
        public async Task<int> GetBranchIdFromDepartment(int departmentId)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var sql = "SELECT BranchId FROM Department WHERE Id = @DepartmentId";
                return await connection.QueryFirstOrDefaultAsync<int>(sql, new { DepartmentId = departmentId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting branch from department {DepartmentId}", departmentId);
                throw;
            }
        }

        public async Task<int> GetBranchIdFromDesignation(int designationId)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var sql = "SELECT BranchId FROM Designation WHERE Id = @DesignationId";
                return await connection.QueryFirstOrDefaultAsync<int>(sql, new { DesignationId = designationId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting branch from designation {DesignationId}", designationId);
                throw;
            }
        }

        public async Task<int> GetBranchIdFromEmployee(int employeeId)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var sql = "SELECT BranchId FROM Employees WHERE EmpId = @EmployeeId";
                return await connection.QueryFirstOrDefaultAsync<int>(sql, new { EmployeeId = employeeId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting branch from employee {EmployeeId}", employeeId);
                throw;
            }
        }
        public async Task<List<SalaryHeads>> GetSalaryHeadsByBranch(int branchId)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var subscriptionId = _baseService.GetSubscriptionId();
                var companyId = await _baseService.GetCompanyId(subscriptionId);

                var sql = @"SELECT * FROM SalaryHeads 
                   WHERE (BranchId = @BranchId OR BranchId IS NULL OR BranchId = 0)
                   AND SubscriptionId = @SubscriptionId AND CompanyId = @CompanyId
                   ORDER BY Id";

                return (await connection.QueryAsync<SalaryHeads>(sql, new
                {
                    BranchId = branchId,
                    SubscriptionId = subscriptionId,
                    CompanyId = companyId
                })).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving salary heads for branch {BranchId}", branchId);
                throw;
            }
        }
    }
}
