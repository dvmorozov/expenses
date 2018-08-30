using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Helpers;
using System.Web.Mvc;
using SocialApps.Models;
using SocialApps.Repositories;

namespace SocialApps.Controllers
{
    [Authorize]
    public partial class MobileController : PersonalizedController
    {
        private static int _seqNum;

        public ActionResult Reports()
        {
            try
            {
                return View();
            }
            catch (Exception e)
            {
                Application_Error(e);
                return View("Error", new HandleErrorInfo(e, "Mobile", "Reports"));
            }
        }

        //  https://action.mindjet.com/task/14919145
        private CurrencyGroup[] GetCurrencyGroups<T>(IEnumerable<T> allItems)
        {
#if DEBUG
            foreach (var i in allItems)
                Debug.Assert(i.GetType().GetProperty("GROUPID1").GetValue(i) != null);
#endif
            //  Uses reflection to get data from objects of template parameter type.
            return allItems.GroupBy(item => new
            {
                GroupId = (long)item.GetType().GetProperty("GROUPID1").GetValue(item)
            })
            .Select(group => new CurrencyGroup
            {
                GroupId = group.Key.GroupId,
                //  https://github.com/dvmorozov/expenses/issues/10
                //  Selects the first item from each group.
                Currency = MobileRepository.GetCurrency(group)
            }).ToArray();
        }

        //  https://github.com/dvmorozov/expenses/issues/10
        private IEnumerable<T> FilterItemsByGroupId<T>(IEnumerable<T> allItems, int groupId)
        {
            //  Select items of given currency group.
            return allItems.Where(t => (long)t.GetType().GetProperty("GROUPID1").GetValue(t) == groupId);
        }
    }
}
