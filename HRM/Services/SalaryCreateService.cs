using Dapper;
using HRM.Interfaces;
using HRM.Models;
using Microsoft.Data.SqlClient;
using System.Data;

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

                    var query = @" WITH AttendanceSummary AS ( SELECT EmployeeId, COUNT(DISTINCT CONVERT(date, LoginDate)) AS PresentDays FROM Attendance WHERE DATENAME(MONTH, LoginDate) = @MonthName GROUP BY EmployeeId ), SalaryLatest AS ( SELECT EmployeeId, MAX(ApplyDate) AS LatestApplyDate FROM SalaryCalculation GROUP BY EmployeeId ), SalaryPivot AS ( SELECT sc.EmployeeId, MAX(CASE WHEN sc.Parameter = 'Basic Salary' THEN sc.Value END) AS [BASIC SALARY], MAX(CASE WHEN sc.Parameter = 'House Rent Allowance' THEN sc.Value END) AS [HOUSE RENT], MAX(CASE WHEN sc.Parameter = 'Medical Allowance' THEN sc.Value END) AS [MEDICAL ALLOW], MAX(CASE WHEN sc.Parameter = 'Conveyance' THEN sc.Value END) AS [Conveyance], MAX(CASE WHEN sc.Parameter = 'Provident Fund' THEN sc.Value END) AS [PF] FROM SalaryCalculation sc INNER JOIN SalaryLatest sl ON sl.EmployeeId = sc.EmployeeId AND sl.LatestApplyDate = sc.ApplyDate GROUP BY sc.EmployeeId ), LoanDeduction AS ( SELECT EmployeeId, TotalLoanInstallment FROM ( SELECT l.EmployeeId, li.Installment_Amount AS TotalLoanInstallment, ROW_NUMBER() OVER (PARTITION BY l.EmployeeId, l.LoanId ORDER BY li.DateOfInstallment) AS rn FROM LoanApproval l INNER JOIN LoanInstallment li ON l.LoanId = li.LoanId WHERE l.AppStatus = 'Approved' AND l.LoanIssued = 'Issued' AND (l.LoanCompleteStatus = 'OnGoing' OR l.LoanCompleteStatus <> 'LoanComplete') ) t WHERE rn = 1 ), BonusAmount AS ( SELECT EmployeeId, Max(BonusAmount) AS Bonus FROM BonusCalculate WHERE DATENAME(MONTH, BonusDate) = @MonthName GROUP BY EmployeeId ) SELECT e.EmpId AS EmployeeId, e.EmployeeName, b.Name AS BranchName, ISNULL(a.PresentDays, 0) AS Present, 22 AS WorkingDays, (22 - ISNULL(a.PresentDays, 0)) AS Absent, ROUND(ISNULL(s.[BASIC SALARY],0)/22 * ISNULL(a.PresentDays,0),0) AS BasicSalary, ISNULL(s.[HOUSE RENT], 0) AS HouseRent, ISNULL(s.[MEDICAL ALLOW], 0) AS MedicalAllowance, ISNULL(s.[Conveyance], 0) AS Conveyance, ISNULL(s.[PF], 0) AS ProvidentFund, ROUND( ISNULL(s.[BASIC SALARY],0)/22 * ISNULL(a.PresentDays,0) + ISNULL(s.[HOUSE RENT],0) + ISNULL(s.[MEDICAL ALLOW],0) + ISNULL(s.[Conveyance],0), 0 ) AS GrossSalary, CASE WHEN ( ISNULL(s.[BASIC SALARY],0) + ISNULL(s.[HOUSE RENT],0) + ISNULL(s.[MEDICAL ALLOW],0) + ISNULL(s.[Conveyance],0) ) >= 30000 THEN ROUND(( ISNULL(s.[BASIC SALARY],0) + ISNULL(s.[HOUSE RENT],0) + ISNULL(s.[MEDICAL ALLOW],0) + ISNULL(s.[Conveyance],0) ) * 0.05, 0) ELSE 0 END AS Tax, ISNULL(ld.TotalLoanInstallment, 0) AS LoanDeduction, ISNULL(bn.Bonus, 0) AS Bonus, ROUND( ( ISNULL(s.[BASIC SALARY],0)/22 * ISNULL(a.PresentDays,0) + ISNULL(s.[HOUSE RENT],0) + ISNULL(s.[MEDICAL ALLOW],0) + ISNULL(s.[Conveyance],0) ) - ISNULL(s.[PF],0) - CASE WHEN ( ISNULL(s.[BASIC SALARY],0) + ISNULL(s.[HOUSE RENT],0) + ISNULL(s.[MEDICAL ALLOW],0) + ISNULL(s.[Conveyance],0) ) >= 30000 THEN ( (ISNULL(s.[BASIC SALARY],0) + ISNULL(s.[HOUSE RENT],0) + ISNULL(s.[MEDICAL ALLOW],0) + ISNULL(s.[Conveyance],0)) * 0.05 ) ELSE 0 END - ISNULL(ld.TotalLoanInstallment, 0) + ISNULL(bn.Bonus, 0), 0) AS NetSalary FROM Employees e LEFT JOIN Branch b ON b.Id = e.BranchId LEFT JOIN AttendanceSummary a ON a.EmployeeId = e.EmpId LEFT JOIN SalaryPivot s ON s.EmployeeId = e.EmpId LEFT JOIN LoanDeduction ld ON ld.EmployeeId = e.EmpId LEFT JOIN BonusAmount bn ON bn.EmployeeId = e.EmpId WHERE e.BranchId = @BranchId AND e.SubscriptionId = @SubscriptionId ORDER BY e.EmpId;";

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


        //public async Task<List<SalaryCreate>> GetAllSalaryCreateAsync(int branchId, string monthName)
        //{
        //    try
        //    {
        //        using (var connection = new SqlConnection(_connectionString))
        //        {
        //            connection.Open();
        //            var subscriptionId = _baseService.GetSubscriptionId();
        //            var userId = _baseService.GetUserId();
        //            var branchIds = await _baseService.GetBranchId(subscriptionId, userId);

        //            var query = @"WITH AttendanceSummary AS ( SELECT EmployeeId, COUNT(DISTINCT CONVERT(date, LoginDate)) AS PresentDays FROM Attendance WHERE DATENAME(MONTH, LoginDate) = '"+ branchId +"' GROUP BY EmployeeId ), SalaryLatest AS ( SELECT EmployeeId, MAX(ApplyDate) AS LatestApplyDate FROM SalaryCalculation GROUP BY EmployeeId ), SalaryPivot AS ( SELECT sc.EmployeeId, MAX(CASE WHEN sc.Parameter = 'Basic Salary' THEN sc.Value END) AS [BASIC SALARY], MAX(CASE WHEN sc.Parameter = 'House Rent Allowance' THEN sc.Value END) AS [HOUSE RENT], MAX(CASE WHEN sc.Parameter = 'Medical Allowance' THEN sc.Value END) AS [MEDICAL ALLOW], MAX(CASE WHEN sc.Parameter = 'Conveyance' THEN sc.Value END) AS [Conveyance], MAX(CASE WHEN sc.Parameter = 'Provident Fund' THEN sc.Value END) AS [PF] FROM SalaryCalculation sc INNER JOIN SalaryLatest sl ON sl.EmployeeId = sc.EmployeeId AND sl.LatestApplyDate = sc.ApplyDate GROUP BY sc.EmployeeId ), LoanDeduction AS ( SELECT EmployeeId, TotalLoanInstallment FROM ( SELECT l.EmployeeId, li.Installment_Amount AS TotalLoanInstallment, ROW_NUMBER() OVER (PARTITION BY l.EmployeeId, l.LoanId ORDER BY li.DateOfInstallment) AS rn FROM LoanApproval l INNER JOIN LoanInstallment li ON l.LoanId = li.LoanId WHERE l.AppStatus = 'Approved' AND l.LoanIssued = 'Issued' AND (l.LoanCompleteStatus = 'OnGoing' OR l.LoanCompleteStatus <> 'LoanComplete') ) t WHERE rn = 1 ), BonusAmount AS ( SELECT EmployeeId, Max(BonusAmount) AS Bonus FROM BonusCalculate WHERE DATENAME(MONTH, BonusDate) = '"+ branchId + "' GROUP BY EmployeeId ) SELECT e.EmpId AS EmployeeId, e.EmployeeName, b.Name AS BranchName, ISNULL(a.PresentDays, 0) AS Present, 22 AS WorkingDays, (22 - ISNULL(a.PresentDays, 0)) AS Absent, ISNULL(s.[BASIC SALARY], 0) AS BasicSalary, ISNULL(s.[HOUSE RENT], 0) AS HouseRent, ISNULL(s.[MEDICAL ALLOW], 0) AS MedicalAllowance, ISNULL(s.[Conveyance], 0) AS Conveyance, ISNULL(s.[PF], 0) AS ProvidentFund, ROUND( ISNULL(s.[BASIC SALARY],0) + ISNULL(s.[HOUSE RENT],0) + ISNULL(s.[MEDICAL ALLOW],0) + ISNULL(s.[Conveyance],0), 0 ) AS GrossSalary, CASE WHEN ( ISNULL(s.[BASIC SALARY],0) + ISNULL(s.[HOUSE RENT],0) + ISNULL(s.[MEDICAL ALLOW],0) + ISNULL(s.[Conveyance],0) ) >= 30000 THEN ROUND(( ISNULL(s.[BASIC SALARY],0) + ISNULL(s.[HOUSE RENT],0) + ISNULL(s.[MEDICAL ALLOW],0) + ISNULL(s.[Conveyance],0) ) * 0.05, 0) ELSE 0 END AS Tax, ISNULL(ld.TotalLoanInstallment, 0) AS LoanDeduction, ISNULL(bn.Bonus, 0) AS Bonus, ROUND( ( ISNULL(s.[BASIC SALARY],0) + ISNULL(s.[HOUSE RENT],0) + ISNULL(s.[MEDICAL ALLOW],0) + ISNULL(s.[Conveyance],0) ) - ISNULL(s.[PF],0) - CASE WHEN ( ISNULL(s.[BASIC SALARY],0) + ISNULL(s.[HOUSE RENT],0) + ISNULL(s.[MEDICAL ALLOW],0) + ISNULL(s.[Conveyance],0) ) >= 30000 THEN ( (ISNULL(s.[BASIC SALARY],0) + ISNULL(s.[HOUSE RENT],0) + ISNULL(s.[MEDICAL ALLOW],0) + ISNULL(s.[Conveyance],0)) * 0.05 ) ELSE 0 END - ISNULL(ld.TotalLoanInstallment, 0) + ISNULL(bn.Bonus, 0), 0) AS NetSalary FROM Employees e LEFT JOIN Branch b ON b.Id = e.BranchId LEFT JOIN AttendanceSummary a ON a.EmployeeId = e.EmpId LEFT JOIN SalaryPivot s ON s.EmployeeId = e.EmpId LEFT JOIN LoanDeduction ld ON ld.EmployeeId = e.EmpId LEFT JOIN BonusAmount bn ON bn.EmployeeId = e.EmpId Where e.BranchId = '"+ branchId +"' AND e.SubscriptionId = '"+ subscriptionId +"' ORDER BY e.EmpId;";

        //            var result = await connection.QueryAsync<SalaryCreate>(query, new { subscriptionId, });
        //            return result.ToList();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}

        public async Task<bool> InsertSalaryCreateAsync(SalaryCreate salaryCreate)
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

                    var queryString = "insert into SalaryRegister (MonthIndex,Month,Year,FromDate,ToDate,GenDate,EmployeeId,Sl,Parameter,Value,FinalAmount,DesignationId,SalaryHeadsId,DepartmentId,AccountsPayable,BranchId,SubscriptionId,CompanyId,CreatedAt) values ";
                    queryString += "( @MonthIndex,@Month,@Year,@FromDate,@ToDate,@GenDate,@EmployeeId,@Sl,@Parameter,@Value,@FinalAmount,@DesignationId,@SalaryHeadsId,@DepartmentId,@AccountsPayable,@BranchId,@SubscriptionId,@CompanyId,@CreatedAt)";
                    var parameters = new DynamicParameters();
                    parameters.Add("MonthIndex", salaryCreate.Absent, DbType.String);
                    parameters.Add("Month", salaryCreate.Absent, DbType.String);
                    parameters.Add("Year", salaryCreate.Absent, DbType.String);
                    parameters.Add("FromDate", salaryCreate.Absent, DbType.String);
                    parameters.Add("ToDate", salaryCreate.Absent, DbType.String);
                    parameters.Add("GenDate", salaryCreate.Absent, DbType.String);
                    parameters.Add("EmployeeId", salaryCreate.Absent, DbType.String);
                    parameters.Add("Sl", salaryCreate.Absent, DbType.String);
                    parameters.Add("Parameter", salaryCreate.Absent, DbType.String);
                    parameters.Add("Value", salaryCreate.Absent, DbType.String);
                    parameters.Add("FinalAmount", salaryCreate.Absent, DbType.String);
                    parameters.Add("DesignationId", salaryCreate.Absent, DbType.String);
                    parameters.Add("SalaryHeadsId", salaryCreate.Absent, DbType.String);
                    parameters.Add("DepartmentId", salaryCreate.Absent, DbType.String);
                    parameters.Add("AccountsPayable", salaryCreate.Absent, DbType.String);
                    parameters.Add("Sl", salaryCreate.Absent, DbType.String);
                    parameters.Add("BranchId", salaryCreate.BranchId, DbType.Int64);
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
