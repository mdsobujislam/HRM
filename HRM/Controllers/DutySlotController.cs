using HRM.Interfaces;
using HRM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HRM.Controllers
{
    public class DutySlotController : Controller
    {
        private readonly IDutySlotService _service;
        private readonly IBranchService _branchService;
        private readonly IDesignationService _designationService;
        private readonly IDepartmentService _departmentService;
        public DutySlotController(IDutySlotService service, IBranchService branchService, IDesignationService designationService, IDepartmentService departmentService)
        {
            _service = service;
            _branchService = branchService;
            _designationService = designationService;
            _departmentService = departmentService;
        }
        public async Task<IActionResult> Index()
        {
            var branchList = await _branchService.GetAllBranch();
            ViewBag.BranchList = branchList.Select(b => new SelectListItem
            {
                Value = b.Id.ToString(),
                Text = b.Name
            }).ToList();

            var designationList = await _designationService.GetAllDesignation();
            ViewBag.DesignationList = designationList.Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = d.DesignationName
            }).ToList();
            var departmentList = await _departmentService.GetAllDepartment();
            ViewBag.DepartmentList = departmentList.Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = d.DepartmentName
            }).ToList();
            var dutySlots = await _service.GetAllDutySlot();
            return View(dutySlots);
        }
        [HttpPost]
        public async Task<IActionResult> CreateOrUpdateDepartment(DutySlot dutySlot)
        {

            if (dutySlot.Id == 0)
            {
                bool isCreated = await _service.InsertDutySlot(dutySlot);
                TempData["SuccessMessage"] = isCreated ? "Duty slot created successfully." : "Failed to create duty slot.";
            }
            else
            {
                bool isUpdated = await _service.UpdateDutySlot(dutySlot);
                if (!isUpdated)
                {
                    TempData["ErrorMessage"] = "Update failed. Name may already exist.";
                    return RedirectToAction("Index");
                }
                TempData["SuccessMessage"] = "Duty slot updated successfully.";
            }

            return RedirectToAction("Index");
        }
    }
}
