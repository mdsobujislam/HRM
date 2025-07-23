namespace HRM.Models
{
    public class Companies
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string TagLine { get; set; }
        public string VatRegistrationNo { get; set; }
        public string TinNo { get; set; }
        public string WebsiteLink { get; set; }
        public string Email { get; set; }
        public string ContactNumber { get; set; }
        public string Address { get; set; }
        public string Remarks { get; set; }
        public IFormFile LogoFile { get; set; }
        public string ExistingLogoPath { get; set; }
    }
}
