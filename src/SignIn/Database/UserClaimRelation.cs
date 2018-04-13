using Starcounter;

namespace SignIn
{
    [Database]
    public class UserClaimRelation
    {
        public SystemUser Subject { get; set; }
        public ClaimDb Object { get; set; }
    }
}