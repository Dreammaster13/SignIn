using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using SignIn.DatabaseAccess;
using SignIn.Domain;
using SignIn.ViewModels;
using Starcounter;
using Starcounter.Authorization.Authentication;
using Starcounter.Authorization.Model;
using Starcounter.Authorization.SignIn;
using Starcounter.Authorization.UserManagement;
using Starcounter.Startup.Abstractions;
using Starcounter.Startup.Routing;
using Starcounter.Startup.Routing.Activation;

namespace SignIn
{
    public class Startup : IStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services
                    .AddSignInManager<SystemUserSession, SystemUser>()
                .AddRouter()
                .AddTransient<IUsersRepository, UsersRepository>()
                .AddTransient<IScAuthenticationTicketRepository<SystemUserSession>, ScAuthenticationTicketRepository<SystemUserSession>>()
                .AddTransient<ISignInManager, SignInManager>()
                .AddTransient<ITransactionControl, TransactionControl>()
                .AddTransient<IPasswordHasher<SystemUser>, PasswordHasher<SystemUser>>()
                .AddTransient<AdminCreator>()
                .AddUserManagement<SystemUser, ManageUserViewModel>()
                ;
        }

        public void Configure(IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.ApplicationServices.GetRouter()
                .RegisterAllFromCurrentAssembly();

            RegisterUri<AdminCreator>(applicationBuilder.ApplicationServices, "/SignIn/CreateAdmin",
                adminCreator => {
                    Db.Transact(adminCreator.CreateAdmin);
                    return "Admin created. Username: 'admin', password: 'admin'";
                });

            Blender.MapUri("/SignIn/SignIn?returnTo={?}", string.Empty, new []{"redirection"});
        }

        private void RegisterUri<THandler>(IServiceProvider serviceProvider, string uri, Func<THandler, Response> handlerAction)
        {
            Handle.GET(uri,
                () => {
                    var handler = serviceProvider.GetRequiredService<THandler>();
                    return handlerAction(handler);
                });
        }
    }
}