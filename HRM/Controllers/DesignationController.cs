using HRM.Interfaces;
using HRM.Models;
using HRM.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HRM.Controllers
{
    public class DesignationController : Controller
    {
        private readonly IDesignationService _designationService;
        private readonly IBranchService _branchService;
        private readonly IDepartmentService _departmentService;
        public DesignationController(IDesignationService designationService,IBranchService branchService, IDepartmentService departmentService)
        {
            _designationService = designationService ?? throw new ArgumentNullException(nameof(designationService));
            _branchService = branchService;
            _departmentService = departmentService;
        }
        [HttpGet]
        public async Task<IActionResult>  Index()
        {
            var branchList=await _branchService.GetAllBranch();
            ViewBag.BranchList = branchList.Select(b => new SelectListItem
            {
                Value = b.Id.ToString(),
                Text = b.Name
            }).ToList();

            var departmentList = await _departmentService.GetAllDepartment();
            ViewBag.DepartmentList = departmentList.Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = d.DepartmentName
            }).ToList();

            var designationList = await _designationService.GetAllDesignation();
            return View(designationList);
        }

        [HttpGet]
        public async Task<JsonResult> GetDepartmentsByBranch(int branchId)
        {
            var departments = await _departmentService.GetDepartmentsByBranchId(branchId);

            var departmentList = departments.Select(d => new
            {
                id = d.Id,
                name = d.DepartmentName
            });

            return Json(departmentList);
        }


        [HttpPost]
        public async Task<IActionResult> CreateOrUpdate(Designation designation)
        {
            if (designation.Id == 0)
            {
                bool isCreated = await _designationService.InsertDesignation(designation);
                if (!isCreated)
                {
                    TempData["ErrorMessage"] = "A designation already exists for this SubscriptionId.";
                    return RedirectToAction("INdex");
                }
                TempData["SuccessMessage"] = "User Created Successfully";
            }
            else
            {
                bool isUpdated = await _designationService.UpdateDesignation(designation);
                if (!isUpdated)
                {
                    TempData["ErrorMessage"] = "designation name already exists or update failed";
                    return RedirectToAction("Index");
                }
                TempData["SuccessMessage"] = "User Updated Successfully";
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            var result = await _designationService.DeleteDesignation(id);

            TempData["SuccessMessage"] = "Designation deleted successfully.";
            return RedirectToAction("Index");
        }

    }
}
