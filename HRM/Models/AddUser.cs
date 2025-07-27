namespace HRM.Models
{
    public class AddUser
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string MobileNo { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int BranchId { get; set; }
        public bool Status { get; set; }
    }
}
