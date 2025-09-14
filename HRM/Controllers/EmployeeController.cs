using HRM.Interfaces;
using HRM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HRM.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IEmployeeService _employeeService;
        private readonly IBranchService _branchService;
        private readonly IDepartmentService _departmentService;
        private readonly IDesignationService _designationService;
        public EmployeeController(IEmployeeService employeeService, IBranchService branchService, IDepartmentService departmentService, IDesignationService designationService)
        {
            _employeeService = employeeService;
            _branchService = branchService;
            _departmentService = departmentService;
            _designationService = designationService;
        }
        [HttpGet]
        public async Task<IActionResult> Index(int? empId)
        {
            var branchList = await _branchService.GetAllBranch();
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
            ViewBag.DesignationList = designationList.Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = d.DesignationName
            }).ToList();

            // ✅ If editing, load existing employee, else new
            Employee employee = new Employee();
            if (empId.HasValue)
            {
                employee = await _employeeService.GetEmployeeById(empId.Value);
                if (employee == null)
                {
                    return NotFound();
                }
            }

            // ✅ Pass employee list to show below form
            var employees = await _employeeService.GetAllEmployee();
            ViewBag.Employees = employees;

            return View(employee);
        }
        [HttpGet]
        public async Task<IActionResult> GetEmployee()
        {
            var employees = await _employeeService.GetAllEmployee();

            // Reuse branch/department/designation lists here too
            var branchList = await _branchService.GetAllBranch();
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
            ViewBag.DesignationList = designationList.Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = d.DesignationName
            }).ToList();

            return View(employees);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrUpdate(Employee employee, IFormFile PhotoFile)
        {
            if (employee.EmpId == 0)
            {
                bool isCreated = await _employeeService.InsertEmployee(employee, PhotoFile);
                if (!isCreated)
                {
                    TempData["ErrorMessage"] = "An employee with this details already exists.";
                    return RedirectToAction("Index");
                }
                TempData["SuccessMessage"] = "Employee Created Successfully";
            }
            else
            {
                bool isUpdated = await _employeeService.UpdateEmployee(employee, PhotoFile);
                if (!isUpdated)
                {
                    TempData["ErrorMessage"] = "Employee update failed or already exists.";
                    return RedirectToAction("Index");
                }
                TempData["SuccessMessage"] = "Employee Updated Successfully";
            }
            return RedirectToAction("GetEmployee");
        }
        [HttpPost]
        public async Task<IActionResult> UpdateEmployee(Employee employee, IFormFile PhotoFile)
        {
            var result = await _employeeService.UpdateEmployee(employee, PhotoFile);
            if (result)
                TempData["Message"] = "Employee updated successfully!";
            else
                TempData["Message"] = "Update failed!";

            return RedirectToAction("GetEmployee");
        }


        [HttpPost]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var result = await _employeeService.DeleteEmployee(id);

            if (result)
            {
                TempData["SuccessMessage"] = "Employee deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete employee.";
            }

            return RedirectToAction("GetEmployee"); 
        }

       
    }
}
