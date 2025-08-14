using HRM.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace HRM.Controllers
{
    public class BonusCalculateController : Controller
    {
        private readonly IBonusCalculateService _bonusCalculateService;
        private readonly IBranchService _branchService;
        private readonly IDesignationService _designationService;
        private readonly IDepartmentService _departmentService;
        private readonly IEmployeeService _employeeService;

        public BonusCalculateController(IBonusCalculateService bonusCalculateService, IBranchService branchService, IDesignationService designationService, IDepartmentService departmentService, IEmployeeService employeeService)
        {
            _bonusCalculateService = bonusCalculateService;
            _branchService = branchService;
            _designationService = designationService;
            _departmentService = departmentService;
            _employeeService = employeeService;
        }
        public async Task<IActionResult> Index()
        {
            var branches = await _branchService.GetAllBranch();
            ViewBag.BranchList = branches.Select(b => new SelectListItem
            {
                Value = b.Id.ToString(),
                Text = b.Name
            }).ToList();
            var designations = await _designationService.GetAllDesignation();
            ViewBag.DesignationList = designations.Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = d.DesignationName
            }).ToList();
            var departments = await _departmentService.GetAllDepartment();
            ViewBag.DepartmentList = departments.Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = d.DepartmentName
            }).ToList();
            var employees = await _employeeService.GetAllEmployee();
            ViewBag.EmployeeList = employees.Select(e => new SelectListItem
            {
                Value = e.EmpId.ToString(),
                Text = e.EmployeeName
            }).ToList();

            var bonusCalculates =await _bonusCalculateService.GetAllAsync();
            return View();
        }
    }
}
