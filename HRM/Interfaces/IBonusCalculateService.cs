using HRM.Models;

namespace HRM.Interfaces
{
    public interface IBonusCalculateService
    {
        Task<List<BonusCalculate>> GetAllAsync();
        Task<List<BonusCalculate>> GetAllDataShowAsync(BonusCalculate bonusCalculate);
        Task<bool> InsertBonusCalculate(BonusCalculate bonusCalculate);
        Task<bool> UpdateBonusCalculate(BonusCalculate bonusCalculate);
        Task<bool> DeleteBonusCalculate(int id);
    }
}
