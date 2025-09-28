using Dapper;
using HRM.Interfaces;
using HRM.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace HRM.Services
{
    public class EmployeeSeparationService : IEmployeeSeparationService
    {
        private readonly string _connectionString;
        private readonly BaseService _baseService;
        public EmployeeSeparationService(IConfiguration configuration, BaseService baseService)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(_connectionString));
            _baseService = baseService;
        }
        public async Task<bool> InsertEmployeeSeparationAsync(EmployeeSeparation employeeSeparation)
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


                    var empBranchquery = "Select BranchId from Employees where EmpId='" + employeeSeparation.EmployeeId + "'";
                    int empBranchId = await connection.ExecuteScalarAsync<int>(empBranchquery);


                    var queryString = "insert into EmployeeSeparation (EmployeeId,SeparationReasonsId,Remarks,Sep_Date,Req_date,BranchId,SubscriptionId,CompanyId) values ";
                    queryString += "( @EmployeeId,@SeparationReasonsId,@Remarks,@Sep_Date,@Req_date,@BranchId,@SubscriptionId,@CompanyId); SELECT CAST(SCOPE_IDENTITY() AS INT);";
                    var parameters = new DynamicParameters();
                    parameters.Add("EmployeeId", employeeSeparation.EmployeeId, DbType.String);
                    parameters.Add("SeparationReasonsId", employeeSeparation.SeparationReasonsId, DbType.String);
                    parameters.Add("Remarks", employeeSeparation.Remarks, DbType.String);
                    parameters.Add("Sep_Date", employeeSeparation.SeparationDate, DbType.String);
                    parameters.Add("Req_date", employeeSeparation.RequestDate, DbType.String);
                    parameters.Add("BranchId", empBranchId, DbType.Int64);
                    parameters.Add("SubscriptionId", subscriptionId);
                    parameters.Add("CompanyId", companyId);
                    parameters.Add("CreatedAt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DbType.String);
                    int employeeSeparationId = await connection.ExecuteScalarAsync<int>(queryString, parameters);

                    if (employeeSeparation.PdfFiles != null && employeeSeparation.PdfFiles.Count > 0)
                    {
                        string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "employeeSeparationFile");

                        // Ensure folder exists
                        if (!Directory.Exists(uploadPath))
                        {
                            Directory.CreateDirectory(uploadPath);
                        }

                        foreach (var file in employeeSeparation.PdfFiles)
                        {
                            if (file != null && file.Length > 0)
                            {
                                // Create unique file name
                                string fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
                                string filePath = Path.Combine(uploadPath, fileName);

                                // Save file to folder
                                using (var stream = new FileStream(filePath, FileMode.Create))
                                {
                                    await file.CopyToAsync(stream);
                                }

                                // Insert file path into database
                                string fileQuery = @" INSERT INTO EmployeeSeperationFile (EmployeeId, application_file, content_type,Document_Type, EmployeeSeparationId, BranchId, SubscriptionId, CompanyId) VALUES (@EmployeeId, @application_file, @content_type,@Document_Type, @EmployeeSeparationId, @BranchId, @SubscriptionId, @CompanyId)";

                                var fileParams = new DynamicParameters();
                                fileParams.Add("EmployeeId", employeeSeparation.EmployeeId);
                                fileParams.Add("application_file", "/uploads/employeeSeparationFile/" + fileName);
                                fileParams.Add("content_type", file.ContentType);
                                fileParams.Add("Document_Type", file.ContentType);
                                fileParams.Add("EmployeeSeparationId", employeeSeparationId);
                                fileParams.Add("BranchId", empBranchId);
                                fileParams.Add("SubscriptionId", subscriptionId);
                                fileParams.Add("CompanyId", companyId);

                                await connection.ExecuteAsync(fileQuery, fileParams);
                            }
                        }
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
