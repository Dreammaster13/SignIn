namespace SignIn.DatabaseAccess
{
    public interface ISignInUnitOfWork
    {
        void Commit();

        IUsersRepository Users { get; }
    }
}