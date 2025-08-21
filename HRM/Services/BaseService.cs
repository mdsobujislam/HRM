using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using System.Security.Claims;

namespace HRM.Services;

public class BaseService
{
    private readonly string _connectionString;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public BaseService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(_connectionString));
        _httpContextAccessor = httpContextAccessor;
    }


    public int GetSubscriptionId()
    {
        var subscriptionIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("SubscriptionId")?.Value;
        return int.TryParse(subscriptionIdClaim, out var subscriptionId) ? subscriptionId : 0;
    }

    public int GetUserId()
    {
        var userId = _httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
        return int.TryParse(userId, out var EmployeeId) ? EmployeeId : 0;
    }
    
    public async Task<int> GetBranchId(int subscriptionId, int userId)
    {
        const string query = @" SELECT BranchId FROM Users WHERE Id = @UserId AND SubscriptionId = @SubscriptionId";

        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        var branchId = await connection.QueryFirstOrDefaultAsync<int?>(
            query, new { UserId = userId, SubscriptionId = subscriptionId });

        return branchId ?? 0;
    }

    public async Task<int> GetCompanyId(int subscriptionId)
    {
        const string query = @" SELECT Id FROM Companies WHERE SubscriptionId = @SubscriptionId";

        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        var companyId = await connection.QueryFirstOrDefaultAsync<int?>(
            query, new { SubscriptionId = subscriptionId });

        return companyId ?? 0;
    }

}