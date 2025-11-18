using Attendance.API.Interfaces;
using Attendance.API.Models.Attendance;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;

[ApiController]
[Route("api/[controller]")]
public class AttendanceController : ControllerBase
{
    private readonly IAttendanceService _attendanceService;
    private readonly IConfiguration _configuration;

    public AttendanceController(
        IAttendanceService attendanceService,
        IConfiguration configuration)
    {
        _attendanceService = attendanceService;
        _configuration = configuration;
    }

    [HttpPost("sync")]
    public async Task<IActionResult> SaveAttendance([FromBody] AttendanceBatchDto dto)
    {
        if (!Request.Headers.TryGetValue("X-API-KEY", out var providedKey))
        {
            return Unauthorized(new { success = false, message = "API key is missing." });
        }

        var expectedKey = _configuration["AttendanceSync:ApiKey"];

        if (string.IsNullOrEmpty(providedKey))
        {
            return Unauthorized(new { success = false, message = "API key is empty." });
        }

        if (providedKey != expectedKey)
        {
            return Unauthorized(new { success = false, message = "Invalid API key." });
        }
        var result = await _attendanceService.SaveAttendanceAsync(dto);
        // Success
        return Ok(new { success = true, message = "Attendance accepted." });
    }

}
