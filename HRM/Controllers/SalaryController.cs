
using HRM.Interfaces;
using HRM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace HRM.Controllers
{
    public class SalaryController : Controller
    {
        private readonly ISalaryService _salaryService;
        private readonly IBranchService _branchService;
        private readonly ISalaryHeadsService _salaryHeadsService;
        private readonly IDepartmentService _departmentService;
        private readonly IDesignationService _designationService;
        private readonly IEmployeeService _employeeService;
        private readonly ILogger<SalaryController> _logger;
        public SalaryController(ISalaryService salaryService, IBranchService branchService, ISalaryHeadsService salaryHeadsService, IDepartmentService departmentService, IDesignationService designationService, IEmployeeService employeeService, ILogger<SalaryController> logger)
        {
            _salaryService = salaryService;
            _branchService = branchService;
            _salaryHeadsService = salaryHeadsService;
            _departmentService = departmentService;
            _designationService = designationService;
            _employeeService = employeeService;
            _logger = logger;
        }
        public async Task<IActionResult> Index()
        {
            try
            {
                var salaryHeadsList = await _salaryHeadsService.GetAllSalaryHeads();
                ViewBag.SalaryHeadsList = salaryHeadsList.Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Salaryitems
                }).ToList();

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

                var employeeList = await _employeeService.GetAllEmployee();
                ViewBag.EmployeeList = employeeList.Select(e => new SelectListItem
                {
                    Value = e.EmpId.ToString(),
                    Text = e.EmployeeName
                }).ToList();

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading salary page");
                TempData["Error"] = "Error loading salary page: " + ex.Message;
                return View();
            }
        }
     

        [HttpPost]
        public async Task<IActionResult> SaveSalary(List<Salary> Salaries)
        {
            if (!Salaries.Any())
            {
                TempData["Error"] = "No salary records to save";
                return RedirectToAction("Index");
            }

            try
            {
                var success = await _salaryService.InsertSalary(Salaries);

                if (success)
                {
                    var employeeCount = Salaries.Select(s => s.EmployeeId).Distinct().Count();
                    TempData["Success"] = $"Added salary records for {employeeCount} employees";
                }
                else
                {
                    TempData["Error"] = "Failed to save salaries";
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error saving salaries: " + ex.Message;
                return RedirectToAction("Index");
            }
        }
        // In your EmployeeController
        [HttpGet]
        public async Task<IActionResult> GetByBranch(int branchId)
        {
            try
            {
                if (branchId <= 0)
                {
                    return BadRequest("Invalid branch ID");
                }

                var employees = await _salaryService.GetEmployeesByBranch(branchId);

                var result = employees.Select(e => new
                {
                    value = e.EmpId,
                    text = e.EmployeeName,
                    departmentId = e.DepartmentId,
                    designationId = e.DesignationId,
                    branchId = e.BranchId
                }).ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
        // Add these methods to your SalaryController

        [HttpPost]
        public async Task<IActionResult> CalculateSalary([FromBody] CalculationRequest request)
        {
            try
            {
                if (request == null || request.SalaryValues == null)
                {
                    return BadRequest(new { Success = false, Error = "Invalid request" });
                }

                // Get employees based on calculation type
                List<Employee> employees = new List<Employee>();

                switch (request.CalculationType)
                {
                    case 1: // Branch-wise
                        if (!request.BranchId.HasValue || request.BranchId <= 0)
                            return BadRequest(new { Success = false, Error = "Please select a branch" });
                        employees = await _salaryService.GetEmployeesByCriteria(branchId: request.BranchId);
                        break;

                    case 2: // Department-wise
                        if (!request.DepartmentId.HasValue || request.DepartmentId <= 0)
                            return BadRequest(new { Success = false, Error = "Please select a department" });
                        employees = await _salaryService.GetEmployeesByCriteria(departmentId: request.DepartmentId);
                        break;

                    case 3: // Designation-wise
                        if (!request.DesignationId.HasValue || request.DesignationId <= 0)
                            return BadRequest(new { Success = false, Error = "Please select a designation" });
                        employees = await _salaryService.GetEmployeesByCriteria(designationId: request.DesignationId);
                        break;

                    case 4: // Employee-wise
                        if (!request.EmployeeId.HasValue || request.EmployeeId <= 0)
                            return BadRequest(new { Success = false, Error = "Please select an employee" });
                        employees = await _salaryService.GetEmployeesByCriteria(employeeId: request.EmployeeId);
                        break;

                    default:
                        return BadRequest(new { Success = false, Error = "Invalid calculation type" });
                }

                if (!employees.Any())
                {
                    return BadRequest(new { Success = false, Error = "No employees found for the selected criteria" });
                }

                // Get all salary heads for the first employee's branch
                // (assuming all employees in the batch have the same branch)
                var allSalaryHeads = await _salaryService.GetSalaryHeadsByBranch(employees.First().BranchId);
                var salaryHeadNames = allSalaryHeads.ToDictionary(sh => sh.Id, sh => sh.Salaryitems);

                // Prepare salary records
                var salaries = new List<Salary>();
                var (fromDate, toDate) = _salaryService.CalculateMonthDates(request.MonthIndex, DateTime.Now.Year);

                foreach (var employee in employees)
                {
                    double totalAmount = 0;
                    var salaryItems = new List<SalaryItem>();

                    // Add all salary heads, with 0 value for those not provided
                    foreach (var salaryHead in allSalaryHeads)
                    {
                        double value = request.SalaryValues.ContainsKey(salaryHead.Id)
                            ? request.SalaryValues[salaryHead.Id]
                            : 0;

                        totalAmount += value;

                        salaryItems.Add(new SalaryItem
                        {
                            Id = salaryHead.Id,
                            Value = value
                        });
                    }

                    var salary = new Salary
                    {
                        SalaryHeadsId = allSalaryHeads.FirstOrDefault()?.Id ?? 0,
                        BranchId = employee.BranchId,
                        DepartmentId = employee.DepartmentId,
                        DesignationId = employee.DesignationId,
                        EmployeeId = employee.EmpId,
                        Value = totalAmount,
                        FinalAmount = totalAmount,
                        MonthIndex = request.MonthIndex,
                        Month = request.Month,
                        FromDate = fromDate,
                        ToDate = toDate,
                        GenDate = DateTime.Now,
                        SalaryItems = salaryItems
                    };

                    salaries.Add(salary);
                }

                // Return the calculated salaries as JSON for preview
                return Ok(new
                {
                    Success = true,
                    Salaries = salaries.Select(s => new {
                        s.EmployeeId,
                        EmployeeName = employees.First(e => e.EmpId == s.EmployeeId)?.EmployeeName,
                        s.BranchId,
                        Branch = employees.First(e => e.EmpId == s.EmployeeId)?.Branch,
                        s.DepartmentId,
                        Department = employees.First(e => e.EmpId == s.EmployeeId)?.Department,
                        s.DesignationId,
                        Designation = employees.First(e => e.EmpId == s.EmployeeId)?.Designation,
                        s.MonthIndex,
                        s.Month,
                        s.FinalAmount,
                        s.SalaryHeadsId,
                        SalaryItems = s.SalaryItems.Select(i => new { i.Id, i.Value })
                    }),
                    EmployeeCount = employees.Count,
                    TotalAmount = salaries.Sum(s => s.FinalAmount),
                    SalaryHeadNames = salaryHeadNames // Add this line

                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating salary");
                return StatusCode(500, new { Success = false, Error = ex.Message });
            }
        }
        [HttpPost]

        [HttpPost]
        public async Task<IActionResult> SaveCalculatedSalaries([FromBody] List<Salary> salaries)
        {
            try
            {
                if (salaries == null || !salaries.Any())
                {
                    return BadRequest(new { Success = false, Message = "No salary records to save" });
                }

                var success = await _salaryService.InsertSalary(salaries);

                if (success)
                {
                    var employeeCount = salaries.Select(s => s.EmployeeId).Distinct().Count();
                    return Ok(new { Success = true, Message = $"Added salary records for {employeeCount} employees" });
                }
                else
                {
                    return BadRequest(new { Success = false, Message = "Failed to save salaries or all records were duplicates" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving calculated salaries");
                return StatusCode(500, new { Success = false, Message = "Error saving salaries: " + ex.Message });
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetBranchFromDepartment(int departmentId)
        {
            try
            {
                if (departmentId <= 0)
                {
                    return BadRequest("Invalid department ID");
                }

                var branchId = await _salaryService.GetBranchIdFromDepartment(departmentId);
                return Ok(new { branchId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting branch from department {DepartmentId}", departmentId);
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetBranchFromDesignation(int designationId)
        {
            try
            {
                if (designationId <= 0)
                {
                    return BadRequest("Invalid designation ID");
                }

                var branchId = await _salaryService.GetBranchIdFromDesignation(designationId);
                return Ok(new { branchId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting branch from designation {DesignationId}", designationId);
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetBranchFromEmployee(int employeeId)
        {
            try
            {
                if (employeeId <= 0)
                {
                    return BadRequest("Invalid employee ID");
                }

                var branchId = await _salaryService.GetBranchIdFromEmployee(employeeId);
                return Ok(new { branchId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting branch from employee {EmployeeId}", employeeId);
                return StatusCode(500, new { error = ex.Message });
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetSalaryHeadsByBranch(int branchId)
        {
            try
            {
                if (branchId <= 0)
                {
                    return BadRequest("Invalid branch ID");
                }

                var salaryHeads = await _salaryService.GetSalaryHeadsByBranch(branchId);
                return Ok(salaryHeads);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting salary heads for branch {BranchId}", branchId);
                return StatusCode(500, new { error = ex.Message });
            }
        }
        public class CalculationRequest
        {
            public int CalculationType { get; set; }
            public int? BranchId { get; set; }
            public int? DepartmentId { get; set; }
            public int? DesignationId { get; set; }
            public int? EmployeeId { get; set; }
            public int MonthIndex { get; set; }
            public string Month { get; set; }
            public Dictionary<int, double> SalaryValues { get; set; }
        }
    }
}