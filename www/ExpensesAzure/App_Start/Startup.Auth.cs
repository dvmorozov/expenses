using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Owin;
using Owin.Security.Providers.Yahoo;
using Owin.Security.Providers.LinkedIn;
using SocialApps.Models;
using Microsoft.Azure;
using SocialApps.Repositories;

namespace SocialApps
{
    public partial class Startup
    {
        // Дополнительные сведения о настройке проверки подлинности см. по адресу: http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Настройка контекста базы данных, диспетчера пользователей и диспетчера входа для использования одного экземпляра на запрос
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            // Включение использования файла cookie, в котором приложение может хранить информацию для пользователя, выполнившего вход,
            // и использование файла cookie для временного хранения информации о входах пользователя с помощью стороннего поставщика входа
            // Настройка файла cookie для входа
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                Provider = new CookieAuthenticationProvider
                {
                    // Позволяет приложению проверять метку безопасности при входе пользователя.
                    // Эта функция безопасности используется, когда вы меняете пароль или добавляете внешнее имя входа в свою учетную запись.  
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                        validateInterval: TimeSpan.FromMinutes(30),
                        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                }
            });            
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Позволяет приложению временно хранить информацию о пользователе, пока проверяется второй фактор двухфакторной проверки подлинности.
            app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

            // Позволяет приложению запомнить второй фактор проверки имени входа. Например, это может быть телефон или почта.
            // Если выбрать этот параметр, то на устройстве, с помощью которого вы входите, будет сохранен второй шаг проверки при входе.
            // Точно так же действует параметр RememberMe при входе.
            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);

            app.UseMicrosoftAccountAuthentication(
                clientId: ConfigurationManagerAdapter.GetSetting("Microsoft.clientId"),
                clientSecret: ConfigurationManagerAdapter.GetSetting("Microsoft.clientSecret"));

            app.UseTwitterAuthentication(
               consumerKey: ConfigurationManagerAdapter.GetSetting("Twitter.consumerKey"),
               consumerSecret: ConfigurationManagerAdapter.GetSetting("Twitter.consumerSecret"));

            app.UseFacebookAuthentication(
               appId: ConfigurationManagerAdapter.GetSetting("Facebook.appId"),
               appSecret: ConfigurationManagerAdapter.GetSetting("Facebook.appSecret"));

            app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            {
                ClientId = ConfigurationManagerAdapter.GetSetting("Google.ClientId"),
                ClientSecret = ConfigurationManagerAdapter.GetSetting("Google.ClientSecret")
            });

            //  https://www.evernote.com/shard/s132/nl/14501366/c928f0f7-ada3-48c8-bc82-7721f081e12c
            app.UseYahooAuthentication(
                consumerKey: ConfigurationManagerAdapter.GetSetting("Yahoo.consumerKey"), 
                consumerSecret: ConfigurationManagerAdapter.GetSetting("Yahoo.consumerSecret"));

            app.UseLinkedInAuthentication(
                clientId: ConfigurationManagerAdapter.GetSetting("LinkedIn.clientId"), 
                clientSecret: ConfigurationManagerAdapter.GetSetting("LinkedIn.clientSecret"));
        }
    }
}