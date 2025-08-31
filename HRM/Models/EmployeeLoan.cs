namespace HRM.Models
{
    public class EmployeeLoan
    {
        public int Id { get; set; }
        public List<IFormFile> PdfFiles { get; set; }  // for multiple files
        public string PdfPath { get; set; }
        public int EmployeeId { get; set; }
        public string DateOfLoan { get; set; }
        public string LoanAgainst { get; set; }
        public double LoanPercentage { get; set; }
        public double LoanAmount { get; set; }
        public double Interest { get; set; }
        public double InterestAmount { get; set; }
        public double Term { get; set; }
        public double PerMonthInterest { get; set; }
        public double EmiPrinciple { get; set; }
        public double Emi { get; set; }
        public string ApplicationId { get; set; }
    }
}
