using System;
using System.Collections.Generic;
using Zbang.Zbox.Domain.Common;

namespace Zbang.Zbox.Mvc3WebRole.App_Start.AzureCdn
{
    public class CdnHelpersConfig
    {
        private string m_CdnEndointUrl;
        public List<string> CdnFolders { get; set; }

        public CdnHelpersConfig()
        {
            CdnFolders = new List<string>();
        }
        public string CdnEndointUrl
        {
            get
            {
                return m_CdnEndointUrl;
            }
            set
            {
                var url = value;
                if (Validation.IsUrlWithoutSceme(url))
                {
                    url = string.Format("https://{0}", url);
                }
                m_CdnEndointUrl = url;
            }
        }
        public Func<bool> DebuggingEnabled { get; set; }
        public string HashKey { get; set; }
       
        //public string StorageFolderName { get; set; }

        //public void EnableImageOptimizations();
        public void UseCdnForContentFolder()
        {
            UseCdnForFolder("~/Content");
        }
        public void UseCdnForScriptsFolder()
        {
            UseCdnForFolder("~/Scripts");
        }
        public void UseCdnForFolder(string virtualPath)
        {
            CdnFolders.Add(virtualPath);

            

        }

    }
}