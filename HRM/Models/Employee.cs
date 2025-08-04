namespace HRM.Models
{
    public class Employee
    {
        public int EmpId { get; set; }
        public string EmployeeName { get; set; }
        public string FatherName { get; set; }
        public string MotherName { get; set; }
        public string SpouseName { get; set; }
        public string DOB { get; set; }
        public string Gender { get; set; }
        public string BloodGroup { get; set; }
        public string MaritalStatus { get; set; }
        public string TinNo { get; set; }
        public string NationalID { get; set; }
        public string Telephone { get; set; }
        public string MobileNo { get; set; }
        public string MailID { get; set; }
        public string PermanentAddress { get; set; }
        public string PresentAddress { get; set; }
        public string Branch { get; set; }
        public int BranchId { get; set; }
        public string Department { get; set; }
        public int DepartmentId { get; set; }
        public string Designation { get; set; }
        public int DesignationId { get; set; }
        public DateTime DateOfAppointment { get; set; }
        public string Religion { get; set; }
        public string Nationality { get; set; }
        public string Shift { get; set; }
        public string Version { get; set; }
        public string JobType { get; set; }
        public string UploadPhoto { get; set; }
        public int CompanyId { get; set; }
        public int SubscriptionId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
    }
}