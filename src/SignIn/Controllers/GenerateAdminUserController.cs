using Starcounter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignIn.Controllers
{
    public class GenerateAdminUserController
    {
        public void Register()
        {
            Handle.GET("/signin/generateadminuser", (Request request) =>
            {
                if (Db.SQL("SELECT o FROM Simplified.Ring3.SystemUser o").First != null)
                {
                    Handle.SetOutgoingStatusCode(403);
                    return "Unable to generate admin user: database is not empty!";
                }

                string ip = request.ClientIpAddress.ToString();

                if (ip == "127.0.0.1" || ip == "localhost")
                {
                    SignInOut.AssureAdminSystemUser();

                    return "Default admin user has been successfully generated.";
                }

                Handle.SetOutgoingStatusCode(403);
                return "Access denied.";

            }, new HandlerOptions() { SkipRequestFilters = true });
        }
    }
}
