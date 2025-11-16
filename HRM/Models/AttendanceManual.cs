namespace HRM.Models
{
    public class AttendanceManual
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public DateTime LoginDate { get; set; }
        public DateTime LogoutDate { get; set; }
        public int TxNo { get; set; }
    }
}
