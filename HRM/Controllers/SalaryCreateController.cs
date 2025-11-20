using HRM.Interfaces;
using HRM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Operations;

namespace HRM.Controllers
{
    public class SalaryCreateController : Controller
    {
        private readonly ISalaryCreateService _salaryCreateService;
        private readonly IBranchService _branchService;
        private readonly ISalaryHeadsService _salaryHeadsService;
        private readonly IDepartmentService _departmentService;
        private readonly IDesignationService _designationService;
        private readonly IEmployeeService _employeeService;
        public SalaryCreateController(ISalaryCreateService salaryCreateService, IBranchService branchService, ISalaryHeadsService salaryHeadsService, IDepartmentService departmentService, IDesignationService designationService, IEmployeeService employeeService)
        {
            _salaryCreateService = salaryCreateService;
            _branchService = branchService;
            _salaryHeadsService = salaryHeadsService;
            _departmentService = departmentService;
            _designationService = designationService;
            _employeeService = employeeService;
        }

        public async Task<IActionResult> Index(int branchId, string monthSelect)
        {
            try
            {
                var branchList = await _branchService.GetAllBranch();
                ViewBag.BranchList = branchList.Select(b => new SelectListItem
                {
                    Value = b.Id.ToString(),
                    Text = b.Name
                }).ToList();

                var salaryCreates = await _salaryCreateService.GetAllSalaryCreateAsync(branchId, monthSelect);

                if (salaryCreates != null && !salaryCreates.Any())
                {
                    TempData["Error"] = $"Salary for the month of {monthSelect} has already been created.";
                    return View(new List<SalaryCreate>());
                }

                return View(salaryCreates);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading salary page: " + ex.Message;
                return View();
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSalary(List<SalaryCreate> SalaryList)
        {
            if (SalaryList == null || !SalaryList.Any())
            {
                TempData["Error"] = "No salary data to save.";
                return RedirectToAction("Index");
            }

            try
            {
                var ok = await _salaryCreateService.InsertSalaryListAsync(SalaryList);
                if (ok) TempData["Success"] = "Salary entries created successfully.";
                else TempData["Error"] = "Salary heads not found for this subscription.";
            }
            catch (System.Exception ex)
            {
                TempData["Error"] = "Error creating salary entries: " + ex.Message;
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> SalaryReport(int branchId, string monthSelect)
        {
            try
            {
                var branchList = await _branchService.GetAllBranch();
                ViewBag.BranchList = branchList.Select(b => new SelectListItem
                {
                    Value = b.Id.ToString(),
                    Text = b.Name
                }).ToList();

                var salaryCreates = await _salaryCreateService.GetSalaryReportCreateAsync(branchId, monthSelect);

                if (salaryCreates != null && !salaryCreates.Any())
                {
                    TempData["Error"] = $"Salary for the month of {monthSelect} has already been created.";
                    return View(new List<SalaryCreate>());
                }

                return View(salaryCreates);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading salary page: " + ex.Message;
                return View();
            }
        }

    }
}
