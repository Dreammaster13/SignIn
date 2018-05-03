using Starcounter;
using Starcounter.Authorization.Model;

namespace SignIn
{
    [Database]
    public class ClaimDb: IClaimDb
    {
        public string Type { get; set; }
        public string Value { get; set; }
        public string ValueType { get; set; }
        public string Issuer { get; set; }
        public string OriginalIssuer { get; set; }
        public string PropertiesSerialized { get; set; }
    }
}