using Dapper;
using HRM.Interfaces;
using HRM.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System.ComponentModel.Design;
using System.Data;

namespace HRM.Services
{
    public class RecommendLoanApplicationService : IRecommendLoanApplicationService
    {
        private readonly string _connectionString;
        private readonly BaseService _baseService;

        public RecommendLoanApplicationService(IConfiguration configuration, BaseService baseService)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(_connectionString));
            _baseService = baseService;
        }
        public async Task<List<RecommendLoanApplication>> GetAllRecommendLoanApplication()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var subscriptionId = _baseService.GetSubscriptionId();
                    var userId = _baseService.GetUserId();
                    var branchId = await _baseService.GetBranchId(subscriptionId, userId);
                    var loginid = _baseService.GetUserId();

                    var query = @"SELECT t1.Id,t2.EmpId AS EmployeeId, t2.EmployeeName AS EmployeeName, t1.AppliDate AS AppliDate, (SELECT t5.Name FROM Branch t5 WHERE t5.Id = t2.BranchId) AS Branch, (SELECT t3.DesignationName FROM Designation t3 WHERE t3.Id = t2.DesignationId) AS Designation, (SELECT t4.DepartmentName FROM Department t4 WHERE t4.Id = t2.DepartmentId) AS Department, t1.AmountLoan AS AmountLoan, (SELECT t6.Value FROM Salary t6 WHERE t6.EmployeeId = t2.EmpId AND t6.Parameter = 'Basic Salary' AND t6.Month = DATENAME(MONTH, GETDATE()) AND t6.Year = YEAR(GETDATE()) ) AS BasicSalary,t1.Grade as Grade,CAST(DATEDIFF(YEAR, t2.DateOfAppointment, GETDATE()) AS VARCHAR(10)) + ' Years ' + CAST(DATEDIFF(MONTH, DATEADD(YEAR, DATEDIFF(YEAR, t2.DateOfAppointment, GETDATE()), t2.DateOfAppointment), GETDATE()) AS VARCHAR(10)) + ' Months ' + CAST(DATEDIFF(DAY, DATEADD(MONTH, DATEDIFF(MONTH, t2.DateOfAppointment, GETDATE()), t2.DateOfAppointment), GETDATE()) AS VARCHAR(10)) + ' Days' AS Duration,t1.RecommendedAmount as RecommendedAmount,t1.RecommendDate as RecommendDate,t1.PreviousLoan as PreviousLoan,t1.Repaid as Repaid,t1.Hr_Rem as Hr_Rem,t1.LoanApproved as LoanApproved,t1.LoanAppDate as LoanAppDate,t1.Term as Term,t1.interest as interest,t1.AppStatus as AppStatus,t1.LoanIssued as LoanIssued,t1.LoanId as LoanId FROM LoanApproval t1 JOIN Employees t2 ON t2.EmpId = t1.EmployeeId WHERE  t2.SubscriptionId=@subscriptionId ";

                    var result = await connection.QueryAsync<RecommendLoanApplication>(query, new { subscriptionId = subscriptionId });
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public async Task<List<RecommendLoanApplication>> SearchData(RecommendLoanApplication recommendLoanApplication)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var subscriptionId = _baseService.GetSubscriptionId();
                    var userId = _baseService.GetUserId();
                    var branchId = await _baseService.GetBranchId(subscriptionId, userId);

                    if (string.IsNullOrEmpty(recommendLoanApplication.FromDate) || string.IsNullOrEmpty(recommendLoanApplication.ToDate))
                    {
                        return new List<RecommendLoanApplication>();
                    }

                    var query = @"SELECT t1.Id, t2.EmpId AS EmployeeId, t2.EmployeeName AS EmployeeName, t1.AppliDate AS AppliDate, (SELECT t5.Name FROM Branch t5 WHERE t5.Id = t2.BranchId) AS Branch, (SELECT t3.DesignationName FROM Designation t3 WHERE t3.Id = t2.DesignationId) AS Designation, (SELECT t4.DepartmentName FROM Department t4 WHERE t4.Id = t2.DepartmentId) AS Department, t1.AmountLoan AS AmountLoan, (SELECT t6.Value FROM Salary t6 WHERE t6.EmployeeId = t2.EmpId AND t6.Parameter = 'Basic Salary' AND t6.Month = DATENAME(MONTH, GETDATE()) AND t6.Year = YEAR(GETDATE()) ) AS BasicSalary, t1.Grade AS Grade, CAST(DATEDIFF(YEAR, t2.DateOfAppointment, GETDATE()) AS VARCHAR(10)) + ' Years ' + CAST(DATEDIFF(MONTH, DATEADD(YEAR, DATEDIFF(YEAR, t2.DateOfAppointment, GETDATE()), t2.DateOfAppointment), GETDATE()) AS VARCHAR(10)) + ' Months ' + CAST(DATEDIFF(DAY, DATEADD(MONTH, DATEDIFF(MONTH, t2.DateOfAppointment, GETDATE()), t2.DateOfAppointment), GETDATE()) AS VARCHAR(10)) + ' Days' AS Duration, t1.RecommendedAmount AS RecommendedAmount, t1.RecommendDate AS RecommendDate, t1.PreviousLoan AS PreviousLoan, t1.Repaid AS Repaid, t1.Hr_Rem AS Hr_Rem, t1.LoanApproved AS LoanApproved, t1.LoanAppDate AS LoanAppDate, t1.Term AS Term, t1.interest AS interest, t1.AppStatus AS AppStatus, t1.LoanIssued AS LoanIssued, t1.LoanId AS LoanId FROM LoanApproval t1 JOIN Employees t2 ON t2.EmpId = t1.EmployeeId WHERE t1.SubscriptionId = @SubscriptionId AND t1.AppliDate >= @FromDate AND t1.AppliDate < DATEADD(DAY, 1, @ToDate) and t1.RecommendDate is NULL ";

                    var result = await connection.QueryAsync<RecommendLoanApplication>(
                        query,
                        new
                        {
                            SubscriptionId = subscriptionId,
                            FromDate = Convert.ToDateTime(recommendLoanApplication.FromDate),
                            ToDate = Convert.ToDateTime(recommendLoanApplication.ToDate)
                        });

                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching loan application data", ex);
            }
        }




        public async Task<bool> UpdateRecommendLoanApplication(RecommendLoanApplication recommendLoanApplication)
        {
            try
            {
                using (var connection=new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var query = @"UPDATE LoanApproval SET Grade=@Grade, RecommendedAmount=@RecommendedAmount,RecommendDate=@RecommendDate,Hr_Rem=@Hr_Rem WHERE Id='"+ recommendLoanApplication.Id + "'";
                    var parameters = new DynamicParameters();
                    parameters.Add("Grade", recommendLoanApplication.Grade, DbType.String);
                    parameters.Add("RecommendedAmount", recommendLoanApplication.RecommendedAmount, DbType.String);
                    parameters.Add("RecommendDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DbType.String);
                    parameters.Add("Hr_Rem", recommendLoanApplication.Hr_Rem, DbType.String);
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
