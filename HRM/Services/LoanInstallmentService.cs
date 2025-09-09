using Dapper;
using HRM.Interfaces;
using HRM.Models;
using Microsoft.Data.SqlClient;
using System.Data;

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

                    var query = @" SELECT l.Id, l.LoanId, l.EmployeeId, CONVERT(nvarchar(50), l.DateOfInstallment, 23) AS DateOfInstallment, l.Installment_No, l.Installment_Amount, l.InstallmentStatus, CONVERT(nvarchar(50), l.PaymentDate, 23) AS PaymentDate, l.SalaryMonth, l.SalaryYear, CASE WHEN l.RefId IS NOT NULL AND l.RefId <> 0 THEN ( SELECT CAST(MIN(li.Installment_No) AS NVARCHAR(10)) + CASE WHEN MIN(li.Installment_No) <> MAX(li.Installment_No) THEN '-' + CAST(MAX(li.Installment_No) AS NVARCHAR(10)) ELSE '' END FROM LoanInstallment li WHERE li.RefId = l.RefId AND li.LoanId = l.LoanId AND li.EmployeeId = l.EmployeeId ) ELSE CAST(l.Installment_No AS NVARCHAR(10)) END AS InstallmentDetails FROM LoanInstallment l WHERE l.EmployeeId = @EmployeeId AND l.SubscriptionId = @subscriptionId ORDER BY l.Installment_No;";

                    var result = await connection.QueryAsync<LoanInstallment>(
                        query, new { subscriptionId = subscriptionId, EmployeeId = empId });

                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }




        //public async Task<List<LoanInstallment>> GetLoanInstallmentsAsync(int empId)
        //{
        //    try
        //    {
        //        using (var connection = new SqlConnection(_connectionString))
        //        {
        //            connection.Open();
        //            var subscriptionId = _baseService.GetSubscriptionId();
        //            var userId = _baseService.GetUserId();
        //            var branchId = await _baseService.GetBranchId(subscriptionId, userId);
        //            var loginid = _baseService.GetUserId();


        //            var query = @"Select LoanId,EmployeeId,Installment_No,DateOfInstallment,Installment_Amount,InstallmentStatus,PaymentDate,SalaryMonth,SalaryYear from LoanInstallment where EmployeeId=@EmployeeId and SubscriptionId=@subscriptionId ";

        //            var result = await connection.QueryAsync<LoanInstallment>(query, new { subscriptionId = subscriptionId, EmployeeId= empId });
        //            return result.ToList();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}

        public async Task<bool> UpdateLoanInstallmentAsync(PayInstallment payInstallment)
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

                    // Find employee branch
                    var branchQuery = @"SELECT BranchId FROM Employees WHERE EmpId=@Id AND SubscriptionId=@subscriptionId";
                    var branchQueryId = await connection.QueryFirstOrDefaultAsync<int>(
                        branchQuery, new { Id = payInstallment.EmployeeId, subscriptionId });

                    var idQuery = @"SELECT Id FROM LoanInstallment WHERE LoanId = @LoanId AND EmployeeId = @EmployeeId AND Installment_No = @InstallmentNo AND SubscriptionId = @SubscriptionId";

                    var installmentId = await connection.QueryFirstOrDefaultAsync<int>(
                        idQuery,
                        new
                        {
                            LoanId = payInstallment.LoanId,
                            EmployeeId = payInstallment.EmployeeId,
                            InstallmentNo = payInstallment.installmentFrom,
                            SubscriptionId = subscriptionId
                        });

                    // Loop from installmentFrom to installmentTo
                    for (int instNo = payInstallment.installmentFrom; instNo <= payInstallment.installmentTo; instNo++)
                    {
                        var queryString = @" UPDATE LoanInstallment SET PaymentDate = @PaymentDate, InstallmentStatus = @InstallmentStatus, RefId = @RefId, BranchId = @BranchId, SubscriptionId = @SubscriptionId, CompanyId = @CompanyId WHERE LoanId = @LoanId AND EmployeeId = @EmployeeId AND Installment_No = @InstallmentNo AND SubscriptionId = @SubscriptionId";

                        var parameters = new DynamicParameters();
                        parameters.Add("PaymentDate", payInstallment.dateOfPayment, DbType.String);
                        parameters.Add("InstallmentStatus", "Paid", DbType.String);
                        parameters.Add("RefId", installmentId);
                        parameters.Add("BranchId", branchQueryId);
                        parameters.Add("SubscriptionId", subscriptionId);
                        parameters.Add("CompanyId", companyId);
                        parameters.Add("LoanId", payInstallment.LoanId);
                        parameters.Add("EmployeeId", payInstallment.EmployeeId);
                        parameters.Add("InstallmentNo", instNo);

                        await connection.ExecuteAsync(queryString, parameters);
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
