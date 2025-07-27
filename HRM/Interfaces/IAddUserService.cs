using HRM.Models;

namespace HRM.Interfaces
{
    public interface IAddUserService
    {
        Task<List<AddUser>> GetAllAddUser();
        Task<bool> InsertAddUser(AddUser addUser);
        Task<bool> UpdateAddUser(AddUser addUser);
        Task<bool> DeleteAddUser(int addUserId);
    }
}
