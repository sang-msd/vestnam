using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TGClothes.Common
{
    public class HasCredentialAttribute : AuthorizeAttribute
    {
        public string RoleId { get; set; }
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            //var isAuthorized = base.AuthorizeCore(httpContext);
            //if (!isAuthorized)
            //{
            //    return false;
            //}
            var session = (UserLogin)HttpContext.Current.Session[CommonConstants.USER_SESSION];
            List<string> privilegeLevels = this.GetCredentialByLoggedUser(session.Email);
            if (privilegeLevels.Contains(this.RoleId))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private List<string> GetCredentialByLoggedUser(string email)
        {
            var credential = (List<string>)HttpContext.Current.Session[CommonConstants.SESSION_CREDENTIAL];
            return credential;
        }
    }
}