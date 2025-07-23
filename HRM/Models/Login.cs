using System.ComponentModel.DataAnnotations;

namespace HRM.Models
{
    public class Login
    {
        [EmailAddress]
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Password { get;set; } =string.Empty;  
    }
}
