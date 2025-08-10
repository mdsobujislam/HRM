using HRM.Interfaces;
using HRM.Models;
using HRM.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HRM.Controllers
{
    public class LeaveTypeController : Controller
    {
        private readonly ILeaveTypeService _leaveTypeService;
        private readonly IBranchService _branchService;
        public LeaveTypeController(ILeaveTypeService leaveTypeService, IBranchService branchService)
        {
            _leaveTypeService = leaveTypeService ?? throw new ArgumentNullException(nameof(leaveTypeService));
            _branchService = branchService;
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
            return View(leaveTypeList);
        }
        [HttpPost]
        public async Task<IActionResult> CreateOrUpdate(LeaveType leaveType)
        {
            if (leaveType.Id == 0)
            {
                bool isCreated = await _leaveTypeService.InsertTeaveType(leaveType);
                if (!isCreated)
                {
                    TempData["ErrorMessage"] = "A leaveType already exists for this SubscriptionId.";
                    return RedirectToAction("INdex");
                }
                TempData["SuccessMessage"] = "leaveType Created Successfully";
            }
            else
            {
                bool isUpdated = await _leaveTypeService.UpdateTeaveType(leaveType);
                if (!isUpdated)
                {
                    TempData["ErrorMessage"] = "leaveType name already exists or update failed";
                    return RedirectToAction("Index");
                }
                TempData["SuccessMessage"] = "leaveType Updated Successfully";
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            var result = await _leaveTypeService.DeleteTeaveType(id);

            TempData["SuccessMessage"] = "Deleted successfully.";
            return RedirectToAction("Index");
        }
    }
}
