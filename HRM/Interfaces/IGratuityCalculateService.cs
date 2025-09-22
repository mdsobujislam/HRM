using HRM.Models;

namespace HRM.Interfaces
{
    public interface IGratuityCalculateService
    {
        Task<List<ShowGratuity>> GetAllShowGratuityAsync(int branchId,string date);
        Task<bool> InsertGratuityCalculateAsysnc(GratuityCalculate gratuityCalculate);
    }
}
