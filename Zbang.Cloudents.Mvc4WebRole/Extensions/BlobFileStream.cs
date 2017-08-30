using System.Globalization;
using System.Web;
using System.Web.Mvc;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Zbang.Cloudents.Mvc4WebRole.Extensions
{
    public class BlobFileStream : FileResult
    {
        readonly CloudBlockBlob m_Blob;
        readonly string m_FileName;
        readonly bool m_IsDownload;

        public BlobFileStream(CloudBlockBlob blob, string contentType, string fileName, bool isDownload)
            : base(contentType)
        {
            m_Blob = blob;
            m_FileName = fileName;
            m_IsDownload = isDownload;
        }

        protected override void WriteFile(HttpResponseBase response)
        {
            response.Clear();
            if (m_IsDownload)
            {
                string headerValue = ContentDispositionUtil.GetHeaderValue(m_FileName);
                response.AddHeader("Content-Disposition", headerValue);

                //response.AddHeader("Content-Disposition", "attachment; filename="
                //    + HttpUtility.UrlEncode(m_FileName.Replace(" ", "_").Replace(",", "_"),System.Text.Encoding.UTF8));
                    //+ HttpUtility.UrlPathEncode(m_FileName.Replace(" ", "_").Replace(",", "_"))); // for chrome        
            }
            else
            {
                response.AddHeader("Content-Disposition", "inline; filename=" + HttpUtility.UrlPathEncode(m_FileName).Replace(" ", "_")
                                                                                               .Replace(",", "_")); // for chrome        
            }


            m_Blob.FetchAttributes();
            response.AddHeader("Content-Length", m_Blob.Properties.Length.ToString(CultureInfo.InvariantCulture));
            //response.AddHeader("Content-Encoding", "gzip");
            m_Blob.DownloadToStream(response.OutputStream);
        }
    }
}



