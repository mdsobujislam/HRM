using HRM.Interfaces;
using HRM.Models;
using Microsoft.AspNetCore.Mvc;

namespace HRM.Controllers
{
    public class AttendanceController : Controller
    {
        private readonly IAttendanceService _attendanceService;
        public AttendanceController(IAttendanceService attendanceService)
        {
            _attendanceService = attendanceService ?? throw new ArgumentNullException(nameof(attendanceService));
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> InsertAttendence(Attendance attendance)
        {
            if (attendance == null)
            {
                TempData["ErrorMessage"] = ("Attendance data is required.");
                return RedirectToAction("Index");
            }
            var result = await _attendanceService.InsertAttendance(attendance);
            if (result)
            {
                TempData["SuccessMessage"]=("Attendance recorded successfully.");
            }
            else
            {
                TempData["ErrorMessage"]=("logged in less than 1 hour ago");
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }
    }
}
