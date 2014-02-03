using Microsoft.Practices.Unity;
using System;
using System.Web.Mvc;
using System.Web.Security;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.Queries;

namespace Zbang.Zbox.Mvc3WebRole.Security
{
    public class ZboxRoleProvider : RoleProvider
    {
        public ZboxRoleProvider()
        {
            m_ZboxReadService = DependencyResolver.Current.GetService<IZboxReadService>();
        }

        [Dependency]
        public IZboxReadService m_ZboxReadService { get; set; }


        const string applicationName = "Zbox";
        public const string verifyEmailRole = "verify";
        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override string ApplicationName
        {
            get
            {
                return applicationName;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        public override string[] GetRolesForUser(string username)
        {
            try
            {
                long userid = Convert.ToInt64(username);
                if (userid == 0)
                {
                    TraceLog.WriteError("GetRolesForUser username " + username);
                    return new[] { string.Empty };
                }
                var verifyQuery = new GetUserDetailsQuery(userid);
                var result = m_ZboxReadService.GetUserDetails(verifyQuery);

                return new[] { result.IsEmailVerified ? verifyEmailRole : string.Empty };
            }            
            catch (Exception ex)
            {
                TraceLog.WriteError("GetRolesForUser username " + username, ex);
                return new string[] {};
            }
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            if (roleName == verifyEmailRole)
            {
                long userid = Convert.ToInt64(username);
                var verifyQuery = new GetUserDetailsQuery(userid);
                var result = m_ZboxReadService.GetUserDetails(verifyQuery);

                return result.IsEmailVerified;
            }
            return false;
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }
    }
}