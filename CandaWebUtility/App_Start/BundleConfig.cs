using System.Web.Optimization;

namespace CandaWebUtility
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/" + CacheBusterSuffix.ScriptsSuffix + "/jquery-{version}.js"
                        , "~/" + CacheBusterSuffix.ScriptsSuffix + "/jquery.unobtrusive-ajax.js"
                        , "~/" + CacheBusterSuffix.ScriptsSuffix + "/jquery-ui-{version}.js"
                        , "~/" + CacheBusterSuffix.ScriptsSuffix + "/bootstrap-datepicker.js"
                        , "~/" + CacheBusterSuffix.ScriptsSuffix + "/gridmvc.js"
                        , "~/" + CacheBusterSuffix.ScriptsSuffix + "/helper.js"
                        , "~/" + CacheBusterSuffix.ScriptsSuffix + "/autoNumeric-1.9.25.js"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/" + CacheBusterSuffix.ScriptsSuffix + "/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/" + CacheBusterSuffix.ScriptsSuffix + "/bootstrap.js",
                      "~/" + CacheBusterSuffix.ScriptsSuffix + "/respond.js"));

            bundles.Add(new StyleBundle("~/content/css").Include(
                      "~/" + CacheBusterSuffix.ContentsSuffix + "/bootstrap.css"
                      , "~/" + CacheBusterSuffix.ContentsSuffix + "/bootstrap-datepicker3.css"
                      , "~/" + CacheBusterSuffix.ContentsSuffix + "/Gridmvc.css"
                      , "~/" + CacheBusterSuffix.ContentsSuffix + "/gridmvc.datepicker.css"
                      , "~/" + CacheBusterSuffix.ContentsSuffix + "/site.css"
                      , "~/" + CacheBusterSuffix.ContentsSuffix + "/client.css"));
        }
    }
}