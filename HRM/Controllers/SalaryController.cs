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
        public SalaryController(ISalaryService salaryService, IBranchService branchService, ISalaryHeadsService salaryHeadsService)
        {
            _salaryService = salaryService;
            _branchService = branchService;
            _salaryHeadsService = salaryHeadsService;
        }
        public async Task<IActionResult> Index()
        {
            var salaryHeadsList = await _salaryHeadsService.GetAllSalaryHeads();
            ViewBag.SalaryHeadsList = salaryHeadsList.Select(s => new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = s.Salaryitems
            }).ToList();
            var branchList =await _branchService.GetAllBranch();
            ViewBag.BranchList = branchList.Select(b => new SelectListItem
            {
                Value = b.Id.ToString(),
                Text = b.Name
            }).ToList();
            var salaryList =await _salaryService.GetAllSalary();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SaveSalary(int[] SalaryHeadsId, int[] BranchId, int[] Sl, string[] Parameter, double[] Value)
        {
            for (int i = 0; i < SalaryHeadsId.Length; i++)
            {
                var salary = new Salary
                {
                    SalaryHeadsId = SalaryHeadsId[i],
                    BranchId = BranchId[i],
                    Sl = Sl.Length > i ? Sl[i] : 0,
                    Parameter = Parameter.Length > i ? Parameter[i] : null,
                    Value = Value[i]
                };

                await _salaryService.InsertSalary(salary);
            }

            return RedirectToAction("Index");
        }


    }
}
