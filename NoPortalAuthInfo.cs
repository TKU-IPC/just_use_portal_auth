using System;
using System.Security.Principal;
using System.Web;
using System.Web.Configuration;
using System.Web.Security;

namespace BD.Extensions.AA {
    public class NoPortalAuthInfo : AuthInfo {
        public NoPortalAuthInfo() : base() { }
        public NoPortalAuthInfo(HttpContextBase httpContext) : base(httpContext) { }

        // 不能拿掉，因為在 Account/Sso 是從前端轉址，此時 FormsAuthentication 尚未寫值，故仍需判斷
        public override bool IsAuthenticated() {
            string userName = HttpRequest.QueryString["u"] ?? string.Empty;
            bool fromPortal = !string.IsNullOrEmpty(userName),
                 IsAuth     = base.IsAuthenticated();

            // 從 portal 移轉過來時
            // IsAuth 要判斷！因為是用 QueryString 方式帶入，如果不判斷的話，會有安全性問題。
            if (fromPortal) {
                if (!IsAuth) {
                    FormsAuthentication.SetAuthCookie(userName, false);

                    //renew ticket
                    var cookie = HttpRequest.Cookies[FormsAuthentication.FormsCookieName];
                    var ticket = FormsAuthentication.Decrypt(cookie.Value);

                    HttpContext.User = new GenericPrincipal(new FormsIdentity(ticket), null);

                    IsAuth = true;
                }
                else {
                    // 驗證通過但是使用者代號不同，代表有人在 try 帳號，應予以剔除。
                    if (userName != HttpContext.User.Identity.Name) {
                        FormsAuthentication.RedirectToLoginPage();                        
                    }
                }
            }

            return IsAuth;
        }

        public override string GetClientIP() {
            HttpCookie cookie = HttpRequest.Cookies["TKU_CLIENT_IP"] ?? HttpRequest.Cookies["TKU%5FCLIENT%5FIP"];
            string clientIP = (cookie == null ? "UNKNOWN_CLIENT_IP" : cookie.Value);

            return clientIP;
        }

        public override string RedirectToLoginUrl() {
            string entryUrl = WebConfigurationManager.AppSettings["EntryUrl"];
            return entryUrl;
        }

        public override string RedirectToLogoutUrl() {
            FormsAuthentication.SignOut();
            string signoutUrl = "http://sso.tku.edu.tw/pkmslogout";
            return signoutUrl;
        }

        public override void SetAuthCookie(string userName, bool createPresistentCookie) {
            throw new NotImplementedException();
        }

        public override void SignOut() {
            FormsAuthentication.SignOut();
        }
    }
}