using SignIn.Database;

namespace SignIn.DatabaseAccess
{
    public interface IUsersRepository
    {
        SystemUser FindByUsername(string username);
        SystemUser Create();
    }
}