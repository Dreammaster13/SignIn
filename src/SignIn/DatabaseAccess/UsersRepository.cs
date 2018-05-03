using SignIn.Database;
using Starcounter.Linq;

namespace SignIn.DatabaseAccess
{
    public class UsersRepository : IUsersRepository
    {
        public SystemUser FindByUsername(string username)
        {
            return DbLinq
                .Objects<SystemUser>()
                .FirstOrDefault(user => user.Username == username);
        }

        public SystemUser Create()
        {
            return new SystemUser();
        }
    }
}