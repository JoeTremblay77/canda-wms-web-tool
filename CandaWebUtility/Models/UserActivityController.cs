using CandaWebUtility.Data;
using System.Collections.Generic;
using System.Web.Mvc;
using System;
using System.Linq;

namespace CandaWebUtility
{
    public class UserActivityController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                using (EFEntities db = new EFEntities())
                {

                }
            }
        }
    }

    public class UserActivityItem
    {
        public int MinutesSinceLastAction { get; set; }
        public int TotalMinutesSinceLastAction { get; set; }
        public int SecondsSinceLastAction { get; set; }

        public DateTime TimeOfLastAction { get; set; }

        public string UserName { get; set; }
    }
}