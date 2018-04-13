using Starcounter;
using Starcounter.Authorization.Authentication;

namespace SignIn
{
    [Database]
    public class ClaimDb:IClaimDb
    {
        public string Bytes { get; set; }
        public string ClaimSerialized => Bytes;
    }
}