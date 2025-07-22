namespace HRM.Models
{
    public class PermissionDto
    {
        public Guid RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public List<Menu> MenuList { get; set; }=new List<Menu>();
    }
}
