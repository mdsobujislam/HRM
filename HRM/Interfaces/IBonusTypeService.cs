using HRM.Models;

namespace HRM.Interfaces
{
    public interface IBonusTypeService
    {
        Task<List<BonusType>> GetAllBonusType();
        Task<bool> InsertBonusType(BonusType bonusType);
        Task<bool> UpdateBonusType(BonusType bonusType);
        Task<bool> DeleteBonusTypeA(int id);
    }
}
