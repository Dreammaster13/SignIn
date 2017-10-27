using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Starcounter;

namespace SignIn
{
    [Database]
    public class ResetPassword
    {
        public SystemUser User { get; set; }
        public DateTime Expire { get; set; }
        public string Token { get; set; }
    }
}
