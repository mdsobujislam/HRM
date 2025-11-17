using Attendance.API.Interfaces;
using Attendance.API.Models.Attendance;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace Attendance.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceService _attendanceService;
        private readonly IConfiguration _configuration;

        public AttendanceController(IAttendanceService attendanceService, IConfiguration configuration)
        {
            _attendanceService = attendanceService;
            _configuration = configuration; // Only needed for API key
        }

        [HttpPost("sync")]
        public async Task<IActionResult> SaveAttendance([FromBody] AttendanceBatchDto dto)
        {
            // 1. API Key Validation
            if (!Request.Headers.TryGetValue("X-API-KEY", out var providedKey))
            {
                return Unauthorized(new { success = false, message = "API key is missing." });
            }

            var expectedKey = _configuration["AttendanceSync:ApiKey"];
            if (expectedKey == null || providedKey != expectedKey)
            {
                return Unauthorized(new { success = false, message = "Invalid API key." });
            }

            // 2. Payload Validation
            if (dto == null || dto.Records == null || dto.Records.Count == 0)
            {
                using var reader = new StreamReader(Request.Body, Encoding.UTF8, false, -1, true);
                var rawBody = await reader.ReadToEndAsync();
                Console.WriteLine($"Raw request body: {rawBody}");
                return BadRequest(new { success = false, message = "Invalid request payload." });
            }

            // 3. Save Attendance via Dapper
            var result = await _attendanceService.SaveAttendanceAsync(dto);

            if (!result)
            {
                return StatusCode(500, new { success = false, message = "Failed to save attendance data." });
            }

            return Ok(new { success = true, message = "Attendance saved successfully." });
        }
    }
}
