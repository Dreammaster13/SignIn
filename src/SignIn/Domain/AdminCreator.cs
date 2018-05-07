using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SignIn.Database;
using SignIn.DatabaseAccess;

namespace SignIn.Domain
{
    public class AdminCreator
    {
        private readonly IPasswordHasher<SystemUser> _passwordHasher;
        private readonly ILogger<AdminCreator> _logger;
        private readonly IUsersRepository _usersRepository;

        public AdminCreator(IPasswordHasher<SystemUser> passwordHasher,
            ILogger<AdminCreator> logger,
            IUsersRepository usersRepository)
        {
            _passwordHasher = passwordHasher;
            _logger = logger;
            _usersRepository = usersRepository;
        }

        public void CreateAdmin()
        {
            var adminUsername = "admin";
            var adminPassword = "admin";
            var admin = _usersRepository.FindByUsername(adminUsername);
            if (admin != null)
            {
                _logger.LogInformation("Admin already exists");
                return;
            }

            _logger.LogInformation("Creating admin");
            admin = _usersRepository.Create();
            admin.Username = adminUsername;
            admin.Password = _passwordHasher.HashPassword(admin, adminPassword);
        }
    }
}