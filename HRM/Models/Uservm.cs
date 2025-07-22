using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM.Models
{
    public class Uservm
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string MobileNo { get; set; }
        public bool Status { get; set; }
        public string StatusName { get; set; }
        public string Email { get; set; }
        public string School { get; set; }
        public DateTime Dob { get; set; }
        public string Class { get; set; }
        public string Roles { get; set; } = string.Empty;
        public List<string> Role { get; set; } = new List<string>();
        public List<Role> RoleList { get; set; } = new List<Role>();
    }
}
