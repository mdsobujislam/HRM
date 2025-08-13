using Dapper;
using HRM.Interfaces;
using HRM.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace HRM.Services
{
    public class SalaryHeadsService : ISalaryHeadsService
    {
        private readonly string _connectionString;
        private readonly BaseService _baseService;

        public SalaryHeadsService(IConfiguration configuration, BaseService baseService)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(_connectionString));
            _baseService = baseService;
        }

        public async Task<List<SalaryHeads>> GetAllSalaryHeads()
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            var subscriptionId = _baseService.GetSubscriptionId();

            var query = @"
SELECT t1.Id, t1.Salaryitems, t1.BranchId, t2.Name AS Branch
FROM SalaryHeads t1
JOIN Branch t2 ON t1.BranchId = t2.Id
WHERE t1.SubscriptionId = @subscriptionId";

            var result = await connection.QueryAsync<SalaryHeads>(query, new { subscriptionId });
            return result.GroupBy(s => s.Id).Select(g => g.First()).ToList();

            return result.ToList();
        }

        public async Task<bool> InsertSalaryHeads(SalaryHeads salaryHeads)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            var subscriptionId = _baseService.GetSubscriptionId();
            var companyId = await _baseService.GetCompanyId(subscriptionId);

            var query = @"
INSERT INTO SalaryHeads (Salaryitems, BranchId, SubscriptionId, CompanyId, CreatedAt)
VALUES (@Salaryitems, @BranchId, @SubscriptionId, @CompanyId, @CreatedAt)";

            var parameters = new DynamicParameters();
            parameters.Add("Salaryitems", salaryHeads.Salaryitems, DbType.String);
            parameters.Add("BranchId", salaryHeads.BranchId, DbType.Int32);
            parameters.Add("SubscriptionId", subscriptionId, DbType.Int32);
            parameters.Add("CompanyId", companyId, DbType.Int32);
            parameters.Add("CreatedAt", DateTime.Now, DbType.DateTime);

            var success = await connection.ExecuteAsync(query, parameters);
            return success > 0;
        }

        public async Task<bool> UpdateSalaryHeads(SalaryHeads salaryHeads)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            var subscriptionId = _baseService.GetSubscriptionId();
            var companyId = await _baseService.GetCompanyId(subscriptionId);

            var query = @"
UPDATE SalaryHeads
SET Salaryitems = @Salaryitems,
    BranchId = @BranchId,
    SubscriptionId = @SubscriptionId,
    CompanyId = @CompanyId,
    UpdatedAt = @UpdatedAt
WHERE Id = @Id";

            var parameters = new DynamicParameters();
            parameters.Add("Id", salaryHeads.Id, DbType.Int32);
            parameters.Add("Salaryitems", salaryHeads.Salaryitems, DbType.String);
            parameters.Add("BranchId", salaryHeads.BranchId, DbType.Int32);
            parameters.Add("SubscriptionId", subscriptionId, DbType.Int32);
            parameters.Add("CompanyId", companyId, DbType.Int32);
            parameters.Add("UpdatedAt", DateTime.Now, DbType.DateTime);

            var success = await connection.ExecuteAsync(query, parameters);
            return success > 0;
        }

        public async Task<bool> DeleteSalaryHeads(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            var query = "DELETE FROM SalaryHeads WHERE Id = @Id";
            var parameters = new DynamicParameters();
            parameters.Add("Id", id, DbType.Int32);

            var success = await connection.ExecuteAsync(query, parameters);
            return success > 0;
        }
    }
}
