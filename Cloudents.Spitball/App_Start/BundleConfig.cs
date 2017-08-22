﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace Cloudents.Spitball.App_Start
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.UseCdn = true;


            bundles.Add(new ScriptBundle("~/bundles/angularjs",
                "https://ajax.googleapis.com/ajax/libs/angularjs/1.6.4/angular.min.js").Include(
                "~/Scripts/angular.js"));


            var lessBundle = new Bundle("~/My/Css")
                .Include("~/App_Themes/site.css");
            bundles.Add(lessBundle);
        }
    }
    public class LessTransform : IBundleTransform
    {
        public void Process(BundleContext context, BundleResponse response)
        {
            response.Content = dotless.Core.Less.Parse(response.Content);
            response.ContentType = "text/css";
        }
    }
}