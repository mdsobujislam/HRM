using HRM.Models;

namespace HRM.Interfaces
{
    public interface ISeparationReasonsService
    {
        Task<List<SeparationReasons>> GetAllSeparationReasonsAsync();
        Task<bool> CreateSeparationReasonAsync(SeparationReasons separationReason);
        Task<bool> UpdateSeparationReasonAsync(SeparationReasons separationReason);
        Task<bool> DeleteSeparationReasonAsync(int id);
    }
}
