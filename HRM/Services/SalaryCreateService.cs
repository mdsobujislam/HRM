using Dapper;
using HRM.Interfaces;
using HRM.Models;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Globalization;
using System.Reflection;

namespace HRM.Services
{
    public class SalaryCreateService : ISalaryCreateService
    {
        private readonly string _connectionString;
        private readonly BaseService _baseService;

        public SalaryCreateService(IConfiguration configuration, BaseService baseService)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(_connectionString));
            _baseService = baseService;
        }

        public async Task<List<SalaryCreate>> GetAllSalaryCreateAsync(int branchId, string monthName)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var subscriptionId = _baseService.GetSubscriptionId();
                    var userId = _baseService.GetUserId();

                    var currentYear = DateTime.Now.Year;

                    var queryExisting = @"SELECT COUNT(1) FROM SalaryRegister 
                      WHERE BranchId = @BranchId AND Month = @Month AND Year = @Year";

                    var exists = await connection.ExecuteScalarAsync<int>(queryExisting, new
                    {
                        BranchId = branchId,
                        Month = monthName,
                        Year = currentYear
                    });

                    if (exists > 0)
                    {
                        return new List<SalaryCreate>();
                    }



                    var query = @" WITH AttendanceSummary AS ( SELECT UserIdFromDevice AS EmployeeId, COUNT(DISTINCT CONVERT(date, DeviceTimestamp)) AS PresentDays FROM AttendanceRecords WHERE DATENAME(MONTH, DeviceTimestamp) = @MonthName GROUP BY UserIdFromDevice ), SalaryLatest AS ( SELECT EmployeeId, MAX(ApplyDate) AS LatestApplyDate FROM SalaryCalculation GROUP BY EmployeeId ), SalaryPivot AS ( SELECT sc.EmployeeId, MAX(CASE WHEN sc.Parameter = 'Basic Salary' THEN sc.Value END) AS [BASIC SALARY], MAX(CASE WHEN sc.Parameter = 'House Rent Allowance' THEN sc.Value END) AS [HOUSE RENT], MAX(CASE WHEN sc.Parameter = 'Medical Allowance' THEN sc.Value END) AS [MEDICAL ALLOW], MAX(CASE WHEN sc.Parameter = 'Conveyance' THEN sc.Value END) AS [Conveyance], MAX(CASE WHEN sc.Parameter = 'Provident Fund' THEN sc.Value END) AS [PF] FROM SalaryCalculation sc INNER JOIN SalaryLatest sl ON sl.EmployeeId = sc.EmployeeId AND sl.LatestApplyDate = sc.ApplyDate GROUP BY sc.EmployeeId ), LoanDeduction AS ( SELECT EmployeeId, TotalLoanInstallment FROM ( SELECT l.EmployeeId, li.Installment_Amount AS TotalLoanInstallment, ROW_NUMBER() OVER (PARTITION BY l.EmployeeId, l.LoanId ORDER BY li.DateOfInstallment) AS rn FROM LoanApproval l INNER JOIN LoanInstallment li ON l.LoanId = li.LoanId WHERE l.AppStatus = 'Approved' AND l.LoanIssued = 'Issued' AND (l.LoanCompleteStatus = 'OnGoing' OR l.LoanCompleteStatus <> 'LoanComplete') ) t WHERE rn = 1 ), BonusAmount AS ( SELECT EmployeeId, MAX(BonusAmount) AS Bonus FROM BonusCalculate WHERE DATENAME(MONTH, BonusDate) = @MonthName GROUP BY EmployeeId ) SELECT e.EmpId AS EmployeeId, e.EmployeeName, b.Name AS BranchName, ISNULL(a.PresentDays, 0) AS Present, 22 AS WorkingDays, (22 - ISNULL(a.PresentDays, 0)) AS Absent, ROUND(ISNULL(s.[BASIC SALARY],0)/22 * ISNULL(a.PresentDays,0),0) AS BasicSalary, ISNULL(s.[HOUSE RENT], 0) AS HouseRent, ISNULL(s.[MEDICAL ALLOW], 0) AS MedicalAllowance, ISNULL(s.[Conveyance], 0) AS Conveyance, ISNULL(s.[PF], 0) AS ProvidentFund, ROUND( ISNULL(s.[BASIC SALARY],0)/22 * ISNULL(a.PresentDays,0) + ISNULL(s.[HOUSE RENT],0) + ISNULL(s.[MEDICAL ALLOW],0) + ISNULL(s.[Conveyance],0), 0 ) AS GrossSalary, CASE WHEN ( ISNULL(s.[BASIC SALARY],0) + ISNULL(s.[HOUSE RENT],0) + ISNULL(s.[MEDICAL ALLOW],0) + ISNULL(s.[Conveyance],0) ) >= 30000 THEN ROUND( ( ISNULL(s.[BASIC SALARY],0) + ISNULL(s.[HOUSE RENT],0) + ISNULL(s.[MEDICAL ALLOW],0) + ISNULL(s.[Conveyance],0) ) * 0.05, 0 ) ELSE 0 END AS Tax, ISNULL(ld.TotalLoanInstallment, 0) AS LoanDeduction, ISNULL(bn.Bonus, 0) AS Bonus, ROUND( ( ISNULL(s.[BASIC SALARY],0)/22 * ISNULL(a.PresentDays,0) + ISNULL(s.[HOUSE RENT],0) + ISNULL(s.[MEDICAL ALLOW],0) + ISNULL(s.[Conveyance],0) ) - ISNULL(s.[PF],0) - CASE WHEN ( ISNULL(s.[BASIC SALARY],0) + ISNULL(s.[HOUSE RENT],0) + ISNULL(s.[MEDICAL ALLOW],0) + ISNULL(s.[Conveyance],0) ) >= 30000 THEN ( (ISNULL(s.[BASIC SALARY],0) + ISNULL(s.[HOUSE RENT],0) + ISNULL(s.[MEDICAL ALLOW],0) + ISNULL(s.[Conveyance],0) ) * 0.05 ) ELSE 0 END - ISNULL(ld.TotalLoanInstallment, 0) + ISNULL(bn.Bonus, 0), 0 ) AS NetSalary FROM Employees e LEFT JOIN Branch b ON b.Id = e.BranchId LEFT JOIN AttendanceSummary a ON a.EmployeeId = e.EmpId LEFT JOIN SalaryPivot s ON s.EmployeeId = e.EmpId LEFT JOIN LoanDeduction ld ON ld.EmployeeId = e.EmpId LEFT JOIN BonusAmount bn ON bn.EmployeeId = e.EmpId WHERE e.BranchId = @BranchId AND e.SubscriptionId = @SubscriptionId ORDER BY e.EmpId;";

                    var result = await connection.QueryAsync<SalaryCreate>(query, new
                    {
                        BranchId = branchId,
                        MonthName = monthName,
                        SubscriptionId = subscriptionId
                    });

                    return result.ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> InsertSalaryListAsync(IEnumerable<SalaryCreate> salaryList)
        {
            if (salaryList == null || !salaryList.Any()) return false;

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var subscriptionId = _baseService.GetSubscriptionId();
                        var userId = _baseService.GetUserId();
                        var companyId = await _baseService.GetCompanyId(subscriptionId);

                        // 1) Load SalaryHeads for this SubscriptionId (dynamic)
                        string headQuery = @"SELECT Id, Salaryitems FROM SalaryHeads WHERE SubscriptionId = @SubscriptionId ORDER BY Id";
                        var salaryHeads = (await connection.QueryAsync<dynamic>(headQuery, new { SubscriptionId = subscriptionId }, transaction)).ToList();

                        if (!salaryHeads.Any())
                        {
                            // যদি SalaryHeads না থাকে, rollback ও false return
                            transaction.Rollback();
                            return false;
                        }

                        // 2) Prepare insert query
                        string insertQuery = @"
                            INSERT INTO SalaryRegister
                            (MonthIndex, Month, Year, FromDate, ToDate, GenDate, EmployeeId,
                             BranchId, CompanyId, SubscriptionId, Sl, Parameter, Value, FinalAmount,
                             TxId, RefTxId, DesignationId, SalaryHeadsId, DepartmentId, AccountsPayable)
                            VALUES
                            (@MonthIndex, @Month, @Year, @FromDate, @ToDate, @GenDate, @EmployeeId,
                             @BranchId, @CompanyId, @SubscriptionId, @Sl, @Parameter, @Value,
                             @FinalAmount, @TxId, @RefTxId, @DesignationId, @SalaryHeadsId, @DepartmentId,
                             @AccountsPayable)";

                        // 3) For each employee, insert rows for each SalaryHead
                        foreach (var emp in salaryList)
                        {
                            int sl = 1;
                            // If BranchId not provided from model, try to get default branch
                            int branchId = emp.BranchId > 0 ? emp.BranchId : await _baseService.GetBranchId(subscriptionId, userId);

                            foreach (var head in salaryHeads)
                            {
                                string headName = Convert.ToString(head.Salaryitems);
                                int headId = (int)head.Id;

                                // Map head name -> model property name (dynamic mapping)
                                string propName = MapSalaryItemToProperty(headName);
                                double value = GetPropertyValue(emp, propName);

                                var parameters = new DynamicParameters();
                                parameters.Add("@MonthIndex", DateTime.Now.Month, DbType.Int32);
                                parameters.Add("@Month", DateTime.Now.ToString("MMMM", CultureInfo.InvariantCulture), DbType.String);
                                parameters.Add("@Year", DateTime.Now.Year, DbType.Int32);
                                parameters.Add("@FromDate", new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1), DbType.DateTime);
                                parameters.Add("@ToDate", DateTime.Now, DbType.DateTime);
                                parameters.Add("@GenDate", DateTime.Now, DbType.DateTime);

                                parameters.Add("@EmployeeId", emp.EmployeeId, DbType.String);
                                parameters.Add("@BranchId", branchId, DbType.Int32);
                                parameters.Add("@CompanyId", companyId, DbType.Int32);
                                parameters.Add("@SubscriptionId", subscriptionId, DbType.Int32);

                                parameters.Add("@Sl", sl++, DbType.Int32);
                                parameters.Add("@Parameter", headName, DbType.String);
                                parameters.Add("@Value", value, DbType.Decimal);

                                // FinalAmount — for NetSalary we can set NetSalary, otherwise same as value
                                double finalAmount = headName.Equals("Net Salary", StringComparison.OrdinalIgnoreCase) ||
                                                     headName.Equals("NetSalary", StringComparison.OrdinalIgnoreCase)
                                                     ? emp.NetSalary
                                                     : value;

                                parameters.Add("@FinalAmount", finalAmount, DbType.Decimal);

                                parameters.Add("@TxId", null);
                                parameters.Add("@RefTxId", null);
                                parameters.Add("@DesignationId", 0);
                                parameters.Add("@SalaryHeadsId", headId);
                                parameters.Add("@DepartmentId", 0);
                                parameters.Add("@AccountsPayable", emp.AmountPayable, DbType.Decimal);

                                await connection.ExecuteAsync(insertQuery, parameters, transaction);
                            }
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        // Map SalaryHeads text to SalaryCreate property names
        // Extend this mapping as your SalaryHeads rows grow
        private string MapSalaryItemToProperty(string item)
        {
            if (string.IsNullOrWhiteSpace(item)) return null;

            item = item.Trim().ToLowerInvariant();

            // Add more cases if you have different naming in SalaryHeads
            return item switch
            {
                // common variations
                "basic salary" => nameof(SalaryCreate.BasicSalary),
                "basic" => nameof(SalaryCreate.BasicSalary),

                "house rent allowance" => nameof(SalaryCreate.HouseRent),
                "house rent" => nameof(SalaryCreate.HouseRent),
                "houserent" => nameof(SalaryCreate.HouseRent),

                "medical allowance" => nameof(SalaryCreate.MedicalAllowance),
                "medical" => nameof(SalaryCreate.MedicalAllowance),

                "conveyance" => nameof(SalaryCreate.Conveyance),

                "special allowance" => nameof(SalaryCreate.SPECIALALLOW),
                "specialallow" => nameof(SalaryCreate.SPECIALALLOW),

                "provident fund" => nameof(SalaryCreate.ProvidentFund),
                "pf" => nameof(SalaryCreate.ProvidentFund),

                "income tax" => nameof(SalaryCreate.IncomeTax),
                "tax" => nameof(SalaryCreate.Tax),

                "loan deduction" => nameof(SalaryCreate.LoanDeduction),
                "loan" => nameof(SalaryCreate.LoanDeduction),

                "bonus" => nameof(SalaryCreate.Bonus),

                "gross salary" => nameof(SalaryCreate.GrossSalary),
                "grosssalary" => nameof(SalaryCreate.GrossSalary),

                "net salary" => nameof(SalaryCreate.NetSalary),
                "netsalary" => nameof(SalaryCreate.NetSalary),

                "accounts payable" => nameof(SalaryCreate.AmountPayable),
                "accountspayable" => nameof(SalaryCreate.AmountPayable),

                _ => null // যদি মেলে না, then treated as 0
            };
        }

        // Reflection based getter — safe & culture invariant
        private double GetPropertyValue(SalaryCreate model, string propertyName)
        {
            if (model == null || string.IsNullOrEmpty(propertyName)) return 0;

            PropertyInfo prop = model.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (prop == null) return 0;

            object val = prop.GetValue(model);
            if (val == null) return 0;

            try
            {
                // handle numeric conversion safely
                return Convert.ToDouble(val, CultureInfo.InvariantCulture);
            }
            catch
            {
                return 0;
            }
        }

        public async Task<List<SalaryCreate>> GetSalaryReportCreateAsync(int branchId, string monthName)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var subscriptionId = _baseService.GetSubscriptionId();
                    var userId = _baseService.GetUserId();



                    var query = @" WITH SalaryLatest AS ( SELECT EmployeeId, MAX(GenDate) AS LatestGenDate FROM SalaryRegister WHERE Month = 'november' GROUP BY EmployeeId ), SalaryPivot AS ( SELECT sr.EmployeeId, MAX(CASE WHEN sr.Parameter = 'Basic Salary' THEN sr.Value END) AS BasicSalary, MAX(CASE WHEN sr.Parameter = 'House Rent Allowance' THEN sr.Value END) AS HouseRent, MAX(CASE WHEN sr.Parameter = 'Medical Allowance' THEN sr.Value END) AS MedicalAllowance, MAX(CASE WHEN sr.Parameter = 'Conveyance' THEN sr.Value END) AS Conveyance, MAX(CASE WHEN sr.Parameter = 'Provident Fund' THEN sr.Value END) AS ProvidentFund FROM SalaryRegister sr INNER JOIN SalaryLatest sl ON sl.EmployeeId = sr.EmployeeId AND sl.LatestGenDate = sr.GenDate WHERE sr.Month = 'november' GROUP BY sr.EmployeeId ) SELECT e.EmpId AS EmployeeId, e.EmployeeName, b.Name AS BranchName, sp.BasicSalary, sp.HouseRent, sp.MedicalAllowance, sp.Conveyance, sp.ProvidentFund, ISNULL(sp.BasicSalary, 0) + ISNULL(sp.HouseRent, 0) + ISNULL(sp.MedicalAllowance, 0) + ISNULL(sp.Conveyance, 0) AS GrossSalary, ISNULL(sp.BasicSalary, 0) + ISNULL(sp.HouseRent, 0) + ISNULL(sp.MedicalAllowance, 0) + ISNULL(sp.Conveyance, 0) - ISNULL(sp.ProvidentFund, 0) AS NetSalary FROM Employees e LEFT JOIN Branch b ON b.Id = e.BranchId LEFT JOIN SalaryPivot sp ON sp.EmployeeId = e.EmpId WHERE e.BranchId = 3 ORDER BY e.EmpId;";

                    var result = await connection.QueryAsync<SalaryCreate>(query, new
                    {
                        BranchId = branchId,
                        MonthName = monthName,
                        SubscriptionId = subscriptionId
                    });

                    return result.ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


    }
}
