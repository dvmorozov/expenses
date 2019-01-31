using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace SocialApps.Controllers
{
    [Authorize]
    public class PersonalizedController : ErrorHandlingController
    {
        public virtual Guid GetUserId()
        {
            if (User != null)
            {   //  check for unit-testing mode
                var userId = User.Identity.GetUserId();
                if (userId != null)
                {
                    return Guid.Parse(userId);
                }
            }
            return new Guid();
        }
    }
}
