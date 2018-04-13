using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SignIn.Api;
using Starcounter.Authentication;
using Starcounter.Startup;

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

            DefaultStarcounterBootstrapper.Start(new Startup());
        }
    }

    public class Startup : IStartup
    {
        public static ISignInManager<SystemUserSession, SystemUser> SignInManager;

        public IServiceCollection ConfigureServices(IServiceCollection services)
        {
            return services.AddSignInManager<SystemUserSession, SystemUser>()
                .AddLogging(logging => logging.AddConsole());
        }

        public void Configure(IApplicationBuilder applicationBuilder)
        {
            SignInManager = applicationBuilder.ApplicationServices
                .GetRequiredService<ISignInManager<SystemUserSession, SystemUser>>();
        }
    }
}