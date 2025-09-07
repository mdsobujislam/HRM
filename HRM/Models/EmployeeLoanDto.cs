namespace HRM.Models
{
    public class EmployeeLoanDto
    {
        public int LoanId { get; set; }
        public int ApplicationId { get; set; }
        public DateTime DateOfLoan { get; set; }
        public decimal LoanAmount { get; set; }
        public decimal Interest { get; set; }
        public decimal InterestAmount { get; set; }
        public int Term { get; set; }
        public decimal PerMonthInterest { get; set; }
        public decimal EmiPrinciple { get; set; }
        public decimal Emi { get; set; }

        public List<LoanInstallmentDto> Installments { get; set; } = new();
        public List<string> Documents { get; set; } = new();
    }
}
