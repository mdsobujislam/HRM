using HRM.Interfaces;
using HRM.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HRM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdditionalInfoController : ControllerBase
    {
        private readonly IAdditionalInfoService _additionalInfoService;

        public AdditionalInfoController(IAdditionalInfoService additionalInfoService)
        {
            _additionalInfoService = additionalInfoService;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var list = await _additionalInfoService.GetAllAsync();
            var dto = list.Select(x => new { id = x.Id, name = x.AdditionalInfoName });
            return Ok(dto);
        }

        [HttpPost("Add")]
        public async Task<IActionResult> Add([FromBody] AdditionalInfo model)
        {
            if (string.IsNullOrWhiteSpace(model.AdditionalInfoName))
                return BadRequest("Invalid name.");

            var result = await _additionalInfoService.InsertAdditionalInfo(model);
            if (result == null)
                return StatusCode(500, "Save failed");

            return Ok(new { id = result.Id, name = result.AdditionalInfoName });
        }
    }
}
