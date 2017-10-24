using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignIn
{
    public class SystemUserTokenKey : Something
    {
        public string Token { get; set; }
        public SystemUser User { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastUsed { get; set; }

        /// <summary>
        /// An absolute date in UTC when the token is expired and can be no longer used
        /// </summary>
        public DateTime Expires { get; set; }

        public bool IsPersistent { get; set; }
    }
}
