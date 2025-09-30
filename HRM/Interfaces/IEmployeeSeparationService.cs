using HRM.Models;
using Microsoft.AspNetCore.Mvc;

namespace HRM.Interfaces
{
    public interface IEmployeeSeparationService
    {
        Task<bool> InsertEmployeeSeparationAsync(EmployeeSeparation employeeSeparation);
        Task<IEnumerable<object>> SearchEmployees(string term);
    }
}
