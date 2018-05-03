using System.Collections.Generic;
using System.Linq;
using SignIn.Database;
using Starcounter;
using Starcounter.Authorization.Model;
using Starcounter.Linq;

namespace SignIn
{
    [Database]
    public class SystemUser : IUserWithGroups
    {
        public string Username { get; set; }
        public string Password { get; set; }
        // this is not used, salt is included in Password
        public string PasswordSalt { get; set; }
        public string Email { get; set; }

//        public IEnumerable<IClaimDb> AssociatedClaims => Enumerable.Empty<IClaimDb>();
        public IEnumerable<IClaimDb> AssociatedClaims => DbLinq.Objects<UserClaimRelation>()
            .Where(relation => relation.Subject == this)
            .ToList()
            .Select(relation => relation.Object);

        public IEnumerable<IUserGroup> Groups => Enumerable.Empty<IUserGroup>();

        public override string ToString()
        {
            return $"SystemUser {Username}";
        }
    }
}