namespace HRM.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string MobileNo { get; set; }=string.Empty;
        public bool Status { get; set; }
        public string StatusName { get;set; }=string.Empty;
        public string Email { get; set; }=string.Empty;
        public string Password { get; set; } = string.Empty;
        public int SubscriptionId { get; set; }
        public string Roles { get; set;} = string.Empty;
        public List<string> Role { get; set;} = new List<string>();
        public List<Role> RoleList { get; set;} = new List<Role>();
    }
}
