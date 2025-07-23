namespace HRM.Models
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool Status { get; set; }
        public string StatusName { get; set; }=string.Empty;
    }
}
