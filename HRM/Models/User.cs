using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string MobileNo { get; set; }
        public bool Status { get; set; }
        public string StatusName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime Dob { get; set; }
        public string OTP { get; set; }
        public string School { get; set; }
        public string Class { get; set; }
        public DateTime GetDateby { get; set; }
        public DateTime Updateby { get; set; }
        public string RName { get; set; }  
        public string Roles { get; set; } = string.Empty;
        public List<string> Role { get; set; } = new List<string>();
        public List<Role> RoleList { get; set; } = new List<Role>();
    }
}
