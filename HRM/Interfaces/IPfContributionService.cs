using HRM.Models;

namespace HRM.Interfaces
{
    public interface IPfContributionService
    {
        Task<List<PfContribution>> GetAllPfContributionsAsync();
        Task<bool> InsertPfContributionasync(PfContribution pfContribution);
        Task<bool> UpdatePfContributionAsync(PfContribution pfContribution);
        Task<bool> DeletePfContributionAsync(int id);
    }
}
