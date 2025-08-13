using Dapper;
using HRM.Interfaces;
using HRM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.Data.SqlClient;
using System.Data;
using ILogger = Microsoft.Build.Framework.ILogger;

namespace HRM.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly string _connectionString;
        private readonly BaseService _baseService;
        private readonly IWebHostEnvironment _env;

        public EmployeeService(IConfiguration configuration, BaseService baseService, IWebHostEnvironment env)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(_connectionString));
            _baseService = baseService;
            _env = env;
        }
        public async Task<bool> DeleteEmployee(int empId)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var queryString = "DELETE FROM Employees WHERE EmpId = @id";
                    var parameters = new DynamicParameters();
                    parameters.Add("id", empId, DbType.Int32);

                    var affectedRows = await connection.ExecuteAsync(queryString, parameters);
                    return affectedRows > 0;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        public async Task<List<Employee>> GetAllEmployee()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var subscriptionId = _baseService.GetSubscriptionId();
                    var userId = _baseService.GetUserId();
                    var branchId = await _baseService.GetBranchId(subscriptionId, userId);

                    var query = @"SELECT t1.EmpId, t1.EmployeeName, t1.FatherName, t1.MotherName, t1.SpouseName, t1.DOB, t1.Gender, t1.BloodGroup, t1.MaritalStatus, t1.TinNo, t1.NationalID, t1.Telephone, t1.MobileNo, t1.MailID, t1.PermanentAddress, t1.PresentAddress, t1.DateOfAppointment, t1.Religion, t1.Nationality, t1.Shift, t1.Version, t1.JobType, t1.UploadPhoto, t2.Name as Branch, t3.DepartmentName as Department, t4.DesignationName as Designation, t1.CompanyId, t1.SubscriptionId, t1.CreatedAt, t1.UpdatedAt,t2.Id as BranchId,t3.Id as DepartmentId,t4.Id as DesignationId FROM Employees t1 JOIN Branch t2 on t1.BranchId=t2.Id JOIN Department t3 on t1.DepartmentId=t3.Id JOIN Designation t4 on t1.DesignationId=t4.Id WHERE t1.SubscriptionId = @subscriptionId";

                    var result = await connection.QueryAsync<Employee>(query, new { subscriptionId });
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> InsertEmployee(Employee employee, IFormFile PhotoFile)
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


                    if (PhotoFile != null && PhotoFile.Length > 0)
                    {
                        // Define folder and unique file name
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "profile");
                        Directory.CreateDirectory(uploadsFolder); // Ensure folder exists

                        var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(PhotoFile.FileName);
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        // Save file to server
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await PhotoFile.CopyToAsync(stream);
                        }

                        // Save relative path to DB
                        employee.UploadPhoto = uniqueFileName;
                    }


                    var queryString = "insert into Employees (EmployeeName,FatherName,MotherName,SpouseName,DOB,Gender,BloodGroup,MaritalStatus,TinNo,NationalID,Telephone,MobileNo,MailID,PermanentAddress,PresentAddress,DateOfAppointment,Religion,Nationality,Shift,Version,JobType,UploadPhoto,BranchId,DepartmentId,DesignationId,SubscriptionId,CompanyId,CreatedAt) values ";
                    queryString += "( @EmployeeName,@FatherName,@MotherName,@SpouseName,@DOB,@Gender,@BloodGroup,@MaritalStatus,@TinNo,@NationalID,@Telephone,@MobileNo,@MailID,@PermanentAddress,@PresentAddress,@DateOfAppointment,@Religion,@Nationality,@Shift,@Version,@JobType,@UploadPhoto,@BranchId,@DepartmentId,@DesignationId,@SubscriptionId,@CompanyId,@CreatedAt)";
                    var parameters = new DynamicParameters();
                    parameters.Add("EmployeeName", employee.EmployeeName, DbType.String);
                    parameters.Add("FatherName", employee.FatherName, DbType.String);
                    parameters.Add("MotherName", employee.MotherName, DbType.String);
                    parameters.Add("SpouseName", employee.SpouseName, DbType.String);
                    parameters.Add("DOB", employee.DOB, DbType.String);
                    parameters.Add("Gender", employee.Gender, DbType.String);
                    parameters.Add("BloodGroup", employee.BloodGroup, DbType.String);
                    parameters.Add("MaritalStatus", employee.MaritalStatus, DbType.String);
                    parameters.Add("TinNo", employee.TinNo, DbType.String);
                    parameters.Add("NationalID", employee.Nationality, DbType.String);
                    parameters.Add("Telephone", employee.Telephone, DbType.String);
                    parameters.Add("MobileNo", employee.MobileNo, DbType.String);
                    parameters.Add("MailID", employee.MailID, DbType.String);
                    parameters.Add("PermanentAddress", employee.PermanentAddress, DbType.String);
                    parameters.Add("PresentAddress", employee.PresentAddress, DbType.String);
                    parameters.Add("DateOfAppointment", employee.DateOfAppointment, DbType.String);
                    parameters.Add("Religion", employee.Religion, DbType.String);
                    parameters.Add("Nationality", employee.Nationality, DbType.String);
                    parameters.Add("Shift", employee.Shift, DbType.String);
                    parameters.Add("Version", employee.Version, DbType.String);
                    parameters.Add("JobType", employee.JobType, DbType.String);
                    parameters.Add("UploadPhoto", employee.UploadPhoto);
                    parameters.Add("BranchId", employee.BranchId, DbType.Int64);
                    parameters.Add("DepartmentId", employee.DepartmentId, DbType.Int64);
                    parameters.Add("DesignationId", employee.DesignationId, DbType.Int64);
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


        public async Task<bool> UpdateEmployee(Employee employee, IFormFile PhotoFile)
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

                    if (PhotoFile != null && PhotoFile.Length > 0)
                    {
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "profile");
                        Directory.CreateDirectory(uploadsFolder);

                        var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(PhotoFile.FileName);
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await PhotoFile.CopyToAsync(stream);
                        }

                        employee.UploadPhoto = uniqueFileName;
                    }

                    var queryString = @"UPDATE Employees SET 
                                EmployeeName = @EmployeeName,
                                FatherName = @FatherName,
                                MotherName = @MotherName,
                                SpouseName = @SpouseName,
                                DOB = @DOB,
                                Gender = @Gender,
                                BloodGroup = @BloodGroup,
                                MaritalStatus = @MaritalStatus,
                                TinNo = @TinNo,
                                NationalID = @NationalID,
                                Telephone = @Telephone,
                                MobileNo = @MobileNo,
                                MailID = @MailID,
                                PermanentAddress = @PermanentAddress,
                                PresentAddress = @PresentAddress,
                                DateOfAppointment = @DateOfAppointment,
                                Religion = @Religion,
                                Nationality = @Nationality,
                                Shift = @Shift,
                                Version = @Version,
                                JobType = @JobType,
                                UploadPhoto = @UploadPhoto,
                                BranchId = @BranchId,
                                DepartmentId = @DepartmentId,
                                DesignationId = @DesignationId,
                                SubscriptionId = @SubscriptionId,
                                CompanyId = @CompanyId,
                                UpdatedAt = @UpdatedAt
                                WHERE EmpId = @EmpId";

                    var parameters = new DynamicParameters();
                    parameters.Add("EmpId", employee.EmpId);
                    parameters.Add("EmployeeName", employee.EmployeeName);
                    parameters.Add("FatherName", employee.FatherName);
                    parameters.Add("MotherName", employee.MotherName);
                    parameters.Add("SpouseName", employee.SpouseName);
                    parameters.Add("DOB", employee.DOB);
                    parameters.Add("Gender", employee.Gender);
                    parameters.Add("BloodGroup", employee.BloodGroup);
                    parameters.Add("MaritalStatus", employee.MaritalStatus);
                    parameters.Add("TinNo", employee.TinNo);
                    parameters.Add("NationalID", employee.NationalID);
                    parameters.Add("Telephone", employee.Telephone);
                    parameters.Add("MobileNo", employee.MobileNo);
                    parameters.Add("MailID", employee.MailID);
                    parameters.Add("PermanentAddress", employee.PermanentAddress);
                    parameters.Add("PresentAddress", employee.PresentAddress);
                    parameters.Add("DateOfAppointment", employee.DateOfAppointment);
                    parameters.Add("Religion", employee.Religion);
                    parameters.Add("Nationality", employee.Nationality);
                    parameters.Add("Shift", employee.Shift);
                    parameters.Add("Version", employee.Version);
                    parameters.Add("JobType", employee.JobType);
                    parameters.Add("UploadPhoto", employee.UploadPhoto ?? string.Empty);
                    parameters.Add("BranchId", employee.BranchId);
                    parameters.Add("DepartmentId", employee.DepartmentId);
                    parameters.Add("DesignationId", employee.DesignationId);
                    parameters.Add("SubscriptionId", subscriptionId);
                    parameters.Add("CompanyId", companyId);
                    parameters.Add("UpdatedAt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DbType.String);

                    var success = await connection.ExecuteAsync(queryString, parameters);
                    return success > 0;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }



    }
}
