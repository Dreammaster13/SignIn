namespace SignIn.DatabaseAccess
{
    public class SignInUnitOfWork : ISignInUnitOfWork
    {
        public SignInUnitOfWork()
        {
    
        }
        public void Commit()
        {
            throw new System.NotImplementedException();
        }

        public IUsersRepository Users { get; }
    }
}