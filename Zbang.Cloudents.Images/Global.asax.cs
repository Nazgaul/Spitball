using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using Zbang.Zbox.Infrastructure.File;
using Zbang.Zbox.Infrastructure.Ioc;
using Zbang.Zbox.Infrastructure.Storage;

namespace Zbang.Cloudents.Images
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {


            ImageResizer.Configuration.Config.Current.Pipeline.RewriteDefaults +=

                delegate(IHttpModule m, HttpContext c, ImageResizer.Configuration.IUrlEventArgs args)
                {
                    var blobName = args.VirtualPath.Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries).Last();
                    if (string.IsNullOrEmpty(blobName))
                    {
                        return;
                    }
                    //var fileProcessorFactory = container.Resolve<IFileProcessorFactory>();
                    var path = Path.GetExtension(blobName).ToLower();
                    if (WordProcessor.WordExtensions.Contains(path))
                    {
                        args.QueryString["404"] = "~/images/wordv4.jpg";
                    }
                    if (ExcelProcessor.ExcelExtensions.Contains(path))
                    {
                        args.QueryString["404"] = "~/images/excelv4.jpg";
                    }
                    if (ImageProcessor.ImageExtensions.Contains(path))
                    {
                        args.QueryString["404"] = "~/images/imagev4.jpg";
                    }
                    if (PdfProcessor.PdfExtensions.Contains(path))
                    {
                        args.QueryString["404"] = "~/images/pdfv4.jpg";
                    }
                    if (PowerPoint2007Processor.PowerPoint2007Extensions.Contains(path))
                    {
                        args.QueryString["404"] = "~/images/powerv4.jpg";
                    }
                    if (TiffProcessor.TiffExtensions.Contains(path))
                    {
                        args.QueryString["404"] = "~/images/imagev4.jpg";
                    }
                    if (VideoProcessor.VideoExtensions.Contains(path))
                    {
                        args.QueryString["404"] = "~/images/videov4.jpg";
                    }
                    if (TextProcessor.TxtExtensions.Contains(path))
                    {
                        args.QueryString["404"] = "~/images/textv1.jpg";
                    }
                    if (AudioProcessor.AudioExtensions.Contains(path))
                    {
                        args.QueryString["404"] = "~/images/soundv4.jpg";
                    }
                    args.QueryString["404"] = "~/images/filev4.jpg";
                };
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}