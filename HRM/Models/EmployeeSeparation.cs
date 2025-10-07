namespace HRM.Models
{
    public class EmployeeSeparation
    {
        public int Id { get; set; }
        public string EmployeeId { get; set; }
        public int SeparationReasonsId { get; set; }
        public string SeparationReasons { get; set; }
        public string SeparationDate { get; set; }
        public string RequestDate { get; set; }
        public string Remarks { get; set; }
        public List<IFormFile> PdfFiles { get; set; }
        public string PdfPath { get; set; }
    }
}
