using HRM.Models;
using HRM.Models.ViewModels;

namespace HRM.Interfaces
{
    public interface IExcludeTaxService
    {
        Task<List<EmployeeTaxStatusViewModel>> GetAllExcludeTaxAsync(int branchId, int monthIndex, int year);
        Task<bool> StopExcludeTaxAsync(StopExcludeTaxViewModel model);
        Task<bool> StartIncludeTaxAsync(StopExcludeTaxViewModel model);

    }
}
