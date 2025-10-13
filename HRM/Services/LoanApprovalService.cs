using Dapper;
using HRM.Interfaces;
using HRM.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace HRM.Services
{
    public class LoanApprovalService : ILoanApprovalService
    {
        private readonly string _connectionString;
        private readonly BaseService _baseService;

        public LoanApprovalService(IConfiguration configuration, BaseService baseService)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(_connectionString));
            _baseService = baseService;
        }
        public async Task<List<LoanApproval>> SearchData(LoanApproval loanApproval)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var subscriptionId = _baseService.GetSubscriptionId();
                    var userId = _baseService.GetUserId();
                    var branchId = await _baseService.GetBranchId(subscriptionId, userId);

                    if (string.IsNullOrEmpty(loanApproval.FromDate) || string.IsNullOrEmpty(loanApproval.ToDate))
                    {
                        return new List<LoanApproval>();
                    }

                    var query = @"SELECT t1.Id, t2.EmpId AS EmployeeId, t2.EmployeeName AS EmployeeName, t1.AppliDate AS AppliDate, (SELECT t5.Name FROM Branch t5 WHERE t5.Id = t2.BranchId) AS Branch, (SELECT t3.DesignationName FROM Designation t3 WHERE t3.Id = t2.DesignationId) AS Designation, (SELECT t4.DepartmentName FROM Department t4 WHERE t4.Id = t2.DepartmentId) AS Department, t1.AmountLoan AS AmountLoan, (SELECT t6.Value FROM Salary t6 WHERE t6.EmployeeId = t2.EmpId AND t6.Parameter = 'Basic Salary' AND t6.Month = DATENAME(MONTH, GETDATE()) AND t6.Year = YEAR(GETDATE()) ) AS BasicSalary, t1.Grade AS Grade, CAST(DATEDIFF(YEAR, t2.DateOfAppointment, GETDATE()) AS VARCHAR(10)) + ' Years ' + CAST(DATEDIFF(MONTH, DATEADD(YEAR, DATEDIFF(YEAR, t2.DateOfAppointment, GETDATE()), t2.DateOfAppointment), GETDATE()) AS VARCHAR(10)) + ' Months ' + CAST(DATEDIFF(DAY, DATEADD(MONTH, DATEDIFF(MONTH, t2.DateOfAppointment, GETDATE()), t2.DateOfAppointment), GETDATE()) AS VARCHAR(10)) + ' Days' AS Duration, t1.RecommendedAmount AS RecommendedAmount, t1.RecommendDate AS RecommendDate, t1.PreviousLoan AS PreviousLoan, t1.Repaid AS Repaid, t1.Hr_Rem AS Hr_Rem, t1.LoanApproved AS LoanApproved, t1.LoanAppDate AS LoanAppDate, t1.Term AS Term, t1.interest AS interest, t1.AppStatus AS AppStatus, t1.LoanIssued AS LoanIssued, t1.LoanId AS LoanId FROM LoanApproval t1 JOIN Employees t2 ON t2.EmpId = t1.EmployeeId WHERE t1.SubscriptionId = @SubscriptionId AND t1.AppliDate >= @FromDate AND t1.AppliDate < DATEADD(DAY, 1, @ToDate) and t1.LoanAppDate is NULL ";

                    var result = await connection.QueryAsync<LoanApproval>(
                        query,
                        new
                        {
                            SubscriptionId = subscriptionId,
                            FromDate = Convert.ToDateTime(loanApproval.FromDate),
                            ToDate = Convert.ToDateTime(loanApproval.ToDate)
                        });

                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching loan application data", ex);
            }
        }

        public async Task<bool> UpdateLoanApproval(LoanApproval loanApproval)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var query = @"UPDATE LoanApproval SET LoanApproved=@LoanApproved,LoanAppDate=@LoanAppDate,Term=@Term,interest=@interest,AppStatus=@AppStatus,LoanIssued=@LoanIssued,LoanCompleteStatus=@LoanCompleteStatus WHERE Id='" + loanApproval.Id + "'";
                    var parameters = new DynamicParameters();
                    parameters.Add("LoanApproved", loanApproval.LoanApproved, DbType.String);
                    parameters.Add("LoanAppDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DbType.String);
                    parameters.Add("Term", loanApproval.Term, DbType.Int64);
                    parameters.Add("interest", loanApproval.interest, DbType.Int64);
                    parameters.Add("AppStatus", "Approved", DbType.String);
                    parameters.Add("LoanIssued", "Issued", DbType.String);
                    parameters.Add("LoanCompleteStatus", "OnGonig", DbType.String);
                    var success = await connection.ExecuteAsync(query, parameters);
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
