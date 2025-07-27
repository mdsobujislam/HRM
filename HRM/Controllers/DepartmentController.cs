using HRM.Interfaces;
using HRM.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HRM.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly IDepartmentService _departmentService;
        public DepartmentController(IDepartmentService departmentService)
        {
            _departmentService = departmentService ?? throw new ArgumentNullException(nameof(departmentService));
        }
        public async Task<IActionResult> Index()
        {
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
