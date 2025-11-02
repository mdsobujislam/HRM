using HRM.Models;

namespace HRM.Interfaces
{
    public interface IDepartmentService
    {
        Task<List<Department>> GetAllDepartment();
        Task<bool> InsertDepartment(Department department);
        Task<bool> UpdateDepartment(Department department);
        Task<bool> DeleteDepartment(int departmentId);

        Task<IEnumerable<Department>> GetDepartmentsByBranchId(int branchId);
    }
}
