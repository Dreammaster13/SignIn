using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SignIn.Database;
using SignIn.DatabaseAccess;
using Starcounter.Authorization.Model;
using Starcounter.Authorization.SignIn;

namespace SignIn.Domain
{
    public class SignInManager : ISignInManager
    {
        private readonly ISignInManager<SystemUserSession, SystemUser> _signInManager;
        private readonly IPasswordHasher<SystemUser> _passwordHasher;
        private readonly IUsersRepository _usersRepository;
        private readonly IScAuthenticationTicketRepository<SystemUserSession> _authenticationTicketRepository;
        private readonly ILogger<SignInManager> _logger;

        public SignInManager(ISignInManager<SystemUserSession, SystemUser> signInManager,
            IPasswordHasher<SystemUser> passwordHasher,
            IUsersRepository usersRepository,
            IScAuthenticationTicketRepository<SystemUserSession> authenticationTicketRepository,
            ILogger<SignInManager> logger
            )
        {
            _signInManager = signInManager;
            _passwordHasher = passwordHasher;
            _usersRepository = usersRepository;
            _authenticationTicketRepository = authenticationTicketRepository;
            _logger = logger;
        }

        public SignInResult SignIn(string username, string password)
        {
            var user = _usersRepository.FindByUsername(username);
            if (user == null)
            {
                _logger.LogWarning($"User '{username}' not found");
                return SignInResult.WrongUsernameOrPassword;
            }

            var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(user, user.Password, password);
            if (passwordVerificationResult == PasswordVerificationResult.Failed)
            {
                _logger.LogInformation($"Could not sign in user '{username}': wrong password");
                return SignInResult.WrongUsernameOrPassword;
            }

            if (passwordVerificationResult == PasswordVerificationResult.SuccessRehashNeeded)
            {
                _logger.LogInformation($"Rehashing password for user '{username}'");
                user.Password = _passwordHasher.HashPassword(user, password);
            }

            var authenticationTicket = _authenticationTicketRepository.Create();
            _signInManager.SignIn(user, authenticationTicket);

            return SignInResult.Success;
        }
    }
}