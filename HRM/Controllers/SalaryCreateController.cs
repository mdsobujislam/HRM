using HRM.Interfaces;
using HRM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

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

                // 👇 Pass both branch and month
                var salaryCreates = await _salaryCreateService.GetAllSalaryCreateAsync(branchId, monthSelect);

                if (salaryCreates == null || !salaryCreates.Any())
                    return View(new List<SalaryCreate>());

                return View(salaryCreates);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading salary page: " + ex.Message;
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateSalary(SalaryCreate salaryCreate)
        {
            try
            {
                var result = await _salaryCreateService.InsertSalaryCreateAsync(salaryCreate);
                // Logic to create salary entries
                TempData["Success"] = "Salary entries created successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error creating salary entries: " + ex.Message;
            }
            return RedirectToAction("Index");
        }
    }
}
