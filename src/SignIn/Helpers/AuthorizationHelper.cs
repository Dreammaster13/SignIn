﻿using Simplified.Ring3;
using Simplified.Ring5;
using Starcounter;
using System.Linq;

namespace SignIn.Helpers
{
    // TODO: Replace it with the new Authorization module. The following code was copied and adapted from UserAdmin app (Helper class).
    public static class AuthorizationHelper
    {
        internal static string AdminGroupName = "Admin (System Users)";
        internal static string AdminGroupDescription = "System User Administrator Group";

        public static void SetupPermissions()
        {
            SystemUserGroup adminGroup = GetAdminGroup();
            AssureUriPermission("/signin/settings", adminGroup);
            AssureUriPermission("/signin/user/authentication/settings/{?}", adminGroup);
        }

        public static void AssureUriPermission(string uri, SystemUserGroup group)
        {
            var permission = Db.SQL<UriPermission>(
                "SELECT o.Permission FROM Simplified.Ring5.SystemUserGroupUriPermission o " +
                "WHERE o.Permission.Uri=? AND o.SystemUserGroup=?", uri, group)
                .FirstOrDefault();

            if (permission == null)
            {
                Db.Transact(() =>
                {
                    var p1 = new UriPermission { Uri = uri, CanGet = true };
                    new SystemUserGroupUriPermission { ToWhat = p1, WhatIs = group };
                });
            }
        }

        public static bool TryNavigateTo(string url, Request request, out Json returnPage)
        {
            returnPage = null;

            SystemUser systemUser = SystemUser.GetCurrentSystemUser();
            if (systemUser == null)
            {
                // Ask user to sign in.
                returnPage = Self.GET("/signin/partial/accessdenied-form");
                return false;
            }

            // Check user permission
            if (!CanGetUri(systemUser, url, request))
            {
                // User has no permission, redirect to the Access Denied page
                returnPage = Self.GET("/signin/partial/accessdenied-form");
                return false;
            }

            return true;
        }

        public static bool IsMemberOfGroup(SystemUser user, SystemUserGroup basedOnGroup)
        {
            if (user == null || basedOnGroup == null)
            {
                return false;
            }

            var groups = Db.SQL<SystemUserGroup>(
                "SELECT o.SystemUserGroup FROM Simplified.Ring3.SystemUserGroupMember o " +
                "WHERE o.SystemUser=?", user);

            foreach (var groupItem in groups)
            {
                if (IsBasedOnGroup(groupItem, basedOnGroup))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// TODO: Avoid circular references!!
        /// </summary>
        /// <param name="group"></param>
        /// <param name="basedOnGroup"></param>
        /// <returns></returns>
        private static bool IsBasedOnGroup(SystemUserGroup group, SystemUserGroup basedOnGroup)
        {
            if (group == null) return false;

            // NOTE: To compare to objects queried from database we need to use .Equals(),  "==" wont work!!.
            if (group.Equals(basedOnGroup))
            {
                return true;
            }

            if (IsBasedOnGroup(group.Parent, basedOnGroup))
            {
                return true;
            }

            return false;
        }

        private static bool CanGetUri(SystemUser user, string uri, Request request)
        {
            // Check if there is any permission set for a url
            var per = Db.SQL<UriPermission>(
                "SELECT o FROM  Simplified.Ring5.UriPermission o WHERE o.Uri=?", uri)
                .FirstOrDefault();

            if (per == null)
            {
                // TODO: Check if user is part of Admin group, then allow acces?
                // No permission configuration for this url = DENY ACCESS
                return false;
            }

            UriPermission permission = GetPermission(user, uri);
            if (permission != null)
            {
                return permission.CanGet;
            }

            return false;
        }

        private static UriPermission GetPermission(SystemUser user, string uri)
        {
            if (user == null || string.IsNullOrEmpty(uri))
            {
                return null;
            }

            var permission = Db.SQL<UriPermission>(
                "SELECT o.Permission FROM Simplified.Ring5.SystemUserUriPermission o " +
                "WHERE o.Permission.Uri=? AND o.SystemUser=?", uri, user)
                .FirstOrDefault();

            if (permission != null)
            {
                return permission;
            }

            // Check user group
            var groups = Db.SQL<SystemUserGroupMember>(
                "SELECT o FROM Simplified.Ring3.SystemUserGroupMember o " +
                "WHERE o.SystemUser=?", user);

            foreach (var group in groups)
            {
                permission = GetPermissionFromGroup(group.SystemUserGroup, uri);
                if (permission != null)
                {
                    return permission;
                }
            }
            return null;
        }

        private static UriPermission GetPermissionFromGroup(SystemUserGroup group, string url)
        {
            if (group == null) return null;

            UriPermission permission = Db.SQL<UriPermission>(
                "SELECT o.Permission FROM Simplified.Ring5.SystemUserGroupUriPermission o " +
                "WHERE o.Permission.Uri=? AND o.SystemUserGroup=?", url, group).FirstOrDefault()
                ?? GetPermissionFromGroup(group.Parent, url);

            return permission;
        }

        private static SystemUserGroup GetAdminGroup()
        {
            var adminGroup = Db.SQL<SystemUserGroup>(
                "SELECT o FROM Simplified.Ring3.SystemUserGroup o WHERE o.Name = ?",
                AdminGroupName)
                .FirstOrDefault();

            if (adminGroup == null)
            {
                Db.Transact(() =>
                {
                    adminGroup = new SystemUserGroup
                    {
                        Name = AdminGroupName,
                        Description = AdminGroupDescription
                    };
                });
            }
            return adminGroup;
        }
    }
}