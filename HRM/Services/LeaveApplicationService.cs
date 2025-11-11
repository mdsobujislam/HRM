using Dapper;
using HRM.Interfaces;
using HRM.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace HRM.Services
{
    public class LeaveApplicationService : ILeaveApplicationService
    {
        private readonly string _connectionString;
        private readonly BaseService _baseService;

        public LeaveApplicationService(IConfiguration configuration, BaseService baseService)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(_connectionString));
            _baseService = baseService;
        }
        public async Task<List<LeaveApplication>> GetAllLeaveApplicationByAsync()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var subscriptionId = _baseService.GetSubscriptionId();
                    var userId = _baseService.GetUserId();

                    var query = @"SELECT t2.TypeName AS LeaveType, CONVERT(varchar(10), t1.StartDate, 120) AS StartDate, CONVERT(varchar(10), t1.EndDate, 120) AS EndDate, t1.NumberOfDays AS NumberOfDays, t1.Reason AS Reason, CASE WHEN t1.ImmediateBossByUpdatedAt IS NULL THEN 'Not Approved' ELSE 'Approved' END AS ApprovedByImmediateBossStatus, CASE WHEN t1.HRByUpdatedAt IS NULL THEN 'Not Approved' ELSE 'Approved' END AS ApprovedByHRStatus, STRING_AGG(t3.FilePath, ',') AS FilePaths FROM LeaveRecords t1 JOIN LeaveType t2 ON t1.LeaveTypeId = t2.Id LEFT JOIN LeaveAttachments t3 ON t3.LeaveRecordId = t1.Id WHERE t1.EmployeeId = @userId AND t1.SubscriptionId = @subscriptionId GROUP BY t2.TypeName, t1.StartDate, t1.EndDate, t1.NumberOfDays, t1.Reason, t1.ImmediateBossByUpdatedAt, t1.HRByUpdatedAt";

                    var result = await connection.QueryAsync<LeaveApplication>(query, new { userId, subscriptionId });
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching leave applications", ex);
            }
        }






        public async Task<bool> InsertLeaveApplication(LeaveApplication leaveApplication, List<IFormFile> Documents)
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

                    var leaveHold = @"Select COUNT(1) from LeaveRecords where EmployeeId = '" + userId + "' AND HRByUpdatedAt is null AND  SubscriptionId = '" + subscriptionId + "'";
                    var leaveHoldCount = await connection.ExecuteScalarAsync<int>(leaveHold);
                    if (leaveHoldCount > 0)
                    {
                        return false;
                    }

                    var existingQuery = @"Select COUNT(1) from LeaveRecords where EmployeeId = @EmployeeId AND ((StartDate <= @EndDate AND EndDate >= @StartDate)) AND SubscriptionId = @SubscriptionId";
                    var existingCount = await connection.ExecuteScalarAsync<int>(existingQuery, new
                    {
                        EmployeeId = userId,
                        LeaveTypeId = leaveApplication.LeaveTypeId,
                        StartDate = leaveApplication.StartDate,
                        EndDate = leaveApplication.EndDate,
                        SubscriptionId = subscriptionId
                    });
                    if (existingCount > 0)
                    {
                        return false;
                    }


                    if (Documents != null && Documents.Count > 0)
                    {
                        var attachments = new List<LeaveAttachment>();

                        foreach (var file in Documents)
                        {
                            var originalFileName = Path.GetFileName(file.FileName);
                            var extension = Path.GetExtension(originalFileName);
                            var uniqueFileName = Path.GetFileNameWithoutExtension(originalFileName)
                                                 + "_" + Guid.NewGuid().ToString("N")
                                                 + extension;

                            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "leave-docs");
                            if (!Directory.Exists(uploadsFolder))
                                Directory.CreateDirectory(uploadsFolder);

                            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }

                            attachments.Add(new LeaveAttachment
                            {
                                FileName = uniqueFileName,
                                FilePath = "/uploads/leave-docs/" + uniqueFileName,
                                CreatedAt = DateTime.Now
                            });
                        }


                        leaveApplication.Attachments = attachments;
                    }


                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // Insert into LeaveRecords
                            var queryString = @"INSERT INTO LeaveRecords 
                (EmployeeId, LeaveTypeId, StartDate, EndDate, NumberOfDays, Reason, CreatedBy, CreatedAt, BranchId, SubscriptionId, CompanyId)
                VALUES (@EmployeeId, @LeaveTypeId, @StartDate, @EndDate, @NumberOfDays, @Reason, @CreatedBy, @CreatedAt, @BranchId, @SubscriptionId, @CompanyId);
                SELECT CAST(SCOPE_IDENTITY() as bigint);";

                            var parameters = new DynamicParameters();
                            parameters.Add("EmployeeId", userId, DbType.Int64);
                            parameters.Add("LeaveTypeId", leaveApplication.LeaveTypeId, DbType.Int64);

                            var startDate = Convert.ToDateTime(leaveApplication.StartDate);
                            var endDate = Convert.ToDateTime(leaveApplication.EndDate);
                            var numberOfDays = (endDate - startDate).TotalDays + 1;

                            parameters.Add("StartDate", leaveApplication.StartDate, DbType.String);
                            parameters.Add("EndDate", leaveApplication.EndDate, DbType.String);
                            parameters.Add("NumberOfDays", numberOfDays, DbType.Int64);
                            parameters.Add("Reason", leaveApplication.Reason, DbType.String);
                            parameters.Add("CreatedBy", userId, DbType.Int64);
                            parameters.Add("CreatedAt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DbType.String);
                            parameters.Add("BranchId", branchId, DbType.Int64);
                            parameters.Add("SubscriptionId", subscriptionId);
                            parameters.Add("CompanyId", companyId);

                            // Execute insert and get the new LeaveRecordId
                            var leaveRecordId = await connection.ExecuteScalarAsync<long>(queryString, parameters, transaction);

                            // Insert into LeaveAttachments (if attachments exist)
                            if (leaveApplication.Attachments != null && leaveApplication.Attachments.Any())
                            {
                                foreach (var file in leaveApplication.Attachments)
                                {
                                    var attachQuery = @"INSERT INTO LeaveAttachments (LeaveRecordId, FileName, FilePath, CreatedAt)
                                        VALUES (@LeaveRecordId, @FileName, @FilePath, @CreatedAt)";
                                    var attachParams = new DynamicParameters();
                                    attachParams.Add("LeaveRecordId", leaveRecordId, DbType.Int64);
                                    attachParams.Add("FileName", file.FileName, DbType.String);
                                    attachParams.Add("FilePath", file.FilePath, DbType.String);
                                    attachParams.Add("CreatedAt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DbType.String);

                                    await connection.ExecuteAsync(attachQuery, attachParams, transaction);
                                }
                            }

                            // Commit all
                            transaction.Commit();
                            return true;
                        }
                        catch (Exception)
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> UpdateLeaveApplication(LeaveApplication leaveApplication)
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

                    var queryString = "Update LeaveRecords set ApprovedByImmediateBossStatus=@ApprovedByImmediateBossStatus where Id='" + leaveApplication.Id + "' ";
                    var parameters = new DynamicParameters();
                    parameters.Add("ApprovedByImmediateBossStatus", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DbType.String);
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
