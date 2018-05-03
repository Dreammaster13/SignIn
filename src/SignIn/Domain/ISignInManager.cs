namespace SignIn.Domain
{
    public interface ISignInManager
    {
        SignInResult SignIn(string username, string password);
    }
}