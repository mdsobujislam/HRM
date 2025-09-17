using HRM.Interfaces;
using HRM.Models.ViewModels;
using HRM.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HRM.Controllers
{
    public class ExcludeTaxController : Controller
    {
        private readonly IExcludeTaxService _excludeTaxService;
        private readonly IBranchService _branchService;
        private readonly BaseService _baseService;
        public ExcludeTaxController(IExcludeTaxService excludeTaxService, IBranchService branchService, BaseService baseService)
        {
            _excludeTaxService = excludeTaxService;
            _branchService = branchService;
            _baseService = baseService;
        }

        // helper to populate filter dropdowns (reuse from your Index)
        private void PopulateFilters(int? selectedBranch = 0, int? selectedMonth = null, int? selectedYear = null)
        {
            // Branch list (sync or async depending on your service; here assume sync result is already available)
            var branchList = _branchService.GetAllBranch().Result; // or await if you make this async
            var branchSelectList = branchList.Select(b => new SelectListItem
            {
                Value = b.Id.ToString(),
                Text = b.Name,
                Selected = (selectedBranch.HasValue && selectedBranch.Value == b.Id)
            }).ToList();
            branchSelectList.Insert(0, new SelectListItem { Value = "0", Text = "All Branch", Selected = (selectedBranch == 0) });

            ViewBag.BranchList = branchSelectList;

            var currentMonth = DateTime.Now.Month;
            var months = Enumerable.Range(1, 12).Select(m => new SelectListItem
            {
                Value = m.ToString(),
                Text = new DateTime(2000, m, 1).ToString("MMMM"),
                Selected = (selectedMonth.HasValue ? selectedMonth.Value == m : m == currentMonth)
            }).ToList();
            ViewBag.MonthList = months;

            int currentYear = DateTime.Now.Year;
            var yearList = Enumerable.Range(currentYear - 2, 6).Select(y => new SelectListItem
            {
                Value = y.ToString(),
                Text = y.ToString(),
                Selected = (selectedYear.HasValue ? selectedYear.Value == y : y == currentYear)
            }).ToList();
            ViewBag.YearList = yearList;

            ViewBag.SelectedBranchId = selectedBranch ?? 0;
        }

        public async Task<IActionResult> Index()
        {
            PopulateFilters(0, DateTime.Now.Month, DateTime.Now.Year);

            var model = await _excludeTaxService.GetAllExcludeTaxAsync(0, DateTime.Now.Month, DateTime.Now.Year);
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ShowData(int BranchId = 0, int Month = 0, int Year = 0)
        {
            // Ensure sensible defaults
            if (Month == 0) Month = DateTime.Now.Month;
            if (Year == 0) Year = DateTime.Now.Year;

            PopulateFilters(BranchId, Month, Year);
            var model = await _excludeTaxService.GetAllExcludeTaxAsync(BranchId, Month, Year);
            return View("Index", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StopExcludeTax(StopExcludeTaxViewModel model)
        {
            bool isCreated = await _excludeTaxService.StopExcludeTaxAsync(model);
            if (!isCreated)
            {
                TempData["ErrorMessage"] = "A excludeTax already exists for this SubscriptionId.";
                return RedirectToAction("INdex");
            }

            TempData["SuccessMessage"] = "Selected employees marked as excluded successfully.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StartIncludeTax(StopExcludeTaxViewModel model)
        {

            bool isCreated = await _excludeTaxService.StartIncludeTaxAsync(model);
            if (!isCreated)
            {
                TempData["ErrorMessage"] = "A excludeTax already exists for this SubscriptionId.";
                return RedirectToAction("INdex");
            }

            TempData["SuccessMessage"] = "Selected employees marked as excluded successfully.";
            return RedirectToAction("Index");
        }



        #region previousCode

        //public async Task<IActionResult> Index()
        //{
        //    var branchList = await _branchService.GetAllBranch();

        //    var branchSelectList = branchList.Select(b => new SelectListItem
        //    {
        //        Value = b.Id.ToString(),
        //        Text = b.Name
        //    }).ToList();

        //    // Add "All Branch" at the top with value 0
        //    branchSelectList.Insert(0, new SelectListItem
        //    {
        //        Value = "0",
        //        Text = "All Branch"
        //    });

        //    ViewBag.BranchList = branchSelectList;

        //    // Generate month list

        //    var currentMonth = DateTime.Now.Month;

        //    var months = Enumerable.Range(1, 12)
        //        .Select(m => new SelectListItem
        //        {
        //            Value = m.ToString(),
        //            Text = new DateTime(2000, m, 1).ToString("MMMM"),
        //            Selected = (m == currentMonth) // auto-select current month
        //        })
        //        .ToList();

        //    ViewBag.MonthList = months;



        //    // Generate year list
        //    int currentYear = DateTime.Now.Year;

        //    var yearList = Enumerable.Range(currentYear - 2, 6) // 2 previous + current + 3 next
        //        .Select(y => new SelectListItem
        //        {
        //            Value = y.ToString(),
        //            Text = y.ToString(),
        //            Selected = (y == currentYear)
        //        })
        //        .ToList();

        //    ViewBag.YearList = yearList;


        //    var excludeTaxs = await _excludeTaxService.GetAllExcludeTaxAsync(0);
        //    return View(excludeTaxs);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> StopExcludeTax(StopExcludeTaxViewModel model)
        //{
        //    if (model == null || model.StopEmployeeIds == null || !model.StopEmployeeIds.Any())
        //    {
        //        TempData["ErrorMessage"] = "Please select at least one employee to stop.";
        //        return RedirectToAction("Index");
        //    }

        //    var subscriptionId = _baseService.GetSubscriptionId();
        //    var companyId = await _baseService.GetCompanyId(subscriptionId);

        //    // compute dates if not supplied
        //    var fromDate = model.FromDate ?? new DateTime(model.Year, model.MonthIndex, 1);
        //    var toDate = model.ToDate ?? new DateTime(model.Year, model.MonthIndex, DateTime.DaysInMonth(model.Year, model.MonthIndex));
        //    var genDate = model.GenDate ?? DateTime.Now;

        //    var inserted = new List<int>();
        //    var skipped = new List<int>();
        //    var failed = new List<int>();

        //    foreach (var empId in model.StopEmployeeIds)
        //    {
        //        // check if already exists for this subscription + month + year
        //        bool exists = await _excludeTaxService.ExistsExcludeTaxAsync(empId.ToString(), model.MonthIndex.ToString(), model.Year.ToString(), subscriptionId);
        //        if (exists)
        //        {
        //            skipped.Add(empId);
        //            continue;
        //        }

        //        var excludeTax = new ExcludeTax
        //        {
        //            MonthIndex = model.MonthIndex,
        //            Month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(model.MonthIndex),
        //            Year = model.Year,
        //            FromDate = fromDate.ToString("yyyy-MM-dd"),
        //            ToDate = toDate.ToString("yyyy-MM-dd"),
        //            GenDate = genDate.ToString("yyyy-MM-dd HH:mm:ss"),
        //            EmployeeId = empId
        //        };

        //        var created = await _excludeTaxService.InsertExcludeTaxAsync(excludeTax);
        //        if (created) inserted.Add(empId);
        //        else failed.Add(empId);
        //    }

        //    var messages = new List<string>();
        //    if (inserted.Any()) messages.Add($"{inserted.Count} record(s) created.");
        //    if (skipped.Any()) messages.Add($"{skipped.Count} skipped (already exist).");
        //    if (failed.Any()) messages.Add($"{failed.Count} failed to insert.");

        //    TempData["SuccessMessage"] = string.Join(" ", messages);

        //    return RedirectToAction("Index");
        //}


        //[HttpPost]
        //public async Task<IActionResult> StopExcludeTax(ExcludeTax excludeTax)
        //{
        //        bool isCreated = await _excludeTaxService.InsertExcludeTaxAsync(excludeTax);
        //        if (!isCreated)
        //        {
        //            TempData["ErrorMessage"] = "A TaxtSlabSetup already exists for this SubscriptionId.";
        //            return RedirectToAction("INdex");
        //        }
        //        TempData["SuccessMessage"] = "TaxtSlabSetup Created Successfully";
        //    return RedirectToAction("Index");
        //}
        #endregion

    }
}
