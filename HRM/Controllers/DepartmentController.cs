using HRM.Interfaces;
using HRM.Models;
using HRM.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace HRM.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly IDepartmentService _departmentService;
        private readonly IBranchService _branchService;
        public DepartmentController(IDepartmentService departmentService, IBranchService branchService)
        {
            _departmentService = departmentService ?? throw new ArgumentNullException(nameof(departmentService));
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

            var departments =await _departmentService.GetAllDepartment();
            return View(departments);
        }
        [HttpPost]
        public async Task<IActionResult> CreateOrUpdateDepartment(Department department)
        {
            if (department.Id == 0)
            {
                bool isCreated = await _departmentService.InsertDepartment(department);
                if (!isCreated)
                {
                    TempData["ErrorMessage"] = "A Department already exists for this SubscriptionId.";
                    return RedirectToAction("Index");
                }
                TempData["SuccessMessage"] = "Department Created Successfully";
            }
            else
            {
                bool isUpdated = await _departmentService.UpdateDepartment(department);
                if (!isUpdated)
                {
                    TempData["ErrorMessage"] = "Department name already exists or update failed";
                    return RedirectToAction("Index");
                }
                TempData["SuccessMessage"] = "Branch Updated Successfully";
            }

            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            var result = await _departmentService.DeleteDepartment(id);

            TempData["SuccessMessage"] = "Branch deleted successfully.";
            return RedirectToAction("Index");
        }
    }
}
