using Dapper;
using HRM.Interfaces;
using HRM.Models;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Globalization;

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

        public async Task<bool> InsertEmployeeLoanAsync(LoanApproval employeeLoan)
        {
            try
            {
                var subscriptionId = _baseService.GetSubscriptionId();
                var userId = _baseService.GetUserId();
                var branchId = await _baseService.GetBranchId(subscriptionId, userId);
                var companyId = await _baseService.GetCompanyId(subscriptionId);

                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    // Insert EmployeeLoan
                    string loanQuery = @"
                    INSERT INTO EmployeeLoan
                    (EmployeeId, DateOfLoan, LoanAgainst, LoanPercentage, LoanAmount, Interest, InterestAmount, Term, PerMonthInterest, EmiPrinciple, Emi, ApplicationId, BranchId, SubscriptionId, CompanyId) VALUES (@EmployeeId, @DateOfLoan, @LoanAgainst, @LoanPercentage, @LoanAmount, @Interest, @InterestAmount, @Term, @PerMonthInterest, @EmiPrinciple, @Emi, @ApplicationId, @BranchId, @SubscriptionId, @CompanyId); SELECT CAST(SCOPE_IDENTITY() AS INT);";

                    var loanParams = new DynamicParameters();
                    loanParams.Add("EmployeeId", userId);
                    loanParams.Add("DateOfLoan", employeeLoan.DateOfLoan);
                    loanParams.Add("LoanAgainst", employeeLoan.LoanAgainst);
                    loanParams.Add("LoanPercentage", employeeLoan.LoanPercentage);
                    loanParams.Add("LoanAmount", employeeLoan.LoanAmount);
                    loanParams.Add("Interest", employeeLoan.Interest);
                    loanParams.Add("InterestAmount", employeeLoan.InterestAmount);
                    loanParams.Add("Term", employeeLoan.Term);
                    loanParams.Add("PerMonthInterest", employeeLoan.PerMonthInterest);
                    loanParams.Add("EmiPrinciple", employeeLoan.EmiPrinciple);
                    loanParams.Add("Emi", employeeLoan.Emi);
                    loanParams.Add("ApplicationId", employeeLoan.ApplicationId);
                    loanParams.Add("BranchId", branchId);
                    loanParams.Add("SubscriptionId", subscriptionId);
                    loanParams.Add("CompanyId", companyId);

                    int empLoanId = await connection.ExecuteScalarAsync<int>(loanQuery, loanParams);

                    // Insert uploaded files (multiple)
                    if (employeeLoan.PdfFiles != null && employeeLoan.PdfFiles.Count > 0)
                    {
                        foreach (var file in employeeLoan.PdfFiles)
                        {
                            if (file.Length > 0)
                            {
                                byte[] fileBytes;
                                using (var ms = new MemoryStream())
                                {
                                    await file.CopyToAsync(ms);
                                    fileBytes = ms.ToArray();
                                }

                                string fileQuery = @" INSERT INTO EmployeeLoanFile (EmployeeId, ApplicationFile, ContentType, LoanId, BranchId, SubscriptionId, CompanyId) VALUES (@EmployeeId, @ApplicationFile, @ContentType, @LoanId, @BranchId, @SubscriptionId, @CompanyId);";

                                var fileParams = new DynamicParameters();
                                fileParams.Add("EmployeeId", userId);
                                fileParams.Add("ApplicationFile", fileBytes, DbType.Binary);
                                fileParams.Add("ContentType", file.ContentType);
                                fileParams.Add("LoanId", empLoanId);
                                fileParams.Add("BranchId", branchId);
                                fileParams.Add("SubscriptionId", subscriptionId);
                                fileParams.Add("CompanyId", companyId);

                                await connection.ExecuteAsync(fileQuery, fileParams);
                            }
                        }
                    }

                    // Update LoanApproval
                    string approvalQuery = @" UPDATE LoanApproval SET LoanIssued = @LoanIssued, LoanId = @LoanId WHERE Id = @AppId";

                    var approvalParams = new DynamicParameters();
                    approvalParams.Add("LoanIssued", "Issued");
                    approvalParams.Add("LoanId", empLoanId);
                    approvalParams.Add("AppId", employeeLoan.Id);

                    await connection.ExecuteAsync(approvalQuery, approvalParams);

                    // Insert LoanInstallments
                    for (int i = 1; i <= employeeLoan.Term * 12; i++)
                    {
                        string installmentQuery = @"
                        INSERT INTO LoanInstallment
                        (LoanId, EmployeeId, DateOfInstallment, Installment_No, Installment_Amount, BranchId, SubscriptionId, CompanyId) VALUES (@LoanId, @EmployeeId, @DateOfInstallment, @Installment_No, @Installment_Amount, @BranchId, @SubscriptionId, @CompanyId);";

                        var installmentParams = new DynamicParameters();
                        installmentParams.Add("LoanId", empLoanId);
                        installmentParams.Add("EmployeeId", userId);
                        DateTime loanDate = DateTime.Parse(employeeLoan.DateOfLoan);
                        installmentParams.Add("DateOfInstallment", loanDate.AddMonths(i).ToString("yyyy-MM-dd"), DbType.String);
                        installmentParams.Add("Installment_No", i);
                        installmentParams.Add("Installment_Amount", employeeLoan.Emi);
                        installmentParams.Add("BranchId", branchId);
                        installmentParams.Add("SubscriptionId", subscriptionId);
                        installmentParams.Add("CompanyId", companyId);

                        await connection.ExecuteAsync(installmentQuery, installmentParams);
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                // log exception
                return false;
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
                        return new List<LoanApproval>();

                    var query = @" SELECT t1.Id as ApplicationId, t2.EmpId AS EmployeeId, t2.EmployeeName, CONVERT(varchar(10), t1.AppliDate, 120) AS AppliDate, (SELECT t5.Name FROM Branch t5 WHERE t5.Id = t2.BranchId) AS Branch, t1.Id AS LoanId, t1.LoanApproved as LoanApproved, t1.interest, t1.Term FROM LoanApproval t1 JOIN Employees t2 ON t2.EmpId = t1.EmployeeId WHERE t1.SubscriptionId = @SubscriptionId AND t1.AppliDate >= @FromDate AND t1.AppliDate < DATEADD(DAY, 1, @ToDate)";

                    var from = DateTime.ParseExact(loanApproval.FromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    var to = DateTime.ParseExact(loanApproval.ToDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);

                    var result = await connection.QueryAsync<LoanApproval>(
                        query,
                        new { SubscriptionId = subscriptionId, FromDate = from, ToDate = to });

                    return result?.ToList() ?? new List<LoanApproval>();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching loan application data", ex);
            }
        }




        //public async Task<List<LoanApproval>> SearchData(LoanApproval loanApproval)
        //{
        //    try
        //    {
        //        using (var connection = new SqlConnection(_connectionString))
        //        {
        //            await connection.OpenAsync();

        //            var subscriptionId = _baseService.GetSubscriptionId();
        //            var userId = _baseService.GetUserId();
        //            var branchId = await _baseService.GetBranchId(subscriptionId, userId);

        //            if (string.IsNullOrEmpty(loanApproval.FromDate) || string.IsNullOrEmpty(loanApproval.ToDate))
        //            {
        //                return new List<LoanApproval>();
        //            }

        //            var query = @"SELECT t1.Id as Id, t2.EmpId AS EmployeeId, t2.EmployeeName AS EmployeeName, t1.AppliDate AS AppliDate, (SELECT t5.Name FROM Branch t5 WHERE t5.Id = t2.BranchId) AS Branch, t1.id AS LoanId,t1.LoanApproved as LoanApproved,t1.interest as interest,t1.Term as Term  FROM LoanApproval t1 JOIN Employees t2 ON t2.EmpId = t1.EmployeeId WHERE t1.SubscriptionId = @SubscriptionId AND t1.AppliDate >= @FromDate AND t1.AppliDate < DATEADD(DAY, 1, @ToDate)";

        //            var result = await connection.QueryAsync<LoanApproval>(
        //                query,
        //                new
        //                {
        //                    SubscriptionId = subscriptionId,
        //                    FromDate = Convert.ToDateTime(loanApproval.FromDate),
        //                    ToDate = Convert.ToDateTime(loanApproval.ToDate)
        //                });

        //            return result.ToList();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Error fetching loan application data", ex);
        //    }
        //}

        public Task<bool> UpdateEmployeeLoanAsync(EmployeeLoan employeeLoan)
        {
            throw new NotImplementedException();
        }
    }
}
