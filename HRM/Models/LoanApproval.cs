namespace HRM.Models
{
    public class LoanApproval
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string AppliDate { get; set; }
        public string Branch { get; set; }
        public string Designation { get; set; }
        public string Department { get; set; }
        public double AmountLoan { get; set; }
        public double BasicSalary { get; set; }
        public string Grade { get; set; }
        public string Duration { get; set; }
        public double RecommendedAmount { get; set; }
        public string RecommendDate { get; set; }
        public double PreviousLoan { get; set; }
        public double Repaid { get; set; }
        public string Hr_Rem { get; set; }
        public double LoanApproved { get; set; }
        public string LoanAppDate { get; set; }
        public int Term { get; set; }
        public double interest { get; set; }
        public string AppStatus { get; set; }
        public string LoanIssued { get; set; }
        public int LoanId { get; set; }
        public string Remarks { get; set; }

        //====
        public string ApplicationId { get; set; }
        public string DateOfLoan { get; set; }
        public List<IFormFile> PdfFiles { get; set; }
        public string PdfPath { get; set; }
        public string LoanAgainst { get; set; }
        public double LoanPercentage { get; set; }
        public double LoanAmount { get; set; }
        public double Interest { get; set; }
        public double InterestAmount { get; set; }
        public double EmiInterest { get; set; }
        public double EmiPrinciple { get; set; }
        public double NetEmi { get; set; }
    }
}
