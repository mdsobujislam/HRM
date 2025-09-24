using HRM.Models;

namespace HRM.Interfaces
{
    public interface IGratuityCalculateService
    {
        Task<List<ShowGratuity>> GetAllShowGratuityAsync(ShowGratuity showGratuity);
        Task<bool> InsertGratuityCalculateAsysnc(GratuityCalculate gratuityCalculate);
    }
}
