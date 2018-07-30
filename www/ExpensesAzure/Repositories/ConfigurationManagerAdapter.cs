#if DEBUG
using System.Configuration;
#else
using Microsoft.Azure;
#endif

namespace SocialApps.Repositories
{
    public class ConfigurationManagerAdapter
    {
        static public string GetSetting(string name) {
#if DEBUG
            return ConfigurationManager.AppSettings["Microsoft.clientId"];
#else
            return CloudConfigurationManager.GetSetting("Microsoft.clientId");
#endif
        }
    }
}