using HRM.Models;

namespace HRM.Interfaces
{
    public interface IRoleService
    {
        Task<List<Role>> GetAllRolesAsync();
        Task<Role> GetRoleByIdAsync(Guid roleId);
        Task<Role> GetRoleByNameAsync(string roleName);
        Task<bool> InsertRoleAsync(Role role);
        Task<bool> UpdateRoleAsync(Role role);
        Task<bool> DeleteRoleAsync(Guid roleId);
        Task<List<Menu>> GetAllMenusAsync(Guid roleId);
        Task<bool> AddRolePermissionAsync(RolePermission role);
        Task<List<string>> RolePermissionAsync(string roleId);
    }
}
