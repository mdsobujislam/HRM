using HRM.Models;

namespace HRM.Interfaces
{
    public interface IUserVMService
    {
        Task<List<Uservm>> GetAllUserAsync();
    }
}
