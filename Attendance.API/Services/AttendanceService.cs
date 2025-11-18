using Attendance.API.Interfaces;
using Attendance.API.Models.Attendance;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Attendance.API.Services
{
    public class AttendanceService: IAttendanceService
    {
        private readonly string _connectionString;

        public AttendanceService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(_connectionString));
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
                            var insertBatchQuery = @" INSERT INTO AttendanceBatches (BatchId, DeviceIp, MachineNumber, SentAt, CreatedAt) VALUES (@BatchId, @DeviceIp, @MachineNumber, @SentAt, @CreatedAt); SELECT CAST(SCOPE_IDENTITY() as bigint);";

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

                            var insertRecordsQuery = @" INSERT INTO AttendanceRecords (AttendanceBatchId, EnrollId, UserIdFromDevice, VerifyMode, InOutMode, DeviceTimestamp, Timestamp, WorkCode, RecordKey) VALUES (@AttendanceBatchId, @EnrollId, @UserIdFromDevice, @VerifyMode, @InOutMode, @DeviceTimestamp, @Timestamp, @WorkCode, @RecordKey);";

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

                            transaction.Commit();
                            return true;
                        }
                        catch
                        {
                            transaction.Rollback();
                            return false;
                        }
                    }
                }
            }
            catch
            {
                return false;
            }
        }

    }
}
