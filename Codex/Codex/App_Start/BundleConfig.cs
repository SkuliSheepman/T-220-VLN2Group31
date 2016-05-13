using System.Web;
using System.Web.Optimization;

namespace Codex
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/materialize").Include(
                      "~/Scripts/materialize/materialize.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/admin").Include(
                      "~/Scripts/codex.js",
                      "~/Scripts/admin.js"));

            bundles.Add(new ScriptBundle("~/bundles/teacher").Include(
                      "~/Scripts/codex.js",
                      "~/Scripts/teacher.js"));

            bundles.Add(new ScriptBundle("~/bundles/student").Include(
                      "~/Scripts/codex.js",
                      "~/Scripts/student.js"));

            bundles.Add(new ScriptBundle("~/bundles/codex").Include(
                      "~/Scripts/codex.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/materialize/css/materialize.css",
                      "~/Content/site.css"));
        }
    }
}
