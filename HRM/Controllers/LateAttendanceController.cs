using HRM.Interfaces;
using HRM.Models;
using HRM.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace HRM.Controllers
{
    public class LateAttendanceController : Controller
    {
        private readonly ILateAttendanceService _lateAttendanceService;
        private readonly IBranchService _branchService;
        private readonly ILeaveTypeService _leaveTypeService;
        public LateAttendanceController(ILateAttendanceService lateAttendanceService, IBranchService branchService, ILeaveTypeService leaveTypeService)
        {
            _lateAttendanceService = lateAttendanceService;
            _branchService = branchService;
            _leaveTypeService = leaveTypeService;
        }
        public async Task<IActionResult> Index()
        {
            var branchList = await _branchService.GetAllBranch();
            ViewBag.BranchList = branchList.Select(b => new SelectListItem
            {
                Value = b.Id.ToString(),
                Text = b.Name
            }).ToList();
            var leaveTypeList = await _leaveTypeService.GetAllTeaveType();
            ViewBag.leaveTypeSelectList = leaveTypeList.Select(l => new SelectListItem
            {
                Value = l.Id.ToString(),
                Text = l.TypeName
            }).ToList();
            var lateAttendances = await _lateAttendanceService.GetAllLateAttendance();
            return View(lateAttendances);
        }
        [HttpPost]
        public async Task<IActionResult> CreateOrUpdate(LateAttendance lateAttendance)
        {
            if (lateAttendance.Id == 0)
            {
                bool isCreated = await _lateAttendanceService.InsertLateAttendance(lateAttendance);
                if (!isCreated)
                {
                    TempData["ErrorMessage"] = "A lateAttendance already exists for this SubscriptionId.";
                    return RedirectToAction("INdex");
                }
                TempData["SuccessMessage"] = "lateAttendance Created Successfully";
            }
            else
            {
                bool isUpdated = await _lateAttendanceService.InsertLateAttendance(lateAttendance);
                if (!isUpdated)
                {
                    TempData["ErrorMessage"] = "lateAttendance name already exists or update failed";
                    return RedirectToAction("Index");
                }
                TempData["SuccessMessage"] = "lateAttendance Updated Successfully";
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            var result = await _lateAttendanceService.DeleteLateAttendance(id);

            TempData["SuccessMessage"] = "Deleted successfully.";
            return RedirectToAction("Index");
        }
    }
}
