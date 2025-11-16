using Dapper;
using HRM.Interfaces;
using HRM.Models.Attendance;
using Microsoft.Data.SqlClient;
using SkiaSharp;

namespace HRM.Services
{
    public class AttendanceService : IAttendanceService
    {
        private readonly string _connectionString;
        private readonly BaseService _baseService;

        public AttendanceService(IConfiguration configuration, BaseService baseService)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(_connectionString));
            _baseService = baseService;
        }
        public Task<bool> AddManualAttendanceAsync(ManualAttendanceDto dto, int adminId)
        {
            throw new NotImplementedException();
        }

        public Task<List<AttendanceData>> GetAllData(int userId, bool isAdmin)
        {
            throw new NotImplementedException();
        }

        public Task<List<AttendanceData>> GetAttendanceByFilter(int userId, DateTime fromDate, DateTime toDate, bool isAdmin)
        {
            throw new NotImplementedException();
        }

        public Task<List<AttendanceReport>> GetAttendanceReport(int userId, DateTime fromDate, DateTime toDate, bool isAdmin)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> SaveAttendanceAsync(AttendanceBatchDto dto)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // Insert AttendanceBatch and get inserted ID
                            var insertBatchQuery = @"
                        INSERT INTO AttendanceBatch (BatchId, DeviceIp, MachineNumber, SentAt, CreatedAt)
                        VALUES (@BatchId, @DeviceIp, @MachineNumber, @SentAt, @CreatedAt);
                        SELECT CAST(SCOPE_IDENTITY() as bigint);";

                            var attendanceBatchId = await connection.ExecuteScalarAsync<long>(
                                insertBatchQuery,
                                new
                                {
                                    dto.BatchId,
                                    dto.DeviceIp,
                                    dto.MachineNumber,
                                    dto.SentAt,
                                    CreatedAt = DateTime.Now
                                },
                                transaction);

                            // Insert AttendanceRecords in batch
                            var insertRecordsQuery = @"
                        INSERT INTO AttendanceRecord 
                        (AttendanceBatchId, EnrollId, UserIdFromDevice, VerifyMode, InOutMode, DeviceTimestamp, Timestamp, WorkCode, RecordKey) 
                        VALUES 
                        (@AttendanceBatchId, @EnrollId, @UserIdFromDevice, @VerifyMode, @InOutMode, @DeviceTimestamp, @Timestamp, @WorkCode, @RecordKey);";

                            var recordsParams = dto.Records.Select(r => new
                            {
                                AttendanceBatchId = attendanceBatchId,
                                EnrollId = r.EnrollId,
                                UserIdFromDevice = r.UserId,
                                VerifyMode = r.VerifyMode,
                                InOutMode = r.InOutMode,
                                DeviceTimestamp = r.DeviceTimestamp,
                                Timestamp = r.Timestamp,
                                WorkCode = r.WorkCode,
                                RecordKey = r.RecordKey
                            }).ToList();

                            await connection.ExecuteAsync(insertRecordsQuery, recordsParams, transaction);

                            // Commit transaction if all succeeded
                            transaction.Commit();

                            return true;
                        }
                        catch (Exception ex)
                        {
                            // Rollback if any error happens during inserts
                            transaction.Rollback();

                            Console.WriteLine($"Transaction error: {ex.Message}");
                            if (ex.InnerException != null)
                            {
                                Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                            }

                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Error opening connection or other errors
                Console.WriteLine($"Connection error: {ex.Message}");
                return false;
            }

        }
    }
}
