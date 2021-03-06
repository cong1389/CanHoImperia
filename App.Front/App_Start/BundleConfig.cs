﻿using System.Web.Optimization;

namespace App.Front
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

            bundles.Add(new ScriptBundle("~/bundles/jqueryform").Include(
                "~/Scripts/jquery.form*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            //fancybox
            bundles.Add(new ScriptBundle("~/bundles/Scriptsfancybox").Include(
                "~/Scripts/jquery.fancybox.*"));
            bundles.Add(new StyleBundle("~/bundles/Contentfancybox").Include(
                "~/Content/jquery.fancybox.*"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/bundles/Themes").Include(
                     "~/Themes/Basic/Content/theme.min.css"));

            // Code removed for clarity.
            BundleTable.EnableOptimizations = true;
        }
    }
}
