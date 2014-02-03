using System;
using System.Globalization;
using System.Web;
using System.Web.Mvc;
using Microsoft.WindowsAzure.StorageClient;
using System.IO;

public class BlobFileStream : FileResult
{
    readonly CloudBlob m_Blob;
    readonly string m_FileName;
    readonly bool m_IsDownload;

    public BlobFileStream(CloudBlob blob, string contentType, string fileName, bool isDownload)
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
            response.AddHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlPathEncode(m_FileName).Replace(" ", "_")
                .Replace(",", "_")); // for chrome        

        response.AddHeader("Content-Length", m_Blob.Properties.Length.ToString(CultureInfo.InvariantCulture));
        response.Cache.SetCacheability(HttpCacheability.Private);
        response.Cache.SetMaxAge(TimeSpan.FromDays(1));
        
        m_Blob.DownloadToStream(response.OutputStream);
    }
}



