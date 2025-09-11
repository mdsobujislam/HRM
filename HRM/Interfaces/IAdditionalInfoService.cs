using HRM.Models;

namespace HRM.Interfaces
{
    public interface IAdditionalInfoService
    {
        Task<AdditionalInfo?> InsertAdditionalInfo(AdditionalInfo additionalInfo);
        Task<IEnumerable<AdditionalInfo>> GetAllAsync();
        Task<int?> GetByNameAsync(string name);
    }
}
