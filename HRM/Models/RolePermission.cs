namespace HRM.Models
{
    public class RolePermission
    {
        public string RoleId { get; set; } = string.Empty;
        public List<int> Menus { get; set; }=new List<int>();
    }
}
