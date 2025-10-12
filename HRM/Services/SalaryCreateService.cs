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
        public async Task<List<SalaryCreate>> GetAllSalaryCreateAsync(int empId)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var subscriptionId = _baseService.GetSubscriptionId();
                    var userId = _baseService.GetUserId();
                    var branchId = await _baseService.GetBranchId(subscriptionId, userId);

                    var query = @";WITH AttendanceSummary AS ( SELECT EmployeeId, COUNT(DISTINCT CONVERT(date, LoginDate)) AS PresentDays FROM Attendance GROUP BY EmployeeId ), SalaryPivot AS ( SELECT EmployeeId, MAX(CASE WHEN Parameter = 'Basic Salary' THEN Value END) AS [BASIC SALARY], MAX(CASE WHEN Parameter = 'House Rent' THEN Value END) AS [HOUSE RENT], MAX(CASE WHEN Parameter = 'Medical' THEN Value END) AS [MEDICAL ALLOW], MAX(CASE WHEN Parameter = 'TA-DA' THEN Value END) AS [TA-DA], MAX(CASE WHEN Parameter = 'Special Allowance' THEN Value END) AS [SPECIAL ALLOW], MAX(CASE WHEN Parameter = 'PF' THEN Value END) AS [PF], MAX(AccountsPayable) AS [TOTAL PAYABLE] FROM Salary GROUP BY EmployeeId ) SELECT e.EmpId AS [Employee ID], e.EmployeeName AS [Employee Name], b.Name AS [Posting Branch], ISNULL(a.PresentDays, 0) AS [Present], (22) AS [Working Days], (22 - ISNULL(a.PresentDays, 0)) AS [Absent], 0 AS [Non Pay (Days)], ISNULL(s.[BASIC SALARY], 0) AS [BASIC SALARY], ISNULL(s.[HOUSE RENT], 0) AS [HOUSE RENT], ISNULL(s.[MEDICAL ALLOW], 0) AS [MEDICAL ALLOW], ISNULL(s.[TA-DA], 0) AS [TA-DA], ISNULL(s.[SPECIAL ALLOW], 0) AS [SPECIAL ALLOW], ISNULL(s.[PF], 0) AS [PF], ROUND( ISNULL(s.[BASIC SALARY],0) + ISNULL(s.[HOUSE RENT],0) + ISNULL(s.[MEDICAL ALLOW],0) + ISNULL(s.[TA-DA],0) + ISNULL(s.[SPECIAL ALLOW],0) + ISNULL(s.[PF],0), 0 ) AS [TOTAL], ISNULL(s.[TOTAL PAYABLE],0) AS [Amount Payable] FROM Employees e LEFT JOIN Branch b ON b.Id = e.BranchId LEFT JOIN AttendanceSummary a ON a.EmployeeId = e.EmpId LEFT JOIN SalaryPivot s ON s.EmployeeId = e.EmpId ORDER BY e.EmpId;";

                    var result = await connection.QueryAsync<SalaryCreate>(query, new { subscriptionId });
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
