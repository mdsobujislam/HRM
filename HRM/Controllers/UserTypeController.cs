using HRM.Interfaces;
using HRM.Models;
using Microsoft.AspNetCore.Mvc;

namespace S_EDex365.Controllers
{
    public class UserTypeController : Controller
    {
        private readonly IUserTypeService _userTypeService;
        public UserTypeController(IUserTypeService userTypeService)
        {
            _userTypeService = userTypeService ?? throw new ArgumentNullException(nameof(userTypeService));
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> GetAllUserType()
        {
            var usertypeList = await _userTypeService.GetAllUserTypeAsync();
            return Json(new { Data = usertypeList });
        }
        public async Task<IActionResult> AddUsertype()
        {
            return PartialView("_AddUserType");
        }
        [HttpPost]
        public async Task<IActionResult> InsertUserType(UserType userType)
        {
            try
            {
                var success = await _userTypeService.InsertUserTypeAsync(userType);
                return Json(new { result = success });
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public async Task<IActionResult> UpdateUpserType(Guid serviceId)
        {
            var serviceAndPart = await _userTypeService.GetUserTypeByIdAsync(serviceId);
            return PartialView("_UpdateUserType", serviceAndPart);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateUserTypePartail(UserType model)
        {
            try
            {
                var success = await _userTypeService.UpdateUserTypeAsync(model);
                return Json(new { result = success });
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        [HttpPost]
        public async Task<IActionResult> DeleteUserType(Guid userTypeId)
        {
            try
            {
                var result = await _userTypeService.DeleteUserTypeAsync(userTypeId);
                return Json(new { result });
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
