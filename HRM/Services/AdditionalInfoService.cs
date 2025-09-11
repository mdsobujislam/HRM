using Dapper;
using HRM.Interfaces;
using HRM.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace HRM.Services
{
    public class AdditionalInfoService : IAdditionalInfoService
    {
        private readonly string _connectionString;
        private readonly BaseService _baseService;

        public AdditionalInfoService(IConfiguration configuration, BaseService baseService)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(_connectionString));
            _baseService = baseService;
        }

        public async Task<IEnumerable<AdditionalInfo>> GetAllAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            var sql = "SELECT Id, AdditionalInfoName FROM AdditionalInfo ORDER BY AdditionalInfoName";
            var list = await connection.QueryAsync<AdditionalInfo>(sql);
            return list;
        }

        public async Task<int?> GetByNameAsync(string name)
        {
            using var connection = new SqlConnection(_connectionString);
            var sql = "SELECT Id FROM AdditionalInfo WHERE AdditionalInfoName = @name";
            var id = await connection.QueryFirstOrDefaultAsync<int?>(sql, new { name });
            return id;
        }

        public async Task<AdditionalInfo?> InsertAdditionalInfo(AdditionalInfo additionalInfo)
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

                    var queryString = @"INSERT INTO AdditionalInfo 
                                (AdditionalInfoName, BranchId, SubscriptionId, CompanyId, CreatedAt) 
                                OUTPUT INSERTED.Id 
                                VALUES (@AdditionalInfoName, @BranchId, @SubscriptionId, @CompanyId, @CreatedAt)";

                    var parameters = new DynamicParameters();
                    parameters.Add("AdditionalInfoName", additionalInfo.AdditionalInfoName, DbType.String);
                    parameters.Add("BranchId", branchId);
                    parameters.Add("SubscriptionId", subscriptionId);
                    parameters.Add("CompanyId", companyId);
                    parameters.Add("CreatedAt", DateTime.Now, DbType.DateTime);

                    var newId = await connection.ExecuteScalarAsync<int>(queryString, parameters);

                    additionalInfo.Id = newId; // set generated Id
                    return additionalInfo;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

    }
}
