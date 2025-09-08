using Dapper;
using HRM.Interfaces;
using HRM.Models;
using Microsoft.Data.SqlClient;

namespace HRM.Services
{
    public class LoanInstallmentService : ILoanInstallmentService
    {
        private readonly string _connectionString;
        private readonly BaseService _baseService;

        public LoanInstallmentService(IConfiguration configuration, BaseService baseService)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(_connectionString));
            _baseService = baseService;
        }
        public async Task<List<LoanInstallment>> GetLoanInstallmentsAsync(int empId)
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

                    var query = @"Select LoanId,Installment_No,DateOfInstallment,Installment_Amount,InstallmentStatus,PaymentDate,SalaryMonth,SalaryYear from LoanInstallment where EmployeeId=@EmployeeId and SubscriptionId=@subscriptionId ";

                    var result = await connection.QueryAsync<LoanInstallment>(query, new { subscriptionId = subscriptionId, EmployeeId= empId });
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
