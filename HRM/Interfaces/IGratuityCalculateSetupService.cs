using HRM.Models;

namespace HRM.Interfaces
{
    public interface IGratuityCalculateSetupService
    {
        Task<List<GratuityCalculateSetup>> GetAllGratuityCalculateSetupAsync();
        Task<bool> InsertGratuityCalculateSetupAsync(GratuityCalculateSetup gratuityCalculateSetup);
        Task<bool> UpdateGratuityCalculateSetupAsync(GratuityCalculateSetup gratuityCalculateSetup);
        Task<bool> DeleteGratuityCalculateSetupAsync(int id);
    }
}
