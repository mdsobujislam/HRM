namespace HRM.Models
{
    public class LoanInstallmentDto
    {
        public int LoanId { get; set; }
        public int InstallmentNo { get; set; }
        public DateTime DateOfInstallment { get; set; }
        public decimal InstallmentAmount { get; set; }
        public string Status { get; set; }
        public DateTime? PaymentDate { get; set; }
    }
}
