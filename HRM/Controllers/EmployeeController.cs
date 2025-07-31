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
        public async Task<IActionResult> Index()
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

            //var employees =await _employeeService.GetAllEmployee();
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetEmployee()
        {
            var employee = await _employeeService.GetAllEmployee();
            return View(employee);
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
            return RedirectToAction("Index");
        }


        //[HttpGet]
        //public async Task<IActionResult> CreateOrUpdate(int? id)
        //{
        //    if (id.HasValue)
        //    {
        //        var employee = await _employeeService.GetEmployeeById(id.Value);
        //        if (employee == null)
        //        {
        //            return NotFound();
        //        }
        //        return View(employee);
        //    }
        //    else
        //    {
        //        return View(new Employee());
        //    }
        //}
    }
}
