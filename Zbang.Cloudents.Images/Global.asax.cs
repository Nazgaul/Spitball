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
                    var splitUrl = args.VirtualPath.Split(new[] {"/"}, StringSplitOptions.RemoveEmptyEntries);
                    
                    if (splitUrl.Any(f => f.ToLower().StartsWith("http")))
                    {
                        args.VirtualPath = "preview";
                        args.QueryString["404"] = "~/images/link_720.png";
                        return;
                    }

                    var blobName = splitUrl.Last();
                    if (string.IsNullOrEmpty(blobName))
                    {
                        return;
                    }
                    //var fileProcessorFactory = container.Resolve<IFileProcessorFactory>();
                    var blobWithOriginalFileName = Path.GetFileNameWithoutExtension(blobName);
                    var path = Path.GetExtension(blobWithOriginalFileName).ToLower();
                    if (WordProcessor.WordExtensions.Contains(path))
                    {
                        args.QueryString["404"] = "~/images/Icons_720_doc.png";
                        return;
                    }
                    if (ExcelProcessor.ExcelExtensions.Contains(path))
                    {
                        args.QueryString["404"] = "~/images/Icons_720_excel.png";
                        return;
                    }
                    if (ImageProcessor.ImageExtensions.Contains(path))
                    {
                        args.QueryString["404"] = "~/images/Icons_720_image.png";
                        return;
                    }
                    if (PdfProcessor.PdfExtensions.Contains(path))
                    {
                        args.QueryString["404"] = "~/images/Icons_720_pdf.png";
                        return;
                    }
                    if (PowerPoint2007Processor.PowerPoint2007Extensions.Contains(path))
                    {
                        args.QueryString["404"] = "~/images/Icons_720_power.png";
                        return;
                    }
                    if (TiffProcessor.TiffExtensions.Contains(path))
                    {
                        args.QueryString["404"] = "~/images/Icons_720_image.png";
                        return;
                    }
                    if (VideoProcessor.VideoExtensions.Contains(path))
                    {
                        args.QueryString["404"] = "~/images/Icons_720_video.png";
                        return;
                    }
                    if (TextProcessor.TxtExtensions.Contains(path))
                    {
                        args.QueryString["404"] = "~/images/Icons_720_txt.png";
                        return;
                    }
                    if (AudioProcessor.AudioExtensions.Contains(path))
                    {
                        args.QueryString["404"] = "~/images/Icons_720_sound.png";
                        return;
                    }
                   
                    args.QueryString["404"] = "~/images/Icons_720_generic.png";
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