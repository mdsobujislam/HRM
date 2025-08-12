using System.ComponentModel.DataAnnotations.Schema;

namespace HRM.Models
{
    public class OffDays
    {
        public int Id { get; set; }
        //public string OffDay { get; set; }
        public string OffDay { get; set; }

        // Not mapped, used for form binding
        [NotMapped] // or just don't map in Dapper query
        public List<string> OffDayList
        {
            get => string.IsNullOrEmpty(OffDay)
                   ? new List<string>()
                   : OffDay.Split(',').ToList();
            set => OffDay = value != null ? string.Join(",", value) : "";
        }
        public int DesignationId { get; set; }
        public string Designation { get; set; }
        public int DepartmentId { get; set; }
        public string Department { get; set; }
        public int BranchId { get; set; }
        public string Branch { get; set; }
    }
}
