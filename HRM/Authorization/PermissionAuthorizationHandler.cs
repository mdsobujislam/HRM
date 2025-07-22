using Microsoft.AspNetCore.Authorization;
using HRM.Interfaces;
using System.Security.Claims;

namespace HRM.Authorization
{
    public class PermissionAuthorizationHandler: AuthorizationHandler<PermissionRequirement>
    {
        private readonly IRoleService _roleService;
        private readonly IUserService _userService;
        //private readonly IUserService _userService;
        public PermissionAuthorizationHandler(IRoleService roleService, IUserService studentService)
        {
            _roleService = roleService ??
                throw new ArgumentNullException(nameof(roleService));
            _userService = studentService ?? 
                throw new ArgumentNullException(nameof(_userService));

        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != null)
            {
                var roleList = _userService.GetUserRolesAsync(userId ?? "");
                var roleName = new List<string>();
                foreach (var item in roleList.Result)
                {
                    Guid result = Guid.Parse(item);
                    var name = _roleService.GetRoleByIdAsync(result);
                    roleName.Add(name.Result.Name);
                }
                var rolePermissionList = new List<string>();
                if (roleName.Contains("MD Shoeb"))
                {
                    context.Succeed(requirement);
                    return Task.CompletedTask;
                }

                foreach (var role in roleList.Result)
                {
                    var permissionList = _roleService.RolePermissionAsync(role);
                    rolePermissionList.AddRange(permissionList.Result);
                }

                if (rolePermissionList.Count == 0)
                {
                    return Task.CompletedTask;
                }

                if (rolePermissionList.Contains(requirement.Permission))
                {
                    context.Succeed(requirement);
                }
            }
            return Task.CompletedTask;
        }
    }
}
