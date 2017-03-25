using System.Web;
using System.Web.Mvc;

namespace SocialApps
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new System.Web.Mvc.AuthorizeAttribute());
            //  https://www.evernote.com/shard/s132/nl/14501366/dbf780a8-832e-48c6-bb02-6ed7cf9003d1
            //  filters.Add(new RequireHttpsAttribute());
        }
    }
}
