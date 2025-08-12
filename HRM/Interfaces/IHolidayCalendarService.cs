using HRM.Models;

namespace HRM.Interfaces
{
    public interface IHolidayCalendarService
    {
        Task<List<HolidayCalendar>> GetAllHolidayCalendar();
        Task<bool> InsertHolidayCalendar(HolidayCalendar holidayCalendar);
        Task<bool> UpdateHolidayCalendar(HolidayCalendar holidayCalendar);
        Task<bool> DeleteHolidayCalendar(int id);
    }
}
