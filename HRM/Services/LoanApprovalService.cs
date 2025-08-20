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
        public Task<bool> DeleteLoanApproval(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<LoanApproval>> GetAllLoanApproval()
        {
            throw new NotImplementedException();
        }

        public async Task<bool> InsertLoanApproval(LoanApproval loanApproval)
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

                    var queryString = "insert into Branch (Name,Address,SubscriptionId,CompanyId,CreatedAt) values ";
                    queryString += "( @Name,@Address,@SubscriptionId,@CompanyId,@CreatedAt)";
                    var parameters = new DynamicParameters();
                    //parameters.Add("Name", branch.Name, DbType.String);
                    //parameters.Add("Address", branch.Address, DbType.String);
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

        public Task<bool> UpdateLoanApproval(LoanApproval loanApproval)
        {
            throw new NotImplementedException();
        }
    }
}
