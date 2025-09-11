using System.Numerics;

namespace HRM.Models
{
    //public class Salary
    //{
    //    public int Id { get; set; }
    //    public int MonthIndex { get; set; }
    //    public int SalaryHeadsId { get; set; }
    //    public string SalaryHeads { get; set; }
    //    public string Month { get; set; }
    //    public BigInteger Year { get; set; }
    //    public DateTime FromDate { get; set; }
    //    public DateTime ToDate { get; set; }
    //    public DateTime GenDate { get; set; }
    //    public int EmployeeId { get; set; }
    //    public int BranchId { get; set; }
    //    public string Branch { get; set; }
    //    public int Sl { get; set; }
    //    public string Parameter { get; set; }
    //    public double Value { get; set; }
    //    public double FinalAmount { get; set; }
    //    public int TxId { get; set; }
    //    public int RefTxId { get; set; }
    //    public int DesignationId { get; set; }
    //    public string Designation { get; set; }
    //    public int DepartmentId { get; set; }
    //    public string Department { get; set; }
    //}
    public class Salary
    {
        public int Id { get; set; }
        public int SalaryHeadsId { get; set; }
        public string SalaryHeads { get; set; }
        public int BranchId { get; set; }
        public int DepartmentId { get; set; }
        public int DesignationId { get; set; }
        public int EmployeeId { get; set; }
        public string Branch { get; set; }
        public int Sl { get; set; }
        public string Parameter { get; set; }
        public double Value { get; set; }
        public int MonthIndex { get; set; }
        public string Month { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public DateTime GenDate { get; set; }
        public double FinalAmount { get; set; }
        public double AccountsPayable { get; set; } // Add this property

        public List<SalaryItem> SalaryItems { get; set; } = new List<SalaryItem>();


    }
}
