using System;
using System.Web;
using Starcounter;
using Starcounter.Internal;
using Simplified.Ring2;
using Simplified.Ring3;
using Simplified.Ring5;
using SignIn.Helpers;

namespace SignIn.Models
{
    public class SystemAdminUser
    {
        internal static string AdminGroupName = "Admin (System Users)";
        internal static string AdminGroupDescription = "System User Administrator Group";
        internal static string AdminUsername = "admin";
        internal static string AdminEmail = "admin@starcounter.com";
        

   
        public bool CanCreateAdminUser
        {
            get
            {
                return GetCanCreateAdminUser();
            }
        }

      
        public string ClientRootAddress { get; private set; }
        public string Username { get; private set; }
        public string Password { get; internal set; }
        public string PasswordSalt { get; internal set; }



        /// <summary>
        /// Creates Admin User if missing and adds it to the admin group.  
        /// </summary>
        internal static void CreateAdminSystemUserIfMissing(string adminPassword, out string message)
        {
            message = String.Empty;
            SystemUser user = GetAdminUser();
            SystemUserGroup group = GetAdminUserGroup();
            if (HasAdminUser(user, group))
            {
                message = "There is already an Admin user created";
                return;//Do nothing if there's already an admin user
            }

            // There is no system user beloning to the admin group
            Db.Transact(() =>
            {
                if (group == null)
                {
                    group = new SystemUserGroup();
                    group.Name = AdminGroupName;
                    group.Description = AdminGroupDescription;
                }

                if (user == null)
                {
                    Person person = new Person()
                    {
                        FirstName = AdminUsername,
                        LastName = AdminUsername
                    };

                    user = SystemUser.RegisterSystemUser(AdminUsername, AdminEmail, adminPassword);
                    user.WhatIs = person;
                }

                // Add the admin group to the system admin user
                SystemUserGroupMember member = new Simplified.Ring3.SystemUserGroupMember();

                member.WhatIs = user;
                member.ToWhat = group;
            });
            message = $"Admin user with username = '{AdminUsername}' was created";
        }

        internal static SystemAdminUser Create(string clientRootAddress)
        {
            return  new SystemAdminUser()
            {
                Username = AdminUsername,
                ClientRootAddress = clientRootAddress
            };
        }

        private  bool GetCanCreateAdminUser()
        {
            return GetCanCreateAdminUser(this.ClientRootAddress);
        }
        internal static bool GetCanCreateAdminUser(string clientRootAddress)
        {
            return IsLocal(clientRootAddress) && !HasUsers() && !HasAdminUser();
        }
       
        private static bool HasUsers()
        {
            return (Db.SQL("SELECT o FROM Simplified.Ring3.SystemUser o").First != null);
        
        }

        private static  bool IsLocal(string ip)
        {
            if (string.IsNullOrEmpty(ip))
                return false;

            return (ip == "127.0.0.1" || ip.Equals( "localhost", StringComparison.CurrentCultureIgnoreCase));
 
        }
        private static bool HasAdminUser()
        {
            SystemUser user = GetAdminUser();
            SystemUserGroup group = GetAdminUserGroup();
            return HasAdminUser(user, group);
        }
        private static bool HasAdminUser(SystemUser user, SystemUserGroup group)
        {
            return (group != null && user != null && SystemUser.IsMemberOfGroup(user, group));
        }
        private static SystemUser GetAdminUser()
        {
            SystemUser user =  Db.SQL<SystemUser>("SELECT o FROM Simplified.Ring3.SystemUser o WHERE o.Username = ?", AdminUsername).First;
            return user;
        }
        private static SystemUserGroup GetAdminUserGroup()
        {
            SystemUserGroup group = Db.SQL<SystemUserGroup>("SELECT o FROM Simplified.Ring3.SystemUserGroup o WHERE o.Name = ?",AdminGroupName).First;
            return group;
        }
    }
}