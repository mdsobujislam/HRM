namespace HRM.Models
{
    public class LoanInstallment
    {
        public int LoanId { get; set; }
        public int Installment_No { get; set; }
        public string DateOfInstallment { get; set; }
        public double Installment_Amount { get; set; }
        public string InstallmentStatus { get; set; }
        public string PaymentDate { get; set; }
        public string InstallmentDetails { get; set; }
        public string SalaryMonth { get; set; }
        public int SalaryYear { get; set; }
    }
}
