using HRM.Interfaces;
using HRM.Models;
using HRM.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace HRM.Controllers
{
    public class OffDaysController : Controller
    {
        private readonly IOffDaysService _offDaysService;
        private readonly IBranchService _branchService;
        private readonly IDesignationService _designationService;
        private readonly IDepartmentService _departmentService;
        public OffDaysController(IOffDaysService offDaysService, IBranchService branchService, IDesignationService designationService, IDepartmentService departmentService)
        {
            _offDaysService = offDaysService;
            _branchService = branchService;
            _designationService = designationService;
            _departmentService = departmentService;
        }
        public async Task<IActionResult> Index()
        {
            var branchList =await _branchService.GetAllBranch();
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
            var offDays = await _offDaysService.GetAllOffDays();
            return View(offDays);
        }
        [HttpPost]
        public async Task<IActionResult> CreateOrUpdate(OffDays offDays)
        {
            if (offDays.Id == 0)
            {
                bool isCreated = await _offDaysService.InsertOffDays(offDays);
                if (!isCreated)
                {
                    TempData["ErrorMessage"] = "A offDays already exists for this SubscriptionId.";
                    return RedirectToAction("INdex");
                }
                TempData["SuccessMessage"] = "lateAttendance Created Successfully";
            }
            else
            {
                bool isUpdated = await _offDaysService.InsertOffDays(offDays);
                if (!isUpdated)
                {
                    TempData["ErrorMessage"] = "offDays name already exists or update failed";
                    return RedirectToAction("Index");
                }
                TempData["SuccessMessage"] = "lateAttendance Updated Successfully";
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _offDaysService.DeleteOffDays(id);

            TempData["SuccessMessage"] = "Deleted successfully.";
            return RedirectToAction("Index");
        }
    }
}
