using HRM.Models;

namespace HRM.Interfaces
{
    public interface IDesignationService
    {
        Task<List<Designation>> GetAllDesignation();
        Task<bool> InsertDesignation(Designation designation);
        Task<bool> UpdateDesignation(Designation designation);
        Task<bool> DeleteDesignation(int id);
    }
}
