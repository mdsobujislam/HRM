namespace HRM.Models
{
    public class PayInstallment
    {
        public int installmentFrom { get; set; }
        public int installmentTo { get; set; }
        public double netAmount { get; set; }
        public string dateOfPayment { get; set; }
        public int LoanId { get; set; }
        public string EmployeeId { get; set; }
    }
}
