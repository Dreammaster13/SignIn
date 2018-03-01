using SignIn.Api;

namespace SignIn
{
    class Program
    {
        static void Main()
        {

            new Middleware().Register();
            new CommitHooks().Register();
            new MainHandlers().Register();
            new PartialHandlers().Register();
            new BlenderMapping().Register();

            new Schedular().SetupSessionCleanupTimer();
        }
    }
}