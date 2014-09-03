﻿using SignInApp.Database;
using Starcounter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignInApp.Server {
    public class Handlers {

        internal static void RegisterHandlers() {

            HandlerOptions opt = new HandlerOptions() { HandlerLevel = 0 };

            Starcounter.Handle.GET("/user", () => {
                var p = new SignIn() {
                    Html = "/signin.html",
                };
                return p;
            });

            Starcounter.Handle.GET("/signinuser", (Request request) => {
                var p = new SignInUser() {
                    Html = "/signinuser.html",
                };
                return p;
            }, opt);

        }
    }
}
