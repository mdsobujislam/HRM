using Dapper;
using HRM.Interfaces;
using HRM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Data.SqlClient;
using System.Data;

namespace HRM.Services
{
    public class BonusCalculateService : IBonusCalculateService
    {
        private readonly string _connectionString;
        private readonly BaseService _baseService;

        public BonusCalculateService(IConfiguration configuration, BaseService baseService)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(_connectionString));
            _baseService = baseService;
        }
        public async Task<bool> DeleteBonusCalculate(int id)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var queryString = "delete from BonusCalculate where id=@id";
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

        public async Task<List<BonusCalculate>> GetAllAsync()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var subscriptionId = _baseService.GetSubscriptionId();
                    var userId = _baseService.GetUserId();
                    var branchId = await _baseService.GetBranchId(subscriptionId, userId);

                    var query = @"Select t2.BonusTypesName as BonusType,CONVERT(varchar(10), t1.BonusDate, 103) AS BonusDate,t3.EmployeeName as Employee, t4.Name as BonusType, t5.DesignationName as Designation,t6.DepartmentName as Department,t1.Percentage as Percentage,t1.BonusAmount as BonusAmount from BonusCalculate t1 JOIN BonusType t2 on t2.Id=t1.BonusTypeId JOIN Employees t3 on t3.EmpId=t1.EmployeeId JOIN Branch t4 on t4.Id=t1.BranchId JOIN Designation t5 on t5.Id=t1.DesignationId JOIN Department t6 on t6.Id=t1.DepartmentId where t1.SubscriptionId=@subscriptionId";

                    var result = await connection.QueryAsync<BonusCalculate>(query, new { subscriptionId });
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<BonusCalculate>> GetAllDataShowAsync(BonusCalculate bonusCalculate)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    var subscriptionId = _baseService.GetSubscriptionId();
                    var userId = _baseService.GetUserId();
                    var branchId = await _baseService.GetBranchId(subscriptionId, userId);

                    

                    var query = "";

                    if (bonusCalculate.BranchId != 0)
                    {
                        query = @"SELECT t2.BonusTypesName AS BonusType, CONVERT(varchar(10), t1.BonusDate, 103) AS BonusDate, t3.EmployeeName AS Employee, t4.Name AS Branch, t5.DesignationName AS Designation, t6.DepartmentName AS Department, t1.Percentage, t1.BonusAmount FROM BonusCalculate t1 JOIN BonusType t2 ON t2.Id = t1.BonusTypeId JOIN Employees t3 ON t3.EmpId = t1.EmployeeId JOIN Branch t4 ON t4.Id = t1.BranchId JOIN Designation t5 ON t5.Id = t1.DesignationId JOIN Department t6 ON t6.Id = t1.DepartmentId WHERE t1.SubscriptionId = '" + subscriptionId + "' AND t4.Id = '"+bonusCalculate.BranchId+"' AND t1.BonusDate BETWEEN '"+bonusCalculate.FromDate+ "' AND '"+bonusCalculate.ToDate+"' ";
                    }
                    if (bonusCalculate.DepartmentId != 0)
                    {
                        query = @"SELECT t2.BonusTypesName AS BonusType, CONVERT(varchar(10), t1.BonusDate, 103) AS BonusDate, t3.EmployeeName AS Employee, t4.Name AS Branch, t5.DesignationName AS Designation, t6.DepartmentName AS Department, t1.Percentage, t1.BonusAmount FROM BonusCalculate t1 JOIN BonusType t2 ON t2.Id = t1.BonusTypeId JOIN Employees t3 ON t3.EmpId = t1.EmployeeId JOIN Branch t4 ON t4.Id = t1.BranchId JOIN Designation t5 ON t5.Id = t1.DesignationId JOIN Department t6 ON t6.Id = t1.DepartmentId WHERE t1.SubscriptionId = '" + subscriptionId + "' AND t6.Id = '" + bonusCalculate.DepartmentId + "' AND t1.BonusDate BETWEEN '" + bonusCalculate.FromDate + "' AND '" + bonusCalculate.ToDate + "' ";
                    }
                    if (bonusCalculate.DesignationId != 0)
                    {
                        query = @"SELECT t2.BonusTypesName AS BonusType, CONVERT(varchar(10), t1.BonusDate, 103) AS BonusDate, t3.EmployeeName AS Employee, t4.Name AS Branch, t5.DesignationName AS Designation, t6.DepartmentName AS Department, t1.Percentage, t1.BonusAmount FROM BonusCalculate t1 JOIN BonusType t2 ON t2.Id = t1.BonusTypeId JOIN Employees t3 ON t3.EmpId = t1.EmployeeId JOIN Branch t4 ON t4.Id = t1.BranchId JOIN Designation t5 ON t5.Id = t1.DesignationId JOIN Department t6 ON t6.Id = t1.DepartmentId WHERE t1.SubscriptionId = '" + subscriptionId + "' AND t5.Id = '" + bonusCalculate.DepartmentId + "' AND t1.BonusDate BETWEEN '" + bonusCalculate.FromDate + "' AND '" + bonusCalculate.ToDate + "' ";
                    }
                    if (bonusCalculate.EmployeeId != 0)
                    {
                        query = @"SELECT t2.BonusTypesName AS BonusType, CONVERT(varchar(10), t1.BonusDate, 103) AS BonusDate, t3.EmployeeName AS Employee, t4.Name AS Branch, t5.DesignationName AS Designation, t6.DepartmentName AS Department, t1.Percentage, t1.BonusAmount FROM BonusCalculate t1 JOIN BonusType t2 ON t2.Id = t1.BonusTypeId JOIN Employees t3 ON t3.EmpId = t1.EmployeeId JOIN Branch t4 ON t4.Id = t1.BranchId JOIN Designation t5 ON t5.Id = t1.DesignationId JOIN Department t6 ON t6.Id = t1.DepartmentId WHERE t1.SubscriptionId = '" + subscriptionId + "' AND t3.Id = '" + bonusCalculate.DepartmentId + "' AND t1.BonusDate BETWEEN '" + bonusCalculate.FromDate + "' AND '" + bonusCalculate.ToDate + "' ";
                    }

                    if (string.IsNullOrEmpty(bonusCalculate.FromDate) || string.IsNullOrEmpty(bonusCalculate.ToDate))
                    {
                        return new List<BonusCalculate>();
                    }

                    var result = await connection.QueryAsync<BonusCalculate>(query);
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching bonus data", ex);
            }
        }


        public async Task<bool> InsertBonusCalculate(BonusCalculate bonusCalculate)
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

                    //var bonusDateQuery = @" SELECT TOP 1 t1.BonusDate FROM BonusCalculate t1 WHERE MONTH(t1.BonusDate) = MONTH(@BonusDate) AND YEAR(t1.BonusDate)  = YEAR(@BonusDate)";

                    //var existingBonusDate = await connection.QueryFirstOrDefaultAsync<DateTime?>( bonusDateQuery, new { bonusCalculate.BonusDate } );

                    //if (existingBonusDate.HasValue)
                    //{
                    //    // Bonus for this month and year already exists
                    //    return false;
                    //}
                    //else
                    //{
                        var resultEmpId = new List<int>();

                        if (bonusCalculate.BranchId != 0)
                        {
                            var queryBranch = @"SELECT t1.EmpId as EmployeeId FROM Employees t1 WHERE t1.BranchId = @BranchId";

                            var empIds = await connection.QueryAsync<int>(queryBranch, new { bonusCalculate.BranchId }
                            );

                            // Assign to list
                            resultEmpId = empIds.ToList();

                        }
                        else if (bonusCalculate.DesignationId != 0)
                        {
                            var queryDepartment = @"Select t1.EmpId as EmployeeId from Employees t1 where t1.DepartmentId=@DesignationId";

                            var resultDepId = await connection.QueryAsync<int>(queryDepartment, new { bonusCalculate.DesignationId });
                            resultEmpId = resultDepId.ToList();
                        }
                        else if (bonusCalculate.DepartmentId != 0)
                        {
                            var queryDesignation = @"Select t1.EmpId as EmployeeId from Employees t1 where t1.DepartmentId=@DepartmentId";

                            var resultDesi = await connection.QueryAsync<int>(queryDesignation, new { bonusCalculate.DepartmentId });
                            resultEmpId = resultDesi.ToList();
                        }
                        else if (bonusCalculate.EmployeeId != 0)
                        {
                            var queryEmployee = @"Select t1.EmpId as EmployeeId from Employees t1 where t1.EmpId=@EmployeeId";
                            var resultEmp = await connection.QueryAsync<int>(queryEmployee, new { bonusCalculate.EmployeeId });
                            resultEmpId = resultEmp.ToList();
                        }


                        var success = 0;


                        foreach (var empId in resultEmpId)
                        {
                            var empQuery = @"SELECT t1.BranchId, t1.DepartmentId, t1.DesignationId FROM Employees t1 WHERE t1.EmpId = @EmpId";

                            var empData = await connection.QueryFirstOrDefaultAsync<(int BranchId, int DepartmentId, int DesignationId)>(
                                empQuery, new { EmpId = empId });

                            if (empData.BranchId == 0) continue;

                            var bonusQuery = @"Select t1.Value from Salary t1 where t1.EmployeeId=@EmpId and Parameter='Basic Salary'";
                            double bonusAmountResult = await connection.QueryFirstOrDefaultAsync<double?>(bonusQuery, new { EmpId = empId }) ?? 0;

                            double bonusAmount = (bonusAmountResult * bonusCalculate.Percentage) / 100;
                            double totalBonus = bonusAmount + bonusAmountResult;

                            var existEmpId = @"Select t1.EmployeeId from BonusCalculate t1 where t1.BonusDate=@BonusDate and t1.EmployeeId=@EmployeeId";
                            var existingEmpId = await connection.QueryFirstOrDefaultAsync<long?>(existEmpId, new { BonusDate = bonusCalculate.BonusDate, EmployeeId = empId });

                            if (existingEmpId.HasValue)
                            {
                                // Employee already has a bonus for this date
                                continue;
                            }
                            else
                            {
                                var queryString = @"INSERT INTO BonusCalculate (BonusTypeId, BonusDate, Percentage,BonusAmount, EmployeeId, BranchId, DesignationId, DepartmentId, SubscriptionId, CompanyId, CreatedAt) VALUES (@BonusTypeId, @BonusDate, @Percentage,@BonusAmount, @EmployeeId, @BranchId, @DesignationId, @DepartmentId, @SubscriptionId, @CompanyId, @CreatedAt)";

                                var parameters = new DynamicParameters();
                                parameters.Add("BonusTypeId", bonusCalculate.BonusTypeId, DbType.Int64);
                                parameters.Add("BonusDate", bonusCalculate.BonusDate, DbType.String);
                                parameters.Add("Percentage", bonusCalculate.Percentage, DbType.Double);
                                parameters.Add("BonusAmount", totalBonus, DbType.Double);
                                parameters.Add("EmployeeId", empId, DbType.Int64);
                                parameters.Add("BranchId", empData.BranchId, DbType.Int64);
                                parameters.Add("DesignationId", empData.DesignationId, DbType.Int64);
                                parameters.Add("DepartmentId", empData.DepartmentId, DbType.Int64);
                                parameters.Add("SubscriptionId", subscriptionId);
                                parameters.Add("CompanyId", companyId);
                                parameters.Add("CreatedAt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DbType.String);

                                // 3️⃣ Execute Insert
                                success = await connection.ExecuteAsync(queryString, parameters);
                            }
                        }
                        if (success > 0)
                        {
                            return true;
                        }
                    //}



                    
                    return false;

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task<bool> UpdateBonusCalculate(BonusCalculate bonusCalculate)
        {
            throw new NotImplementedException();
        }
    }
}
