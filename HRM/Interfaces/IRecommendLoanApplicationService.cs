using HRM.Models;

namespace HRM.Interfaces
{
    public interface IRecommendLoanApplicationService
    {
        Task<List<RecommendLoanApplication>> GetAllRecommendLoanApplication();
        Task<bool> UpdateRecommendLoanApplication(RecommendLoanApplication recommendLoanApplication);
        Task<List<RecommendLoanApplication>> SearchData(RecommendLoanApplication recommendLoanApplication);
    }
}
