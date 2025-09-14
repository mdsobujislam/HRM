namespace HRM.Models
{
    public class TaxtSlabSetup
{
    public int Id { get; set; }
    public string Date { get; set; }
    public double IncomeSlab { get; set; }
    public double TaxPer { get; set; }

    public int AdditionalId { get; set; }   // FK id
    public string AdditionalInfoName { get; set; }  // joined value

    public int BranchId { get; set; }       // FK id
    public string BranchName { get; set; }  // joined value

    public int DepartmentId { get; set; }   // FK id
    public string DepartmentName { get; set; } // joined value
}

}
