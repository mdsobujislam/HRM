using Dapper;
using HRM.Interfaces;
using HRM.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace HRM.Services
{
    public class TaxtSlabSetupService : ITaxtSlabSetupService
    {
        private readonly string _connectionString;
        private readonly BaseService _baseService;

        public TaxtSlabSetupService(IConfiguration configuration, BaseService baseService)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(_connectionString));
            _baseService = baseService;
        }
        public async Task<bool> DeleteTaxtSlabSetupAsync(int id)
        {
            try
            {
                using (SqlConnection connection=new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var queryString = "delete from TaxSlab where id=@id";
                    var parameters = new DynamicParameters();
                    parameters.Add("id", id.ToString(), DbType.String);
                    var success=await connection.ExecuteAsync(queryString, parameters);
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

        public async Task<List<TaxtSlabSetup>> GetAllTaxtSlabSetupAsync()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var subscriptionId = _baseService.GetSubscriptionId();
                    var userId = _baseService.GetUserId();
                    var branchId = await _baseService.GetBranchId(subscriptionId, userId);

                    var query = @"select t1.Id as Id, CONVERT(varchar(10), t1.Date, 23) as Date, t1.IncomeSlab, t1.TaxPer, t1.BranchId, t1.DepartmentId, t1.AdditionalId, t2.AdditionalInfoName, t3.Name as BranchName, t4.DepartmentName from TaxSlab t1 JOIN AdditionalInfo t2 on t1.AdditionalId = t2.Id JOIN Branch t3 on t1.BranchId = t3.Id JOIN Department t4 on t1.DepartmentId = t4.Id WHERE t1.SubscriptionId = @subscriptionId";

                    var result = await connection.QueryAsync<TaxtSlabSetup>(query, new { subscriptionId });
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> InsertTaxtSlabSetupAsync(TaxtSlabSetup taxtSlabSetup)
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

                    var queryString = "insert into TaxSlab (Date,IncomeSlab,TaxPer,AdditionalId,BranchId,DepartmentId,SubscriptionId,CompanyId,CreatedAt) values ";
                    queryString += "( @Date,@IncomeSlab,@TaxPer,@AdditionalId,@BranchId,@DepartmentId,@SubscriptionId,@CompanyId,@CreatedAt)";
                    var parameters = new DynamicParameters();
                    parameters.Add("Date", taxtSlabSetup.Date, DbType.String);
                    parameters.Add("IncomeSlab", taxtSlabSetup.IncomeSlab, DbType.Double);
                    parameters.Add("TaxPer", taxtSlabSetup.TaxPer, DbType.Double);
                    parameters.Add("AdditionalId", taxtSlabSetup.AdditionalId, DbType.Int64);
                    parameters.Add("BranchId", taxtSlabSetup.BranchId, DbType.Int64);
                    parameters.Add("DepartmentId", taxtSlabSetup.DepartmentId, DbType.Int64);
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

        public async Task<bool> UpdateTaxtSlabSetupAsync(TaxtSlabSetup taxtSlabSetup)
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

                    var queryString = "Update TaxSlab set Date=@Date,IncomeSlab=@IncomeSlab,TaxPer=@TaxPer,AdditionalId=@AdditionalId,BranchId=@BranchId,DepartmentId=@DepartmentId,SubscriptionId=@SubscriptionId,CompanyId=@CompanyId,UpdatedAt=@UpdatedAt where Id='" + taxtSlabSetup.Id + "' ";
                    var parameters = new DynamicParameters();
                    parameters.Add("Date", taxtSlabSetup.Date, DbType.String);
                    parameters.Add("IncomeSlab", taxtSlabSetup.IncomeSlab, DbType.Double);
                    parameters.Add("TaxPer", taxtSlabSetup.TaxPer, DbType.Double);
                    parameters.Add("AdditionalId", taxtSlabSetup.AdditionalId, DbType.Int64);
                    parameters.Add("BranchId", taxtSlabSetup.BranchId, DbType.Int64);
                    parameters.Add("DepartmentId", taxtSlabSetup.DepartmentId, DbType.Int64);
                    parameters.Add("SubscriptionId", subscriptionId);
                    parameters.Add("CompanyId", companyId);
                    parameters.Add("UpdatedAt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DbType.String);
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
