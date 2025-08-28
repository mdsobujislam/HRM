using Dapper;
using HRM.Interfaces;
using HRM.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace HRM.Services
{
    public class EmployeeLoanService : IEmployeeLoanService
    {
        private readonly string _connectionString;
        private readonly BaseService _baseService;

        public EmployeeLoanService(IConfiguration configuration, BaseService baseService)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(_connectionString));
            _baseService = baseService;
        }
        public async Task<IEnumerable<EmployeeLoan>> GetAllEmployeeLoansAsync()
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

                    var query = @"Select t1.EmployeeId,t1.DateOfLoan from EmployeeLoan t1 WHERE t2.SubscriptionId=@subscriptionId ";

                    var result = await connection.QueryAsync<EmployeeLoan>(query, new { subscriptionId = subscriptionId });
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> InsertEmployeeLoanAsync(EmployeeLoan employeeLoan)
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

                    var loginid = _baseService.GetUserId();
                    //var employeDetailsQuery = "SELECT Name FROM Employees WHERE UserId = @UserId AND SubscriptionId = @SubscriptionId";

                    var queryString = "insert into EmployeeLoan (EmployeeId,DateOfLoan,LoanAgainst,LoanPercentage,LoanAmount,Interest,InterestAmount,Term,PerMonthInterest,EmiPrinciple,Emi,ApplicationId,BranchId,SubscriptionId,CompanyId) values ";
                    queryString += "( @EmployeeId,@DateOfLoan,@LoanAgainst,@LoanPercentage,@LoanAmount,@Interest,@InterestAmount,@Term,@PerMonthInterest,@EmiPrinciple,@Emi,@ApplicationId,@BranchId,@SubscriptionId,@CompanyId)";
                    var parameters = new DynamicParameters();
                    parameters.Add("EmployeeId", loginid);
                    parameters.Add("AppliDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DbType.String);
                    //parameters.Add("AmountLoan", loanApplication.AmountLoan, DbType.String);
                    //parameters.Add("Remarks", loanApplication.Remarks, DbType.String);
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

                    var query = @"SELECT t1.Id as Id, t2.EmpId AS EmployeeId, t2.EmployeeName AS EmployeeName, t1.AppliDate AS AppliDate, (SELECT t5.Name FROM Branch t5 WHERE t5.Id = t2.BranchId) AS Branch, t1.id AS LoanId,t1.LoanApproved as LoanApproved,t1.interest as interest,t1.Term as Term  FROM LoanApproval t1 JOIN Employees t2 ON t2.EmpId = t1.EmployeeId WHERE t1.SubscriptionId = @SubscriptionId AND t1.AppliDate >= @FromDate AND t1.AppliDate < DATEADD(DAY, 1, @ToDate)";

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

        public Task<bool> UpdateEmployeeLoanAsync(EmployeeLoan employeeLoan)
        {
            throw new NotImplementedException();
        }
    }
}
