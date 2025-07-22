using HRM.Interfaces;
using HRM.Models;
using Microsoft.AspNetCore.Mvc;

namespace S_EDex365.Controllers
{
    public class RoleController : Controller
    {
        private readonly IRoleService _roleService;
        public RoleController(IRoleService service)
        {
            _roleService = service ??
                throw new ArgumentNullException(nameof(service));
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetAllRoles()
        {
            var roleList = await _roleService.GetAllRolesAsync();
            return Json(new { Data = roleList });
        }

        public async Task<IActionResult> AddRole()
        {
            return PartialView("_AddRole");
        }

        [HttpPost]
        public async Task<IActionResult> InsertRole(Role model)
        {
            try
            {
                var success = await _roleService.InsertRoleAsync(model);
                return Json(new { result = success });
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public async Task<IActionResult> UpdateRole(Guid serviceId)
        {
            var serviceAndPart = await _roleService.GetRoleByIdAsync(serviceId);
            return PartialView("_UpdateRole", serviceAndPart);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateRolePartail(Role model)
        {
            try
            {
                var success = await _roleService.UpdateRoleAsync(model);
                return Json(new { result = success });
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        [HttpPost]
        public async Task<IActionResult> DeleteRole(Guid roleId)
        {
            try
            {
                var result = await _roleService.DeleteRoleAsync(roleId);
                return Json(new { result });
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<IActionResult> RolePermission(Guid roleId)
        {
            var role = await _roleService.GetRoleByIdAsync(roleId);
            PermissionDto permission = new PermissionDto();
            permission.RoleId = roleId;
            permission.RoleName = role.Name;
            permission.MenuList = await _roleService.GetAllMenusAsync(roleId);
            return PartialView("_RolePermission", permission);
        }
        [HttpPost]
        public async Task<IActionResult> AddPermissionToTole(RolePermission permission)
        {
            var result=await _roleService.AddRolePermissionAsync(permission);
            return Json(new {success=result});
        }
    }
}
