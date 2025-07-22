namespace HRM.Models
{
    public class Menu
    {
        public int Id { get; set; }
        public string Name { get; set; }=string.Empty;
        public string Controller { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string PermissionName { get;set; } = string.Empty;
        public bool Status { get; set; }
    }
}
