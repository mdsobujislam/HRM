namespace HRM.Models.ViewModels
{
    public class UserListViewModel
    {
        public AddUser NewUser { get; set; } = new AddUser();
        public List<AddUser> UserList { get; set; } = new List<AddUser>();
    }
}
