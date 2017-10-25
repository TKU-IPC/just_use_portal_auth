using System;
using System.Web;

namespace BD.Extensions.AA {
    public class AuthInfoFactory {
        private AuthInfo authInfo { get; set; }

        public AuthInfo GetAuthInfo(string isPortal, HttpContextBase httpContext) {
            switch (isPortal) {
                case "yes":
                    authInfo = new PortalAuthInfo(httpContext);
                    break;

                case "no":
                    authInfo = new NoPortalAuthInfo(httpContext);
                    break;

                case "none":
                    authInfo = new FormsAuthInfo(httpContext);
                    break;

                default:
                    break;
            }
            
            return authInfo;
        }
    }
}