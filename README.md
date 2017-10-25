# 使用 Portal 驗證完畢後，轉址回原 web server 網址繼續執行 (asp.net mvc5)
1. 在 web.config appSettings 區段加入 "isPortal" 以決定系統要採用那種驗證方式，目前設定有以下三種：  
   - yes: 使用 portal 驗證，驗證完畢後於 portal 繼續執行  
   - no: 使用 portal 僅驗證，驗證完畢後跳離 portal 執行  
   - none: 自行驗證
   
2. 在 account/login 使用抽象類別 AuthInfo 以及工廠類別 AuthInfoFactory 搭配 "isPortal" 決定使用哪一種驗證：
  - yes: PortalAuthInfo  
  - no: noPortalAuthInfo  
  - none: FormsAuthInfo

本例說明使用 noPrortalAuthInfo
1.  將 portal 登入頁面以 iframe 方式嵌入,	syntax: 
    <iframe id="SsoIframe" src="http://sso.tku.edu.tw/aisinfo/bd/account/sso?&embed=YES" frameborder="0" scrolling="no" style="width:450px; height:250px;"></iframe>
	
  	其中 src 為當驗證成功後，sso 會重導向的網址，此處是重導向至 "account/sso"

2. 重導向到指定網址(mvc action)後，將 Request.Headers["sso_userid"] 值寫入 "FormsAuthentication"，產生 Forms 驗證, 並傳送此值至 View 供 javascript 擷取。為了避免系統的每一個頁面都會被嵌在 iframe 中，我們將真正要前往的網址放入 hidden (避免被 portal 修改)，並用 javascript 讀取 hidden 值，然後以 window.top.location.href 重新導向(跳脫portal) 至指定網址。

3. 為了避免 FormsAuthentication 來不及 renew ticket 導致驗證失敗，此處會額外攜帶一個 QueryString，儲存 Request.Headers["sso_userid"] 之值，但是這種方式容易被有心人士竄改，故需要在程式碼中加入判斷是否有遭竄改，如果有，就使用 FormsAuthentication.RedirectToLoginPage() 重新導向至登入頁面，通常是 account/login (非HttpPost)，以將竄改之使用者登出。
