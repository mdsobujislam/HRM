using HRM.Interfaces;
using HRM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace HRM.Controllers
{
    public class EmployeeSeparationController : Controller
    {
        private readonly IEmployeeSeparationService _employeeSeparationService;
        private readonly ISeparationReasonsService _separationReasonsService;
        public EmployeeSeparationController(IEmployeeSeparationService employeeSeparationService, ISeparationReasonsService separationReasonsService)
        {
            _employeeSeparationService = employeeSeparationService;
            _separationReasonsService = separationReasonsService;
        }
        public async Task<IActionResult> Index()
        {
            var separationReasons =await _separationReasonsService.GetAllSeparationReasonsAsync();
            ViewBag.SeparationReasonsList = separationReasons.Select(b => new SelectListItem
            {
                Value = b.Id.ToString(),
                Text = b.Sep_Reason
            }).ToList();
            return View();
        }
        [HttpGet]
        public async Task<JsonResult> SearchEmployeess(string term)
        {
            var employees = await _employeeSeparationService.SearchEmployees(term);
            return Json(employees);
        }
        [HttpPost]
        public async Task<IActionResult> CreateEmployeeSeparation(EmployeeSeparation employeeSeparation)
        {
            if (employeeSeparation.Id == 0)
            {
                bool isCreated = await _employeeSeparationService.InsertEmployeeSeparationAsync(employeeSeparation);
                if (!isCreated)
                {
                    TempData["ErrorMessage"] = "An Employee Separation already exists for this Employee.";
                    return RedirectToAction("Index");
                }
                TempData["SuccessMessage"] = "Employee Separation Created Successfully";
            }
            return RedirectToAction("Index");
        }

    }
}
