using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SocialApps.Startup))]
namespace SocialApps
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
