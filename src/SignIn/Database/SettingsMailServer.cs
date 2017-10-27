using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Starcounter;

namespace SignIn
{
    [Database]
    public class SettingsMailServer
    {
        public string Name { get; set; }
        public bool Enabled { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public bool EnableSsl { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string SiteHost { get; set; }
        public long SitePort { get; set; }
    }
}
