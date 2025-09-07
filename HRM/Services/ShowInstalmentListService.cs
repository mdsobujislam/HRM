using Dapper;
using HRM.Interfaces;
using HRM.Models;
using Microsoft.Data.SqlClient;

namespace HRM.Services
{
    public class ShowInstalmentListService : IShowInstalmentListService
    {
        private readonly string _connectionString;
        private readonly BaseService _baseService;

        public ShowInstalmentListService(IConfiguration configuration, BaseService baseService)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(_connectionString));
            _baseService = baseService;
        }
        public async Task<List<EmployeeLoanDto>> GetAllShowInstalmentListAsync()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var subscriptionId = _baseService.GetSubscriptionId();
                    var userId = _baseService.GetUserId();
                    var branchId = await _baseService.GetBranchId(subscriptionId, userId);
                    var loginid = _baseService.GetUserId();

                    var query = @" SELECT t1.Id AS LoanId, t1.ApplicationId, t1.DateOfLoan, t1.LoanAmount, t1.Interest, t1.InterestAmount, t1.Term, t1.PerMonthInterest, t1.EmiPrinciple, t1.Emi FROM EmployeeLoan t1 WHERE t1.EmployeeId = '"+ loginid + "' and t1.SubscriptionId='"+ subscriptionId + "'; SELECT LoanId, Installment_No AS InstallmentNo, DateOfInstallment, Installment_Amount AS InstallmentAmount, InstallmentStatus AS Status, PaymentDate FROM LoanInstallment WHERE LoanId IN (SELECT Id FROM EmployeeLoan WHERE EmployeeId = '" + loginid + "' and SubscriptionId='"+ subscriptionId + "'); SELECT LoanId, ApplicationFile FROM EmployeeLoanFile WHERE LoanId IN (SELECT Id FROM EmployeeLoan WHERE EmployeeId = '" + loginid + "' and SubscriptionId='"+ subscriptionId + "'); ";

                    using (var multi = await connection.QueryMultipleAsync(query, new { subscriptionId }))
                    {
                        var loans = (await multi.ReadAsync<EmployeeLoanDto>()).ToList();
                        var installments = (await multi.ReadAsync<LoanInstallmentDto>()).ToList();
                        var documents = (await multi.ReadAsync<dynamic>()).ToList();

                        // Map Installments
                        foreach (var loan in loans)
                        {
                            loan.Installments = installments
                                .Where(x => x.LoanId == loan.LoanId)
                                .ToList();

                            loan.Documents = documents
                                .Where(x => x.LoanId == loan.LoanId)
                                .Select(x => (string)x.ApplicationFile)
                                .ToList();
                        }

                        return loans;
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle/log exception properly
                throw;
            }
        }

    }
}
