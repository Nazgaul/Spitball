using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.StorageClient;
using System.Web.Mvc;

public class BlobFileStream : FileResult
{

    CloudBlob m_blob;
    string m_FileName;
    bool m_IsDownload;

    public BlobFileStream(CloudBlob blob, string contentType, string fileName, bool isDownload)
        : base(contentType)
    {
        m_blob = blob;
        m_FileName = fileName;
        m_IsDownload = isDownload;
    }

    protected override void WriteFile(HttpResponseBase response)
    {
        if (m_IsDownload)
            response.AddHeader("Content-Disposition", "attachment; filename=" + m_FileName.Replace(" ", "_"));
        response.AddHeader("Content-Length", m_blob.Properties.Length.ToString());
        response.Cache.SetExpires(DateTime.Now.AddMonths(1));
        response.Cache.SetValidUntilExpires(false);
        response.Cache.SetCacheability(HttpCacheability.Public);

        m_blob.DownloadToStream(response.OutputStream);
    }


}