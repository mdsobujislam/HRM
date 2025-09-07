using HRM.Models;

namespace HRM.Interfaces
{
    public interface IShowInstalmentListService
    {
        Task<List<EmployeeLoanDto>> GetAllShowInstalmentListAsync();
    }
}
