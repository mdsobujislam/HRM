using HRM.Models;

namespace HRM.Repository
{
    public interface IRoleService
    {
        Task<List<Role>> GetAllRolesAsync();
        Task<Role> GetRoleByIdAsync(int roleId);
        Task<Role> GetRoleByNameAsync(string roleName);
        Task<bool> InsertRoleAsync(Role role);
        Task<bool> UpdateRoleAsync(Role role);
        Task<bool> DeleteRoleAsync(int roleId);
        Task<List<Menu>> GetAllMenusAsync(int roleId);
        Task<bool> AddRolePermissionAsync(RolePermission role);
        Task<List<string>> RolePermissionAsync(string roleId);
    }
}
