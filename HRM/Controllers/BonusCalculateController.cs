using HRM.Interfaces;
using HRM.Models;
using HRM.Services;
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
        private readonly IBonusTypeService _bonusTypeService;

        public BonusCalculateController(IBonusCalculateService bonusCalculateService, IBranchService branchService, IDesignationService designationService, IDepartmentService departmentService, IEmployeeService employeeService, IBonusTypeService bonusTypeService)
        {
            _bonusCalculateService = bonusCalculateService;
            _branchService = branchService;
            _designationService = designationService;
            _departmentService = departmentService;
            _employeeService = employeeService;
            _bonusTypeService = bonusTypeService;
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
            var bonusTypes = await _bonusTypeService.GetAllBonusType();
            ViewBag.BonusTypeList = bonusTypes.Select(b => new SelectListItem
            {
                Value = b.Id.ToString(),
                Text = b.BonusTypesName
            }).ToList();

            var bonusCalculates =await _bonusCalculateService.GetAllAsync();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Insert(BonusCalculate bonusCalculate)
        {
            bool isCreated = await _bonusCalculateService.InsertBonusCalculate(bonusCalculate);
            TempData["SuccessMessage"] = isCreated ? "Bonus Calculate created successfully." : "Failed to create Bonus Calculate.";

            return RedirectToAction("Index");
        }
    }
}
