namespace HRM.Models
{
    public class PermissionDto
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public List<Menu> MenuList { get; set; }=new List<Menu>();
    }
}
