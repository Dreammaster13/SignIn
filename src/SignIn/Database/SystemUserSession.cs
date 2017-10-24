using System;
using Starcounter;

namespace SignIn
{
    public class SystemUserSession
    {
        public string SessionId { get; set; }
        public SystemUser User { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
