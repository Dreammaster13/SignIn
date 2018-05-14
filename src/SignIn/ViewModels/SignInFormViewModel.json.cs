using System;
using System.Web;
using Microsoft.Extensions.Logging;
using SignIn.DatabaseAccess;
using SignIn.Domain;
using Starcounter;
using Starcounter.Startup.Routing;
using Starcounter.Startup.Routing.Activation;

namespace SignIn.ViewModels
{
    [Url(UriTemplate)]
    partial class SignInFormViewModel : Json, IInitPageWithDependencies, IPageContext<string>
    {
        private const string UriTemplate = "/SignIn/SignIn?returnTo={?}";
        private ISignInManager _signInManager;
        private ITransactionControl _transactionControl;
        private string _returnUrl;
        private ILogger _logger;

        public void Init(ISignInManager signInManager,
            ITransactionControl transactionControl,
            ILogger<SignInFormViewModel> logger)
        {
            _logger = logger;
            _transactionControl = transactionControl;
            _signInManager = signInManager;
        }

        [UriToContext]
        public static string UriToContext(string[] uriParameters)
        {
            return uriParameters[0];
        }

        public void Handle(Input.SignInTrigger input)
        {
            var username = Username;
            try
            {
                var signInResult = _transactionControl.Transact(() => _signInManager.SignIn(username, Password));
                if (signInResult == SignInResult.WrongUsernameOrPassword)
                {
                    Message = SignInMessages.WrongUsernameOrPassword;
                }

                if (signInResult == SignInResult.Success)
                {
                    Message = SignInMessages.SignedInSuccessfully;
                    RedirectUrl = _returnUrl;
                    _logger.LogInformation("Signed in user {0}. Redirecting to {1}", username, RedirectUrl);
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Could not sign in user '{0}'", username);
                Message = SignInMessages.SignInError;
            }
            finally
            {
                Password = "";
            }
        }

        public void HandleContext(string context)
        {
            _returnUrl = HttpUtility.UrlDecode(context);
        }
    }
}
