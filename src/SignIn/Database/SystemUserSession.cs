using System;
using Starcounter;
using Starcounter.Authorization.Model;

namespace SignIn
{
    [Database]
    public class SystemUserSession : IScUserAuthenticationTicket<SystemUser>
    {
        public string SessionId { get; set; }
        public SystemUser User { get; set; }
        public DateTime ExpiresAt { get; set; }
        public string PersistenceToken { get; set; }
        public string PrincipalSerialized { get; set; }

    }
}
