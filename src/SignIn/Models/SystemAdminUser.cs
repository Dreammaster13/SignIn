using System;
using Starcounter;
using Simplified.Ring2;
using Simplified.Ring3;
using System.Net;
using System.Linq;

namespace SignIn.Models
{
    /// <summary>
    /// The SystemAdminUser Model is responsible for validating and creating an Admin user that's part of the admin group.
    /// </summary>
    public class SystemAdminUser
    {
        private  const string _adminGroupName = "Admin (System Users)";
        private const string _adminGroupDescription = "System User Administrator Group";
        private const string _adminUsername = "admin";
        private const string _adminEmail = "admin@starcounter.com";
        
        public bool CanCreateAdminUser => GetCanCreateAdminUser(this.ClientRootAddress);
     
        public bool IsAdminCreated => !this.CanCreateAdminUser;
        
        public IPAddress ClientRootAddress { get; private set; }
        public string Username { get; private set; }
        public string Password { get;  set; }

        public string PasswordRepeat { get; set; }



        /// <summary>
        /// Factory method to create a new SystemAdminUser
        /// </summary>
        /// <param name="clientRootAddress"></param>
        /// <returns></returns>
        internal static SystemAdminUser Create(IPAddress clientRootAddress)
        {
            return new SystemAdminUser()
            {
                Username = _adminUsername,
                ClientRootAddress = clientRootAddress
            };
        }

        /// <summary>
        /// Get if an Admin user can be created or not
        /// </summary>
        /// <param name="clientRootAddress"></param>
        /// <returns></returns>
        internal static  bool GetCanCreateAdminUser(IPAddress clientRootAddress)
        {
            return IPAddress.IsLoopback(clientRootAddress) && !HasUsers() && !HasAdminUser();
        }


        internal void CreateAdminUser(out string message, out bool isAlert)
        {
            isAlert = false;
            message = GetPasswordValidationMessage();
            if (!String.IsNullOrEmpty(message))
            {
                isAlert = true;
                return;
            }
            message = this.GetPasswordRepeatValidationMessage();
            if (!String.IsNullOrEmpty(message))
            {
                isAlert = true;
                return;
            }
            CreateAdminSystemUserIfMissing(this.Password, out message, out isAlert);
        }

      
        /// <summary>
        /// Creates Admin User if missing and adds it to the admin group.  
        /// </summary>
        private void CreateAdminSystemUserIfMissing(string adminPassword, out string message, out bool isAlert)
        {
            message = String.Empty;
            isAlert = false;
            SystemUser user = GetAdminUser();
            //SystemUserGroup group = GetAdminUserGroup();
            //if (IsInGroup(user, group))
            if(user != null)
            {
                message = "There is already an Admin user created";
                isAlert = true;
                return;//Do nothing if there's already an admin user
            }

            // There is no system user beloning to the admin group
            Db.Transact(() =>
            {
                //if (group == null)
                //{
                //    group = new SystemUserGroup();
                //    group.Name = _adminGroupName;
                //    group.Description = _adminGroupDescription;
                //}

                if (user == null)
                {
                    Person person = new Person()
                    {
                        FirstName = _adminUsername,
                        LastName = _adminUsername
                    };

                    user = SystemUser.RegisterSystemUser(_adminUsername, _adminEmail, adminPassword);
                    user.WhatIs = person;
                }

                // Add the admin group to the system admin user
                //SystemUserGroupMember member = new SystemUserGroupMember();

                //member.WhatIs = user;
                //member.ToWhat = group;
            });
            message = $"Admin user with username = '{_adminUsername}' was created";
        }
       
        internal string GetPasswordRepeatValidationMessage()
        {
            return this.Password != this.PasswordRepeat ? "Passwords do not match" : null;
        }
        internal string GetPasswordValidationMessage()
        {
            return string.IsNullOrEmpty(this.Password) ?  "Password cannot be empty" : null;
        }
        private static bool HasUsers() => (Db.SQL($"SELECT o FROM {typeof(SystemUser).FullName} o").FirstOrDefault() != null);
        
        private static bool HasAdminUser()
        {
            return (GetAdminUser() != null);
            //return IsInGroup(GetAdminUser(), GetAdminUserGroup());
        }
        //private static bool IsInGroup(SystemUser user, SystemUserGroup group)
        //{
        //    return (group != null && user != null && SystemUser.IsMemberOfGroup(user, group));
        //}
       
        private static SystemUser GetAdminUser() => Db.SQL<SystemUser>($"SELECT o FROM {typeof(SystemUser).FullName} o WHERE o.{nameof(SystemUser.Username)} = ?", _adminUsername).FirstOrDefault();
        private static SystemUserGroup GetAdminUserGroup() => Db.SQL<SystemUserGroup>($"SELECT o FROM {typeof(SystemUserGroup).FullName} o WHERE o.{nameof(SystemUser.Name)} = ?", _adminGroupName).FirstOrDefault();
        
    }
}