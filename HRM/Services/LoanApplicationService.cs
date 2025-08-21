using Dapper;
using HRM.Interfaces;
using HRM.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace HRM.Services
{
    public class LoanApplicationService : ILoanApplicationService
    {
        private readonly string _connectionString;
        private readonly BaseService _baseService;

        public LoanApplicationService(IConfiguration configuration, BaseService baseService)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(_connectionString));
            _baseService = baseService;
        }
        public Task<bool> DeleteLoanApplication(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<LoanApplication>> GetAllLoanApplication()
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

                    var query = @"SELECT t2.EmpId AS EmployeeId, t2.EmployeeName AS EmployeeName, t1.AppliDate AS AppliDate, (SELECT t5.Name FROM Branch t5 WHERE t5.Id = t2.BranchId) AS Branch, (SELECT t3.DesignationName FROM Designation t3 WHERE t3.Id = t2.DesignationId) AS Designation, (SELECT t4.DepartmentName FROM Department t4 WHERE t4.Id = t2.DepartmentId) AS Department, t1.AmountLoan AS AmountLoan, (SELECT t6.Value FROM Salary t6 WHERE t6.EmployeeId = t2.EmpId AND t6.Parameter = 'Basic Salary' AND t6.Month = DATENAME(MONTH, GETDATE()) AND t6.Year = YEAR(GETDATE()) ) AS BasicSalary,CAST(DATEDIFF(YEAR, t2.DateOfAppointment, GETDATE()) AS VARCHAR(10)) + ' Years ' + CAST(DATEDIFF(MONTH, DATEADD(YEAR, DATEDIFF(YEAR, t2.DateOfAppointment, GETDATE()), t2.DateOfAppointment), GETDATE()) AS VARCHAR(10)) + ' Months ' + CAST(DATEDIFF(DAY, DATEADD(MONTH, DATEDIFF(MONTH, t2.DateOfAppointment, GETDATE()), t2.DateOfAppointment), GETDATE()) AS VARCHAR(10)) + ' Days' AS Duration,t1.RecommendedAmount as RecommendedAmount,t1.RecommendDate as RecommendDate,t1.PreviousLoan as PreviousLoan,t1.Repaid as Repaid,t1.Hr_Rem as Hr_Rem,t1.LoanApproved as LoanApproved,t1.LoanAppDate as LoanAppDate,t1.Term as Term,t1.interest as interest,t1.AppStatus as AppStatus,t1.LoanIssued as LoanIssued,t1.LoanId as LoanId FROM LoanApproval t1 JOIN Employees t2 ON t2.EmpId = t1.EmployeeId WHERE t2.EmpId = '"+ loginid + "' and t2.SubscriptionId=@subscriptionId ";

                    var result = await connection.QueryAsync<LoanApplication>(query, new { subscriptionId= subscriptionId });
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> InsertLoanApplication(LoanApplication loanApplication)
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

                    var loginid= _baseService.GetUserId();
                    //var employeDetailsQuery = "SELECT Name FROM Employees WHERE UserId = @UserId AND SubscriptionId = @SubscriptionId";

                    var queryString = "insert into LoanApproval (EmployeeId,AppliDate,AmountLoan,Remarks,BranchId,SubscriptionId,CompanyId) values ";
                    queryString += "( @EmployeeId,@AppliDate,@AmountLoan,@Remarks,@BranchId,@SubscriptionId,@CompanyId)";
                    var parameters = new DynamicParameters();
                    parameters.Add("EmployeeId", loginid);
                    parameters.Add("AppliDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DbType.String);
                    parameters.Add("AmountLoan", loanApplication.AmountLoan, DbType.String);
                    parameters.Add("Remarks", loanApplication.Remarks, DbType.String);
                    parameters.Add("BranchId", branchId);
                    parameters.Add("SubscriptionId", subscriptionId);
                    parameters.Add("CompanyId", companyId);
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

        public Task<bool> UpdateLoanApplication(LoanApplication loanApplication)
        {
            throw new NotImplementedException();
        }
    }
}
