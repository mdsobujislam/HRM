using Dapper;
using HRM.Interfaces;
using HRM.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace HRM.Services
{
    public class SalaryService : ISalaryService
    {
        private readonly string _connectionString;
        private readonly BaseService _baseService;

        public SalaryService(IConfiguration configuration, BaseService baseService)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(_connectionString));
            _baseService = baseService;
        }
        public async Task<bool> DeleteSalary(int id)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var queryString = "delete from Salary where id=@id";
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

        //public async Task<List<Salary>> GetAllSalary()
        //{
        //    try
        //    {
        //        using (var connection = new SqlConnection(_connectionString))
        //        {
        //            connection.Open();
        //            var subscriptionId = _baseService.GetSubscriptionId();
        //            var userId = _baseService.GetUserId();
        //            var branchId = await _baseService.GetBranchId(subscriptionId, userId);

        //            var query = @"Select t1.Id,t1.MonthIndex,t1.Month,t1.Year,t1.FromDate,t1.ToDate,t1.GenDate,t1.EmployeeId,(select t2.Name from Branch t2 where t2.Id=t1.BranchId) as Branch,(Select t3.DesignationName from Designation t3 where t3.Id=t1.DesignationId) as Designation,(Select t4.EmployeeName from Employees t4 where t4.EmpId=t1.EmployeeId) as Employee,t1.Sl,t1.Parameter,t1.Value,t1.FinalAmount,t1.TxId,t1.RefTxId from Salary where t1.SubscriptionId=@subscriptionId ";

        //            var result = await connection.QueryAsync<Salary>(query, new { subscriptionId });
        //            return result.ToList();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}



        public async Task<List<Salary>> GetAllSalary()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var subscriptionId = _baseService.GetSubscriptionId();
                    var userId = _baseService.GetUserId();
                    var branchId = await _baseService.GetBranchId(subscriptionId, userId);

                    var query = @"Select t1.id,t1.SalaryHeadsId,t1.BranchId,t1.Parameter,t1.Value from Salary t1 where t1.SubscriptionId=@subscriptionId ";

                    var result = await connection.QueryAsync<Salary>(query, new { subscriptionId });
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }



        //public async Task<bool> InsertSalary(Salary salary)
        //{
        //    try
        //    {
        //        using (var connection = new SqlConnection(_connectionString))
        //        {
        //            connection.Open();

        //            var subscriptionId = _baseService.GetSubscriptionId();
        //            var userId = _baseService.GetUserId();
        //            var branchId = await _baseService.GetBranchId(subscriptionId, userId);
        //            var companyId = await _baseService.GetCompanyId(subscriptionId);

        //            var queryString = "insert into Salary (SalaryHeads,MonthIndex,Month,Year,FromDate,ToDate,GenDate,EmployeeId,DesignationId,DepartmentId,BranchId,Sl,Parameter,Value,FinalAmount,TxId,RefTxId,SubscriptionId,CompanyId,CreatedAt) values ";
        //            queryString += "( @SalaryHeads@MonthIndex,@Month,@Year,@FromDate,@ToDate,@GenDate,@EmployeeId,@DesignationId,@DepartmentId,@BranchId,@Sl,@Parameter,@Value,@FinalAmount,@TxId,@RefTxId,@SubscriptionId,@CompanyId,@CreatedAt)";
        //            var parameters = new DynamicParameters();
        //            parameters.Add("SalaryHeads", salary.SalaryHeadsId, DbType.String);
        //            parameters.Add("MonthIndex", salary.MonthIndex, DbType.String);
        //            parameters.Add("Month", salary.Month, DbType.String);
        //            parameters.Add("Year", salary.Year, DbType.String);
        //            parameters.Add("FromDate", salary.FromDate, DbType.String);
        //            parameters.Add("ToDate", salary.ToDate, DbType.String);
        //            parameters.Add("GenDate", salary.GenDate, DbType.String);
        //            parameters.Add("EmployeeId", salary.EmployeeId, DbType.String);
        //            parameters.Add("GenDate", salary.GenDate, DbType.String);
        //            parameters.Add("Sl", salary.Sl, DbType.String);
        //            parameters.Add("Parameter", salary.Parameter, DbType.String);
        //            parameters.Add("DesignationId", salary.DesignationId, DbType.Int64);
        //            parameters.Add("DepartmentId", salary.DepartmentId, DbType.Int64);
        //            parameters.Add("BranchId", salary.BranchId, DbType.Int64);
        //            parameters.Add("SubscriptionId", subscriptionId);
        //            parameters.Add("CompanyId", companyId);
        //            parameters.Add("CreatedAt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DbType.String);
        //            var success = await connection.ExecuteAsync(queryString, parameters);
        //            if (success > 0)
        //            {
        //                return true;
        //            }
        //            return false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}

        public async Task<bool> InsertSalary(Salary salary)
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

                    var queryString = "insert into Salary (SalaryHeadsId,BranchId,Sl,Parameter,Value,SubscriptionId,CompanyId) values ";
                    queryString += "( @SalaryHeadsId,@BranchId,@Sl,@Parameter,@Value,@SubscriptionId,@CompanyId)";
                    var parameters = new DynamicParameters();
                    parameters.Add("SalaryHeadsId", salary.SalaryHeadsId, DbType.Int64);
                    parameters.Add("BranchId", salary.BranchId, DbType.Int64);
                    parameters.Add("Sl", salary.Sl, DbType.Int64);
                    parameters.Add("Parameter", salary.Parameter, DbType.Int64);
                    parameters.Add("Value", salary.Value, DbType.Double);
                    parameters.Add("SubscriptionId", subscriptionId);
                    parameters.Add("CompanyId", companyId);
                    //parameters.Add("CreatedAt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DbType.String);
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

        public Task<bool> UpdateSalary(Salary salary)
        {
            throw new NotImplementedException();
        }
    }
}
