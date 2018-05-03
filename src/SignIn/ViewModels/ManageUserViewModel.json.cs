using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SignIn.DatabaseAccess;
using Starcounter;
using Starcounter.Startup.Routing.Activation;

namespace SignIn.ViewModels
{
    partial class ManageUserViewModel : Json, IInitPageWithDependencies, IBound<SystemUser>
    {
        private ILogger<ManageUserViewModel> _logger;
        private IPasswordHasher<SystemUser> _passwordHasher;
        private ITransactionControl _transactionControl;

        public void Init(ILogger<ManageUserViewModel> logger,
            IPasswordHasher<SystemUser> passwordHasher,
            ITransactionControl transactionControl)
        {
            _transactionControl = transactionControl;
            _logger = logger;
            _passwordHasher = passwordHasher;
        }

        public void Handle(Input.ApplyTrigger input)
        {
            if (NewPassword != NewPasswordRepeat)
            {
                Message = SignInMessages.PasswordsDontMatch;
                return;
            }

//            _transactionControl.Transact(() => 
            Data.Password = _passwordHasher.HashPassword(Data, NewPassword);
//                );
            _logger.LogInformation("Password has been changed for user {0}", Data.Username);
            Message = SignInMessages.PasswordChanged;
        }
    }
}
