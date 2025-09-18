using Dapper;
using HRM.Interfaces;
using HRM.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace HRM.Services
{
    public class PfContributionService : IPfContributionService
    {
        private readonly string _connectionString;
        private readonly BaseService _baseService;

        public PfContributionService(IConfiguration configuration, BaseService baseService)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(_connectionString));
            _baseService = baseService;
        }
        public async Task<bool> DeletePfContributionAsync(int id)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var queryString = "delete from PfContribution where id=@id";
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

        public async Task<List<PfContribution>> GetAllPfContributionsAsync()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var subscriptionId = _baseService.GetSubscriptionId();
                    var userId = _baseService.GetUserId();
                    var branchId = await _baseService.GetBranchId(subscriptionId, userId);

                    var query = @"SELECT t1.Id AS Id, t1.Date AS Date, t1.PFContributionPercentage AS PFContributionPercentage, t1.PFContributionYear AS PFContributionYear, t1.BranchId AS BranchId, t1.DesignationId AS DesignationId, t2.Name AS Branch, t3.DesignationName AS Designation FROM PfContribution t1 JOIN Branch t2 ON t1.BranchId = t2.Id JOIN Designation t3 ON t1.DesignationId = t3.Id WHERE t1.SubscriptionId = @subscriptionId";

                    var result = await connection.QueryAsync<PfContribution>(query, new { subscriptionId });
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> InsertPfContributionasync(PfContribution pfContribution)
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

                    var queryString = "insert into PfContribution (Date,PFContributionPercentage,PFContributionYear,BranchId,DesignationId,SubscriptionId,CompanyId) values ";
                    queryString += "( @Date,@PFContributionPercentage,@PFContributionYear,@BranchId,@DesignationId,@SubscriptionId,@CompanyId)";
                    var parameters = new DynamicParameters();
                    parameters.Add("Date", pfContribution.Date, DbType.String);
                    parameters.Add("PFContributionPercentage", pfContribution.PFContributionPercentage, DbType.Double);
                    parameters.Add("PFContributionYear", pfContribution.PFContributionYear, DbType.Double);
                    parameters.Add("BranchId", pfContribution.BranchId, DbType.Int64);
                    parameters.Add("DesignationId", pfContribution.DesignationId, DbType.Int64);
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

        public async Task<bool> UpdatePfContributionAsync(PfContribution pfContribution)
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

                    var queryString = "Update Department set Date=@Date,PFContributionPercentage=@PFContributionPercentage,PFContributionYear=@PFContributionYear,BranchId=@BranchId,DesignationId=@DesignationId,SubscriptionId=@SubscriptionId,CompanyId=@CompanyId where Id='" + pfContribution.Id + "' ";
                    var parameters = new DynamicParameters();
                    parameters.Add("Date", pfContribution.Date, DbType.String);
                    parameters.Add("PFContributionPercentage", pfContribution.PFContributionPercentage, DbType.Double);
                    parameters.Add("PFContributionYear", pfContribution.PFContributionYear, DbType.Double);
                    parameters.Add("BranchId", pfContribution.BranchId, DbType.Int64);
                    parameters.Add("DesignationId", pfContribution.DesignationId, DbType.Int64);
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
