using HRM.Models;

namespace HRM.Interfaces
{
    public interface IPFInterestService
    {
        Task<List<PFInterest>> GetAllPFInterestsAsync();
        Task<bool> InsertPFInterestAsync(PFInterest pFInterest);
        Task<bool> UpdatePFInterestAsync(PFInterest pFInterest);
        Task<bool> DeletePFInterestAsync(int id);
    }
}
