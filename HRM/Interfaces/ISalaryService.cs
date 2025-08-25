using HRM.Models;

public interface ISalaryService
{
    Task<List<Salary>> GetAllSalary();
    Task<bool> InsertSalary(Salary salary);
    Task<bool> InsertSalary(List<Salary> salaries);
    Task<bool> UpdateSalary(Salary salary);
    Task<bool> DeleteSalary(int id);
    Task<List<Employee>> GetEmployeesByBranch(int branchId);

    Task<List<Employee>> GetEmployeesByCriteria(int? branchId = null, int? departmentId = null,
        int? designationId = null, int? employeeId = null);
    Task<List<SalaryHeads>> GetActiveSalaryHeads();
    (DateTime fromDate, DateTime toDate) CalculateMonthDates(int monthIndex, int year);

    Task<int> GetBranchIdFromDepartment(int departmentId);
    Task<int> GetBranchIdFromDesignation(int designationId);
    Task<int> GetBranchIdFromEmployee(int employeeId);
    Task<List<SalaryHeads>> GetSalaryHeadsByBranch(int branchId);

}
