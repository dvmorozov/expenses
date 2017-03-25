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

            // Раскомментируйте приведенные далее строки, чтобы включить вход с помощью сторонних поставщиков входа
            app.UseMicrosoftAccountAuthentication(
                clientId: "000000004414CB41",
                clientSecret: "2jCGjD9qBSKRDNqz-fPzz-nlesfzPz0M");

            app.UseTwitterAuthentication(
               consumerKey: "33XcEz3Yq1UQaQOyT2FCxnMpM",
               consumerSecret: "Acn03sLewO0cvQy6cXn2ZneE1DvQ4aGZU2eYDbhY6nquF9RwF6");

            app.UseFacebookAuthentication(
               appId: "1580602192213180",
               appSecret: "55136442251f33df6e4739256123c288");

            app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            {
                ClientId = "163635128807-dt2m86u271tdgfumnna3svuv54bt6j0c.apps.googleusercontent.com",
                ClientSecret = "LfQtkUzSiDL_RYbvWt73SIBa"
            });

            /*
            //  https://www.evernote.com/shard/s132/nl/14501366/c928f0f7-ada3-48c8-bc82-7721f081e12c
            app.UseYahooAuthentication(
                "dj0yJmk9Z2lDS2FvYVFieXREJmQ9WVdrOWFIb3dSRmRaTXpnbWNHbzlNQS0tJnM9Y29uc3VtZXJzZWNyZXQmeD1jOA--", 
                "cce7af2f4fa02b5cd80d3134398b4832dc07535c");


            app.UseLinkedInAuthentication(
                "75wumudbpro6a2", 
                "a96067df-6d96-4904-b2e4-a3f5de080f60");
             */
        }
    }
}