using HRM.Interfaces;
using HRM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace HRM.Controllers
{
    public class HolidayCalendarController : Controller
    {
        private readonly IHolidayCalendarService _holidayCalendar;
        private readonly IBranchService _branchService;
        public HolidayCalendarController(IHolidayCalendarService holidayCalendar, IBranchService branchService)
        {
            _holidayCalendar = holidayCalendar;
            _branchService = branchService;
        }
        //public async Task<IActionResult> Index()
        //{
        //    var branchList = await _branchService.GetAllBranch();
        //    ViewBag.BranchList = branchList.Select(b => new SelectListItem
        //    {
        //        Value = b.Id.ToString(),
        //        Text = b.Name
        //    }).ToList();

        //    var holidayCalendars = await _holidayCalendar.GetAllHolidayCalendar();
        //    return View(holidayCalendars);
        //}
        public async Task<IActionResult> Index(int year = 2025)
        {
            var branchList = await _branchService.GetAllBranch();
            ViewBag.BranchList = branchList.Select(b => new SelectListItem
            {
                Value = b.Id.ToString(),
                Text = b.Name
            }).ToList();

            // Fetch holidays for the given year
            var holidayCalendars = await _holidayCalendar.GetAllHolidayCalendar();

            // Filter for selected year
            var holidaysForYear = holidayCalendars
                .Where(h => h.Year == year)
                .ToList();

            // Create a lookup for quick access: Key => "month-day"
            var holidayLookup = holidaysForYear
                .ToDictionary(h => $"{h.Month}-{h.Day}", h => h.HolidayName);

            ViewBag.Year = year;
            ViewBag.HolidayLookup = holidayLookup;

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SaveHolidays(int Year, string[] Holidays)
        {
            // Holidays[] will contain "month-day" strings for checked boxes
            var holidayList = Holidays.Select(h =>
            {
                var parts = h.Split('-');
                return new HolidayCalendar
                {
                    Year = Year,
                    Month = int.Parse(parts[0]),
                    Day = int.Parse(parts[1]),
                    HolidayName = "" // Could set based on another input
                };
            }).ToList();

            //await _holidayCalendar.InsertHolidayCalendar(holidayList);
            return RedirectToAction("Index", new { year = Year });
        }


    }
}
