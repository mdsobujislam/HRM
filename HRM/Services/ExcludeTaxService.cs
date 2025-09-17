using Dapper;
using HRM.Interfaces;
using HRM.Models;
using HRM.Models.ViewModels;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.Data.SqlClient;
using System.ComponentModel.Design;
using System.Data;

namespace HRM.Services
{
    public class ExcludeTaxService : IExcludeTaxService
    {
        private readonly string _connectionString;
        private readonly BaseService _baseService;

        public ExcludeTaxService(IConfiguration configuration, BaseService baseService)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(_connectionString));
            _baseService = baseService;
        }
        

        public async Task<List<EmployeeTaxStatusViewModel>> GetAllExcludeTaxAsync(int branchId, int monthIndex, int year)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var subscriptionId = _baseService.GetSubscriptionId();

                // Employees (branch filter optional)
                var employees = (await connection.QueryAsync<Employee>(
                    @"SELECT t1.EmpId, t1.EmployeeName, t2.Name AS Branch
              FROM Employees t1
              JOIN Branch t2 ON t1.BranchId = t2.Id
              WHERE t1.SubscriptionId = @subscriptionId AND (@branchId = 0 OR t1.BranchId = @branchId)",
                    new { subscriptionId, branchId })).ToList();

                // ExcludeTax এ যাদের আছে (branch filter optional)
                var excluded = (await connection.QueryAsync<int>(
                    @"SELECT EmployeeId FROM ExcludeTax
              WHERE MonthIndex = @monthIndex AND Year = @year AND (@branchId = 0 OR BranchId = @branchId)",
                    new { monthIndex, year, branchId })).ToHashSet();

                // Map to viewmodel
                var model = employees.Select(e => new EmployeeTaxStatusViewModel
                {
                    EmpId = e.EmpId,
                    EmployeeName = e.EmployeeName,
                    Branch = e.Branch,
                    IsStopped = excluded.Contains(e.EmpId)
                }).ToList();

                return model;
            }
        }

        public async Task<bool> StartIncludeTaxAsync(StopExcludeTaxViewModel model)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var tran = connection.BeginTransaction())
                {
                    try
                    {
                        foreach (var empId in model.StartEmployeeIds)
                        {
                            var BranchId = await connection.ExecuteScalarAsync<int>( @"SELECT BranchId FROM Employees WHERE EmpId=@EmployeeId", new { EmployeeId = empId }, tran);

                            await connection.ExecuteAsync( @"DELETE FROM ExcludeTax WHERE MonthIndex=@MonthIndex AND Year=@Year AND BranchId=@BranchId AND EmployeeId=@EmployeeId",
                                new
                                {
                                    model.MonthIndex,
                                    model.Year,
                                    BranchId,
                                    EmployeeId = empId
                                }, tran);
                        }

                        tran.Commit();
                        return true;
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }
        }


        public async Task<bool> StopExcludeTaxAsync(StopExcludeTaxViewModel model)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var tran = connection.BeginTransaction())
                {
                    try
                    {
                        var subscriptionId = _baseService.GetSubscriptionId();
                        var companyId = await _baseService.GetCompanyId(subscriptionId);


                        // calculate first day and last day of the given month
                        var firstDay = new DateTime(model.Year, model.MonthIndex, 1);
                        var lastDay = firstDay.AddMonths(1).AddDays(-1);

                        foreach (var empId in model.StopEmployeeIds)
                        {
                            // find branch id of employee
                            var BranchId = await connection.ExecuteScalarAsync<int>(
                                @"SELECT BranchId FROM Employees WHERE EmpId=@EmployeeId",
                                new { EmployeeId = empId }, tran);

                            // check exists
                            var exists = await connection.ExecuteScalarAsync<int>(
                                @"SELECT COUNT(1) FROM ExcludeTax WHERE MonthIndex=@MonthIndex AND Year=@Year AND BranchId=@BranchId AND EmployeeId=@EmployeeId",
                                new
                                {
                                    model.MonthIndex,
                                    model.Year,
                                    BranchId,
                                    EmployeeId = empId
                                }, tran);

                            if (exists == 0)
                            {
                                await connection.ExecuteAsync(
                                    @"INSERT INTO ExcludeTax (MonthIndex, [Month], Year, FromDate, ToDate, GenDate, EmployeeId, BranchId,CompanyId,SubscriptionId) VALUES (@MonthIndex, @Month, @Year, @FromDate, @ToDate, @GenDate, @EmployeeId, @BranchId,@CompanyId,@SubscriptionId)",
                                    new
                                    {
                                        model.MonthIndex,
                                        Month = firstDay.ToString("MMMM"),
                                        model.Year,

                                        // ✅ use DateTime not string
                                        FromDate = model.FromDate ?? firstDay,
                                        ToDate = model.ToDate ?? lastDay,
                                        GenDate = model.GenDate ?? lastDay,

                                        EmployeeId = empId,
                                        BranchId,
                                        CompanyId= companyId,
                                        SubscriptionId= subscriptionId

                                    }, tran);
                            }
                        }

                        tran.Commit();
                        return true;
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }
        }



        #region
        //public async Task<List<Employee>> GetAllExcludeTaxAsync(int brnachId)
        //{
        //    try
        //    {
        //        using (var connection = new SqlConnection(_connectionString))
        //        {
        //            connection.Open();
        //            var subscriptionId = _baseService.GetSubscriptionId();
        //            var userId = _baseService.GetUserId();
        //            var branchId = await _baseService.GetBranchId(subscriptionId, userId);

        //            var query = @"Select t1.EmpId as EmpId,t1.EmployeeName as EmployeeName,t2.Name as Branch from Employees t1 JOIN Branch t2 on t1.BranchId=t2.Id WHERE t1.SubscriptionId = @subscriptionId";

        //            var result = await connection.QueryAsync<Employee>(query, new { subscriptionId });
        //            return result.ToList();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}



        //public async Task<bool> InsertExcludeTaxAsync(ExcludeTax excludeTax)
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

        //            var queryString = "insert into ExcludeTax (MonthIndex,Month,Year,FromDate,ToDate,GenDate,EmployeeId,BranchId,SubscriptionId,CompanyId) values ";
        //            queryString += "( @MonthIndex,@Month,@Year,@FromDate,@ToDate,@GenDate,@BranchId,@EmployeeId,@SubscriptionId,@CompanyId)";
        //            var parameters = new DynamicParameters();
        //            parameters.Add("MonthIndex", excludeTax.MonthIndex, DbType.String);
        //            parameters.Add("Month", excludeTax.Month, DbType.String);
        //            parameters.Add("Year", excludeTax.Year, DbType.String);
        //            parameters.Add("FromDate", excludeTax.FromDate, DbType.String);
        //            parameters.Add("ToDate", excludeTax.ToDate, DbType.String);
        //            parameters.Add("GenDate", excludeTax.GenDate, DbType.String);
        //            parameters.Add("BranchId", excludeTax.BranchId, DbType.String);
        //            parameters.Add("EmployeeId", excludeTax.EmployeeId, DbType.String);
        //            parameters.Add("SubscriptionId", subscriptionId);
        //            parameters.Add("CompanyId", companyId);
        //            //parameters.Add("CreatedAt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DbType.String);
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



        //public async Task<bool> ExistsExcludeTaxAsync(string employeeId, string monthIndex, string year, int subscriptionId)
        //{
        //    using (var connection = new SqlConnection(_connectionString))
        //    {
        //        var sql = @"SELECT COUNT(1) FROM ExcludeTax WHERE EmployeeId = @EmployeeId AND MonthIndex = @MonthIndex AND Year = @Year AND SubscriptionId = @SubscriptionId";
        //        var count = await connection.ExecuteScalarAsync<int>(sql, new
        //        {
        //            EmployeeId = employeeId,
        //            MonthIndex = monthIndex,
        //            Year = year,
        //            SubscriptionId = subscriptionId
        //        });
        //        return count > 0;
        //    }
        //}
        #endregion
    }
}
