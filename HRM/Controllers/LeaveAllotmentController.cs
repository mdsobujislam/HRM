using HRM.Interfaces;
using HRM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace HRM.Controllers
{
    public class LeaveAllotmentController : Controller
    {
        private readonly ILeaveAllotmentService _leaveAllotmentService;
        private readonly IBranchService _branchService;
        private readonly ILeaveTypeService _leaveTypeService;
        public LeaveAllotmentController(ILeaveAllotmentService leaveAllotmentService, IBranchService branchService, ILeaveTypeService leaveTypeService)
        {
            _leaveAllotmentService = leaveAllotmentService;
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

            var leaveAllotments = await _leaveAllotmentService.GetAllLeaveAllotment();
            return View(leaveAllotments);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrUpdate(LeaveAllotment leaveAllotment)
        {
            if (leaveAllotment.Id == 0)
            {
                bool isCreated = await _leaveAllotmentService.InsertLeaveAllotment(leaveAllotment);
                if (!isCreated)
                {
                    TempData["ErrorMessage"] = "A leaveAllotment already exists for this SubscriptionId.";
                    return RedirectToAction("INdex");
                }
                TempData["SuccessMessage"] = "leaveAllotment Created Successfully";
            }
            else
            {
                bool isUpdated = await _leaveAllotmentService.InsertLeaveAllotment(leaveAllotment);
                if (!isUpdated)
                {
                    TempData["ErrorMessage"] = "leaveAllotment name already exists or update failed";
                    return RedirectToAction("Index");
                }
                TempData["SuccessMessage"] = "leaveAllotment Updated Successfully";
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
