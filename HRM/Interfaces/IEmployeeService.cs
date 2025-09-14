using HRM.Models;

namespace HRM.Interfaces
{
    public interface IEmployeeService
    {
        Task<List<Employee>> GetAllEmployee();
        Task<bool> InsertEmployee(Employee employee, IFormFile PhotoFile);
        Task<bool> UpdateEmployee(Employee employee, IFormFile PhotoFile);
        Task<bool> DeleteEmployee(int empId);
        Task<Employee> GetEmployeeById(int empId);   

    }
}
