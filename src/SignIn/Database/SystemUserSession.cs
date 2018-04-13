using System;
using Starcounter;
using Starcounter.Authorization.Authentication;

namespace SignIn
{
    [Database]
    public class SystemUserSession : IUserSession<SystemUser>
    {
        public string SessionId { get; set; }
        public SystemUser User { get; set; }
        public DateTime ExpiresAt { get; set; }
        public string PrincipalSerialized { get; set; }

    }
}
