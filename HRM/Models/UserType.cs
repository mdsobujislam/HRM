using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM.Models
{
    public class UserType
    {
        public Guid Id { get; set; }
        public string UserTypes { get; set; }
        public bool Status { get; set; }
        public string StatusName { get; set; }
    }
}
