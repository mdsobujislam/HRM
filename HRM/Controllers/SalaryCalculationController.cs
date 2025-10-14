using HRM.Interfaces;
using HRM.Models;
using HRM.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace HRM.Controllers
{
    public class SalaryCalculationController : Controller
    {
        private readonly ISalaryCalculationService _salaryCalculationService;
        private readonly ISalaryHeadsService _salaryHeadsService;
        private readonly IBranchService _branchService;
        private readonly IDepartmentService _departmentService;
        private readonly IDesignationService _designationService;

        public SalaryCalculationController(ISalaryHeadsService salaryHeadsService, IBranchService branchService, ISalaryCalculationService salaryCalculationService, IDepartmentService departmentService, IDesignationService designationService)
        {
            _salaryHeadsService = salaryHeadsService;
            _branchService = branchService;
            _salaryCalculationService = salaryCalculationService;
            _departmentService = departmentService;
            _designationService = designationService;
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
            
            var salaryHeads = await _salaryCalculationService.GetAllSalaryHeadsAsync();


            return View(salaryHeads);
        }
        [HttpPost]
        public async Task<IActionResult> Insert(SalaryCalculationMaster model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (model.SalaryItems == null || !model.SalaryItems.Any())
            {
                TempData["Error"] = "No salary items found. Please enter values.";
                return RedirectToAction("Index");
            }

            var result = await _salaryCalculationService.InsertSalaryCalculationAsync(model);

            if (result)
                TempData["Success"] = "Salary calculation saved successfully!";
            else
                TempData["Error"] = "Failed to save salary calculation.";

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<JsonResult> SearchEmployees(string term)
        {
            var employees = await _salaryCalculationService.SearchEmployees(term);
            return Json(employees);
        }

    }
}
