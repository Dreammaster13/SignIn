using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignIn
{
    public class SystemUser : Something
    {

        public string Username
        {
            get { return Name; }
            set { Name = value; }
        }
    }
}