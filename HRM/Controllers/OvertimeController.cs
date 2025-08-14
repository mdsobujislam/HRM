using HRM.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace HRM.Controllers
{
    public class OvertimeController : Controller
    {
        private readonly IOvertimeService _overtimeService;
        private readonly IBranchService _branchService;
        private readonly ILeaveTypeService _leaveTypeService;
        public OvertimeController(IOvertimeService overtimeService, IBranchService branchService, ILeaveTypeService leaveTypeService)
        {
            _overtimeService = overtimeService;
            _branchService = branchService;
            _leaveTypeService = leaveTypeService;
        }
        public async Task<IActionResult> Index()
        {
            var branchList = await _branchService.GetAllBranch();
            ViewBag.BranchList = branchList.Select(b => new SelectListItem
            {
                Value = b.Id.ToString(),
                Text = b.Name
            }).ToList();
            var leaveTypeList = await _leaveTypeService.GetAllTeaveType();
            ViewBag.leaveTypeSelectList = leaveTypeList.Select(l => new SelectListItem
            {
                Value = l.Id.ToString(),
                Text = l.TypeName
            }).ToList();
            var overtimes = await _overtimeService.GetAllOvertime();
            return View(overtimes);
        }
    }
}
