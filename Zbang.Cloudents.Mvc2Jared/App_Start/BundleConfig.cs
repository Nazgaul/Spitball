using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace Zbang.Cloudents.Mvc2Jared
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            IItemTransform cssFixer = new CssRewriteUrlTransform();

            //bundles.Add(new ScriptBundle("Home").Include(
            //            "~/Scripts/jquery-{version}.js"));

            //...
        }
    }
}