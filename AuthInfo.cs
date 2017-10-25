using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BD.Extensions.AA {
    public abstract class AuthInfo {        
        private HttpContextBase httpContext;

        public HttpContextBase HttpContext {
            get {
                return httpContext;
            }
            set {
                httpContext = value;
                this.HttpRequest = httpContext.Request;
            }
        }

        public HttpRequestBase HttpRequest { get; set; }        

        public string SeverName { 
            get {
                return httpContext.Server.MachineName;
            }
        }

        public string MethodName {
            get {
                return this.GetType().Name;
            }
        }        

        public string UserName {
            get {
                return HttpContext.User.Identity.Name;
            }
        }
        
        public AuthInfo() {}

        public AuthInfo(HttpContextBase httpContext) {
            this.HttpContext = httpContext;                 
        }
                
        public virtual bool IsAuthenticated() {
            return HttpRequest.IsAuthenticated;
        }

        public abstract string GetClientIP();        
        public abstract string RedirectToLoginUrl();
        public abstract string RedirectToLogoutUrl();

        public abstract void SetAuthCookie(string userName, bool createPresistentCookie);
        public abstract void SignOut();
    }
}