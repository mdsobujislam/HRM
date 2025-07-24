using HRM.Models;

namespace HRM.Interfaces
{
    public interface IBranchService
    {
        Task<List<Branch>> GetAllBranch();
        Task<Branch> GetAllBranchById(int branchId);
        Task<bool> InsertBranch(Branch branch);
        Task<bool> UpdateBranch(Branch branch);
        Task<bool> DeleteBranch(int branchId);
    }
}
