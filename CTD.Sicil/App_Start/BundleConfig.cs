using System.Web.Optimization;

namespace CTD.Sicil
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/bootstrap.min.css", "~/Content/animate.css",
                "~/Content/Site.css"));
            bundles.Add(new StyleBundle("~/font-awesome/css").Include("~/Content/font-awesome.min.css",
                new CssRewriteUrlTransform()));
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include("~/Scripts/jquery-{version}.js"));
            bundles.Add(
                new ScriptBundle("~/Content/themes/base/jqueryuiStyles").Include(
                    "~/Content/themes/base/jquery-ui.min.css"));
            bundles.Add(new StyleBundle("~/bundles/jqueryui").Include("~/Scripts/jquery-ui-{version}.min.js"));
            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include("~/Scripts/bootstrap.min.js"));
            bundles.Add(new ScriptBundle("~/bundles/inspinia").Include("~/Scripts/plugins/metisMenu/metisMenu.min.js",
                "~/Scripts/plugins/pace/pace.min.js", "~/Scripts/inspinia.js"));
            bundles.Add(new ScriptBundle("~/bundles/skinConfig").Include("~/Scripts/skin.config.min.js"));
            bundles.Add(
                new ScriptBundle("~/plugins/slimScroll").Include(
                    "~/Scripts/plugins/slimscroll/jquery.slimscroll.min.js"));
        }
    }
}