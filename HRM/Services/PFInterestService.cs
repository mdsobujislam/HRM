using Dapper;
using HRM.Interfaces;
using HRM.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace HRM.Services
{
    public class PFInterestService : IPFInterestService
    {
        private readonly string _connectionString;
        private readonly BaseService _baseService;
        public PFInterestService(IConfiguration configuration, BaseService baseService)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(_connectionString));
            _baseService = baseService;
        }
        public async Task<bool> DeletePFInterestAsync(int id)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var queryString = "delete from PFInterest where id=@id";
                    var parameters = new DynamicParameters();
                    parameters.Add("id", id.ToString(), DbType.String);
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

        public async Task<List<PFInterest>> GetAllPFInterestsAsync()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var subscriptionId = _baseService.GetSubscriptionId();
                    var userId = _baseService.GetUserId();
                    var branchId = await _baseService.GetBranchId(subscriptionId, userId);

                    var query = @"SELECT t1.Id AS Id, t1.Year as Year,t1.Interest as Interest,t2.Name as Branch,t2.Id as BranchId FROM PFInterest t1 LEFT JOIN Branch t2 ON t1.BranchId = t2.Id WHERE t1.SubscriptionId = @subscriptionId";

                    var result = await connection.QueryAsync<PFInterest>(query, new { subscriptionId });
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> InsertPFInterestAsync(PFInterest pFInterest)
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

                    var queryString = "insert into PFInterest (Year,Interest,BranchId,SubscriptionId,CompanyId) values ";
                    queryString += "( @Year,@Interest,@BranchId,@SubscriptionId,@CompanyId)";
                    var parameters = new DynamicParameters();
                    parameters.Add("Year", pFInterest.Year, DbType.String);
                    parameters.Add("Interest", pFInterest.Interest, DbType.Double);
                    parameters.Add("BranchId", pFInterest.BranchId, DbType.Int64);
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

        public async Task<bool> UpdatePFInterestAsync(PFInterest pFInterest)
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

                    var queryString = "Update PFInterest set Year=@Year,Interest=@Interest,BranchId=@BranchId,SubscriptionId=@SubscriptionId,CompanyId=@CompanyId where Id='" + pFInterest.Id + "' ";
                    var parameters = new DynamicParameters();
                    parameters.Add("Year", pFInterest.Year, DbType.String);
                    parameters.Add("Interest", pFInterest.Interest, DbType.Double);
                    parameters.Add("BranchId", pFInterest.BranchId, DbType.Int64);
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
    }
}
