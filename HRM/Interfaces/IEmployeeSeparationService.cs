using HRM.Models;

namespace HRM.Interfaces
{
    public interface IEmployeeSeparationService
    {
        Task<bool> InsertEmployeeSeparationAsync(EmployeeSeparation employeeSeparation);
    }
}
