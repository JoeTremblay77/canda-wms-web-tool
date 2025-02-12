using System.Web.Mvc;

namespace CandaWebUtility
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());

            // this is what puts security on every controller by default and you have to explicitly allow anon
            filters.Add(new System.Web.Mvc.AuthorizeAttribute());
        }
    }
}