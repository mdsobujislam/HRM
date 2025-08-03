using HRM.Interfaces;
using HRM.Models;
using Microsoft.AspNetCore.Mvc;

namespace HRM.Controllers
{
    public class DutySlotController : Controller
    {
        private readonly IDutySlotService _service;
        public DutySlotController(IDutySlotService service)
        {
            _service = service;
        }
        public async Task<IActionResult> Index()
        {
            var dutySlots = await _service.GetAllDutySlot();
            return View(dutySlots);
        }
        [HttpPost]
        public async Task<IActionResult> CreateOrUpdateDepartment(DutySlot dutySlot)
        {

            if (dutySlot.Id == 0)
            {
                bool isCreated = await _service.InsertDutySlot(dutySlot);
                TempData["SuccessMessage"] = isCreated ? "Duty slot created successfully." : "Failed to create duty slot.";
            }
            else
            {
                bool isUpdated = await _service.UpdateDutySlot(dutySlot);
                if (!isUpdated)
                {
                    TempData["ErrorMessage"] = "Update failed. Name may already exist.";
                    return RedirectToAction("Index");
                }
                TempData["SuccessMessage"] = "Duty slot updated successfully.";
            }

            return RedirectToAction("Index");
        }
    }
}
