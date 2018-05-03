using System;
using Starcounter;
using Starcounter.Startup;

namespace SignIn
{
    class Program
    {
        static void Main()
        {
            DefaultStarcounterBootstrapper.Start(new Startup());
        }
    }
}