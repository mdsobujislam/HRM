namespace HRM.Models
{
    public class LoanApproval
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string Employee { get; set; }
        public double AmountLoan { get; set; }
        public string Remarks { get; set; }
        public string Duration { get; set; }
    }
}
