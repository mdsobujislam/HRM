using Dapper;
using HRM.Interfaces;
using HRM.Models;
using Microsoft.Data.SqlClient;

namespace HRM.Services
{
    public class BonusTypeService : IBonusTypeService
    {
        private readonly string _connectionString;
        private readonly BaseService _baseService;

        public BonusTypeService(IConfiguration configuration, BaseService baseService)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(_connectionString));
            _baseService = baseService;
        }
        public Task<bool> InsertBonusType(BonusType bonusType)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteBonusTypeA(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<BonusType>> GetAllBonusType()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var subscriptionId = _baseService.GetSubscriptionId();
                    var userId = _baseService.GetUserId();
                    var branchId = await _baseService.GetBranchId(subscriptionId, userId);

                    var query = @"Select Id,BonusTypesName from BonusType WHERE SubscriptionId = @subscriptionId";

                    var result = await connection.QueryAsync<BonusType>(query, new { subscriptionId });
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task<bool> UpdateBonusType(BonusType bonusType)
        {
            throw new NotImplementedException();
        }
    }
}
