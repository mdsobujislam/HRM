using Dapper;
using HRM.Interfaces;
using HRM.Models;
using Microsoft.Data.SqlClient;

namespace HRM.Services
{
    public class HolidayCalendarService : IHolidayCalendarService
    {
        private readonly string _connectionString;
        private readonly BaseService _baseService;

        public HolidayCalendarService(IConfiguration configuration, BaseService baseService)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(_connectionString));
            _baseService = baseService;
        }
        public Task<bool> DeleteHolidayCalendar(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<HolidayCalendar>> GetAllHolidayCalendar()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var subscriptionId = _baseService.GetSubscriptionId();
                    var userId = _baseService.GetUserId();
                    var branchId = await _baseService.GetBranchId(subscriptionId, userId);

                    var query = @"select day from calendar order by day ";

                    var result = await connection.QueryAsync<HolidayCalendar>(query, new { subscriptionId });
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task<bool> InsertHolidayCalendar(HolidayCalendar holidayCalendar)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateHolidayCalendar(HolidayCalendar holidayCalendar)
        {
            throw new NotImplementedException();
        }
    }
}
