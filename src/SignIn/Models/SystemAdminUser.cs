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
        private const string AdminGroupName = "Admin (System Users)";
        private const string AdminGroupDescription = "System User Administrator Group";
        private const string AdminUsername = "admin";
        private const string AdminEmail = "admin@starcounter.com";

        public bool CanCreateAdminUser => GetCanCreateAdminUser(this.ClientRootAddress);

        public bool IsAdminCreated => !this.CanCreateAdminUser;

        public IPAddress ClientRootAddress { get; private set; }
        public string Username { get; private set; }
        public string Password { get; set; }

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
                Username = AdminUsername,
                ClientRootAddress = clientRootAddress
            };
        }

        /// <summary>
        /// Get if an Admin user can be created or not
        /// </summary>
        /// <param name="clientRootAddress"></param>
        /// <returns></returns>
        internal static bool GetCanCreateAdminUser(IPAddress clientRootAddress)
        {
            return IPAddress.IsLoopback(clientRootAddress) && !HasUsers() && !HasAdminUser();
        }

        internal void CreateAdminUser(out string message, out bool isAlert)
        {
            isAlert = false;
            message = GetPasswordValidationMessage();
            if (!string.IsNullOrEmpty(message))
            {
                isAlert = true;
                return;
            }
            message = this.GetPasswordRepeatValidationMessage();
            if (!string.IsNullOrEmpty(message))
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
            message = string.Empty;
            isAlert = false;
            SystemUser user = GetAdminUser();
            SystemUserGroup group = GetAdminUserGroup();
            if (IsInGroup(user, group))
            {
                message = "There is already an Admin user created";
                isAlert = true;
                return;//Do nothing if there's already an admin user
            }

            // There is no system user belonging to the admin group
            Db.Transact(() =>
            {
                if (group == null)
                {
                    group = new SystemUserGroup
                    {
                        Name = AdminGroupName,
                        Description = AdminGroupDescription
                    };
                }

                if (user == null)
                {
                    var person = new Person()
                    {
                        FirstName = AdminUsername,
                        LastName = AdminUsername
                    };

                    user = SystemUser.RegisterSystemUser(AdminUsername, AdminEmail, adminPassword);
                    user.WhatIs = person;
                }

                // Add the admin group to the system admin user
                var member = new SystemUserGroupMember
                {
                    WhatIs = user,
                    ToWhat = group
                };
            });
            message = $"Admin user with username = '{AdminUsername}' was created";
        }

        internal string GetPasswordRepeatValidationMessage() =>
            this.Password != this.PasswordRepeat ? "Passwords do not match" : null;

        internal string GetPasswordValidationMessage() =>
            string.IsNullOrEmpty(this.Password) ? "Password cannot be empty" : null;

        private static bool HasUsers() =>
            Db.SQL($"SELECT o FROM {typeof(SystemUser).FullName} o").Any();

        private static bool HasAdminUser() =>
            IsInGroup(GetAdminUser(), GetAdminUserGroup());

        private static bool IsInGroup(SystemUser user, SystemUserGroup group) =>
            group != null &&
            user != null &&
            SystemUser.IsMemberOfGroup(user, group);

        private static SystemUser GetAdminUser() =>
            Db.SQL<SystemUser>(
                $"SELECT o FROM {typeof(SystemUser).FullName} o " +
                $"WHERE o.{nameof(SystemUser.Username)} = ?", AdminUsername)
            .FirstOrDefault();

        private static SystemUserGroup GetAdminUserGroup() =>
            Db.SQL<SystemUserGroup>(
                $"SELECT o FROM {typeof(SystemUserGroup).FullName} o " +
                $"WHERE o.{nameof(SystemUser.Name)} = ?", AdminGroupName)
            .FirstOrDefault();
    }
}