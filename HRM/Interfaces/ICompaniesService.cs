using HRM.Models;

namespace HRM.Interfaces
{
    public interface ICompaniesService
    {
        Task<List<Companies>> GetAllCompanies();
        Task<bool> InsertCompanies(Companies companies);
        Task<bool> UpdateCompanies(Companies companies);
    }
}
