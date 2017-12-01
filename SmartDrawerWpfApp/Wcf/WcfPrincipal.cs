using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace SmartDrawerWpfApp.Wcf
{
    public class WcfPrincipal : IPrincipal
    {
        IIdentity _identity;
        string[] _roles;

        public WcfPrincipal(IIdentity identity)
        {
            _identity = identity;
            LoadUsersRoles();
        }

        /// <summary>
        /// we should load up the user's roles from the database here based on username
        /// </summary>
        private void LoadUsersRoles()
        {
            //run query on db using _identity.Name
            //then put into _roles
            _roles = new string[] { "WcfUser" };
        }

        public IIdentity Identity
        {
            get { return _identity; }
        }

        public bool IsInRole(string role)
        {
            if (_roles.Contains(role))
                return true;
            else
                return false;
        }

        public string[] Roles
        {
            get { return _roles; }
        }
    }
}
