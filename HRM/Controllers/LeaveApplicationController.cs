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
        public async Task<IActionResult> InitialApproved()
        {
            var leaveTypes = await _leaveTypeService.GetAllTeaveType();
            ViewBag.LeaveTypeList = leaveTypes.Select(b => new SelectListItem
            {
                Value = b.Id.ToString(),
                Text = b.TypeName
            }).ToList();
            var leaveApplications = await _leaveApplicationService.GetAllInitialApproved();
            return View(leaveApplications);
        }
        [HttpPost]
        public async Task<IActionResult> InitialApprovedLeaveApplication(LeaveApplication model)
        {
            if (model == null || model.Id == 0)
            {
                TempData["ErrorMessage"] = "Invalid Leave Application data.";
                return RedirectToAction("InitialApproved");
            }

            var result = await _leaveApplicationService.UpdateLeaveApplicationByImmediateBoss(model);

            if (result)
            {
                TempData["SuccessMessage"] = "Leave Application approved successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to approve Leave Application.";
            }

            return RedirectToAction("InitialApproved");
        }


        public async Task<IActionResult> Approved()
        {
            var leaveTypes = await _leaveTypeService.GetAllTeaveType();
            ViewBag.LeaveTypeList = leaveTypes.Select(b => new SelectListItem
            {
                Value = b.Id.ToString(),
                Text = b.TypeName
            }).ToList();
            var leaveApplications = await _leaveApplicationService.GetAllApproved();
            return View(leaveApplications);
        }

        [HttpPost]
        public async Task<IActionResult> ApprovedLeaveApplication(LeaveApplication model)
        {
            if (model == null || model.Id == 0)
            {
                TempData["ErrorMessage"] = "Invalid Leave Application data.";
                return RedirectToAction("InitialApproved");
            }

            var result = await _leaveApplicationService.UpdateLeaveApplicationbyHR(model);

            if (result)
            {
                TempData["SuccessMessage"] = "Leave Application approved successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to approve Leave Application.";
            }

            return RedirectToAction("Approved");
        }


    }
}
