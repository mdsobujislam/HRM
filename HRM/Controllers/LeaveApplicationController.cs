using HRM.Interfaces;
using HRM.Models;
using HRM.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace HRM.Controllers
{
    public class LeaveApplicationController : Controller
    {
        private readonly ILeaveApplicationService _leaveApplicationService;
        private readonly ILeaveTypeService _leaveTypeService;
        public LeaveApplicationController(ILeaveApplicationService leaveApplicationService, ILeaveTypeService leaveTypeService)
        {
            _leaveApplicationService = leaveApplicationService;
            _leaveTypeService = leaveTypeService;
        }
        public async Task<IActionResult> Index()
        {
            var leaveTypes = await _leaveTypeService.GetAllTeaveType();
            ViewBag.LeaveTypeList = leaveTypes.Select(b => new SelectListItem
            {
                Value = b.Id.ToString(),
                Text = b.TypeName
            }).ToList();

            var leaveApplications =await  _leaveApplicationService.GetAllLeaveApplicationByAsync();
            return View(leaveApplications);
        }

        [HttpPost]
        public async Task<IActionResult> CreateLeaveApplication(LeaveApplication model, List<IFormFile> Documents)
        {
            bool isCreated = await _leaveApplicationService.InsertLeaveApplication(model, Documents);

            if (!isCreated)
            {
                TempData["ErrorMessage"] = "A leave application already exists for this employee or period.";
                return RedirectToAction("Index");
            }

            TempData["SuccessMessage"] = "Leave application created successfully.";
            return RedirectToAction("Index");
        }



    }
}
