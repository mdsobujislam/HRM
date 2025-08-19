using HRM.Interfaces;
using HRM.Models;
using HRM.Services;
using iTextSharp.text;
using iTextSharp.text.pdf;
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

            var bonusCalculates = await _bonusCalculateService.GetAllAsync();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Insert(BonusCalculate bonusCalculate)
        {
            bool isCreated = await _bonusCalculateService.InsertBonusCalculate(bonusCalculate);
            TempData["SuccessMessage"] = isCreated ? "Bonus Calculate created successfully." : "Failed to create Bonus Calculate.";

            return RedirectToAction("Index");
        }
        public async Task<IActionResult> GetData()
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


            var bonusList = await _bonusCalculateService.GetAllAsync();
            if (bonusList == null || !bonusList.Any())
            {
                return View(new List<BonusCalculate>());
            }
            return View(bonusList);
        }
        public async Task<IActionResult> ShowData(BonusCalculate bonusCalculate)
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


            var bonusList = await _bonusCalculateService.GetAllDataShowAsync(bonusCalculate);

            // ✅ If no data, still return the View with empty list
            if (bonusList == null || !bonusList.Any())
            {
                return View(new List<BonusCalculate>());
            }

            return View(bonusList);
        }

        public async Task<IActionResult> ExportPdf(BonusCalculate bonusCalculate)
        {
            var bonusList = await _bonusCalculateService.GetAllDataShowAsync(bonusCalculate);

            // Always make sure list is not null
            var safeList = bonusList?.ToList() ?? new List<BonusCalculate>();

            using (var stream = new MemoryStream())
            {
                // Create PDF document
                Document pdfDoc = new Document(PageSize.A4, 20f, 20f, 20f, 20f);
                PdfWriter.GetInstance(pdfDoc, stream);
                pdfDoc.Open();

                // Add Title
                var titleFont = FontFactory.GetFont("Arial", 16, Font.BOLD);
                pdfDoc.Add(new Paragraph("Bonus Report", titleFont));
                pdfDoc.Add(new Paragraph("Generated on: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm")));
                pdfDoc.Add(new Paragraph("\n"));

                // Create Table (adjust column count based on your model)
                PdfPTable table = new PdfPTable(5); // example: 5 columns
                table.WidthPercentage = 100;

                // Add Headers
                table.AddCell("Bonus Type");
                table.AddCell("Employee");
                table.AddCell("Percentage");
                table.AddCell("BonusAmount");
                table.AddCell("Department");

                // Add Data Rows
                foreach (var item in safeList)
                {
                    table.AddCell(item.BonusType ?? "");
                    table.AddCell(item.Employee ?? "");
                    table.AddCell(item.Percentage.ToString() ?? "");
                    table.AddCell(item.BonusAmount.ToString() ?? "");
                    table.AddCell(item.Department.ToString() ?? "");
                }

                pdfDoc.Add(table);
                pdfDoc.Close();

                // Return File
                return File(stream.ToArray(), "application/pdf", "BonusReport.pdf");
            }
        }


        public async Task<IActionResult> Delete(int id)
        {
            bool isDeleted = await _bonusCalculateService.DeleteBonusCalculate(id);
            TempData["SuccessMessage"] = isDeleted ? "Bonus Calculate deleted successfully." : "Failed to delete Bonus Calculate.";
            return RedirectToAction("GetData");
        }
    }
}
