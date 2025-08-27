namespace HRM.Models
{
    public class EmployeeLoan
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public DateTime DateOfLoan { get; set; }
        public string LoanAgainst { get; set; }
        public double LoanPercentage { get; set; }
        public double LoanAmount { get; set; }
        public double Interest { get; set; }
        public double InterestAmount { get; set; }
        public double Term { get; set; }
        public double PerMonthInterest { get; set; }
        public double EmiPrinciple { get; set; }
        public double Emi { get; set; }
    }
}
