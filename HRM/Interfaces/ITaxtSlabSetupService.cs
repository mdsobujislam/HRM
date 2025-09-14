using HRM.Models;

namespace HRM.Interfaces
{
    public interface ITaxtSlabSetupService
    {
        Task<List<TaxtSlabSetup>> GetAllTaxtSlabSetupAsync();
        Task<bool> InsertTaxtSlabSetupAsync(TaxtSlabSetup taxtSlabSetup);
        Task<bool> UpdateTaxtSlabSetupAsync(TaxtSlabSetup taxtSlabSetup);
        Task<bool> DeleteTaxtSlabSetupAsync(int id);
    }
}
