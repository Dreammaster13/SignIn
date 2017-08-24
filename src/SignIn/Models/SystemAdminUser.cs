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
                return GetCanCreateAdminUser(this.ClientRootAddress);
            }
        }
        public bool IsAdminCreated
        {
            get
            {
                return !this.CanCreateAdminUser;
            }
        }


        public string ClientRootAddress { get; private set; }
        public string Username { get; private set; }
        public string Password { get;  set; }

        public string PasswordRepeat { get; set; }
        



        internal static SystemAdminUser Create(string clientRootAddress)
        {
            return new SystemAdminUser()
            {
                Username = AdminUsername,
                ClientRootAddress = clientRootAddress
            };
        }


        internal static bool GetCanCreateAdminUser(string clientRootAddress)
        {
            return IsLocal(clientRootAddress) && !HasUsers() && !HasAdminUser();
        }


        internal void CreateAdminUser(string repeatedPassword, out string message, out bool isAlert)
        {
            message = String.Empty;
            bool isValid = IsValidPassword( out message) && IsEqualPassword(repeatedPassword, out message);
            isAlert = !isValid;
            if (isValid)
            {
                CreateAdminSystemUserIfMissing(this.Password, out message, out isAlert);
            }
        }

      
        /// <summary>
        /// Creates Admin User if missing and adds it to the admin group.  
        /// </summary>
        private void CreateAdminSystemUserIfMissing(string adminPassword, out string message, out bool isAlert)
        {
            message = String.Empty;
            isAlert = false;
            SystemUser user = GetAdminUser();
            SystemUserGroup group = GetAdminUserGroup();
            if (IsInGroup(user, group))
            {
                message = "There is already an Admin user created";
                isAlert = true;
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
       
        internal bool IsEqualPassword(string repeatedPassword, out string message)
        {
            message = String.Empty;
            if (this.Password != repeatedPassword)
            {
                message = "Passwords do not match";
                return false;
            }
            return true;
        }
       
       
        internal bool IsValidPassword(out string message)
        {
            message = String.Empty;
            bool isValid = true;
            if (string.IsNullOrEmpty(this.Password))
            {
                message = "Password cannot be empty";
                isValid = false;
            }
            return isValid;

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
            return IsInGroup(user, group);
        }
        private static bool IsInGroup(SystemUser user, SystemUserGroup group)
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