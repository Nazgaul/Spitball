using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Zbang.Zbox.Infrastructure.Trace;


namespace Zbang.Zbox.Mvc3WebRole.App_Start.AzureCdn
{
    public class CdnHelpersContext
    {
        public const string CdnDir = "~/cdn";
        const string CssExtension = ".css";
        const string JsExtension = ".js";

        readonly CdnHelpersConfig m_Configuration = new CdnHelpersConfig();
        static readonly CdnHelpersContext Instance = new CdnHelpersContext();

        readonly Dictionary<string, string> m_KeyCompress = new Dictionary<string, string>();
        static CdnHelpersContext()
        {
        }

        CdnHelpersContext()
        {

        }

        private bool ShouldNeedToGenerateCache()
        {
            var currentHash = string.Empty;
            var dataFilePath = Path.Combine(CdnPhysicalPath, "v.dat");
            if (File.Exists(dataFilePath))
            {
                using (var sr = File.OpenText(dataFilePath))
                {
                    currentHash = sr.ReadLine();
                }
            }
            return currentHash != m_Configuration.HashKey;
        }
        private void DeleteFiles(string baseDirectory)
        {
            var directories = Directory.GetDirectories(baseDirectory);
            foreach (var dir in directories)
            {
                try
                {
                    Directory.Delete(dir, true);
                }
                catch (Exception ex)
                {
                    TraceLog.WriteError("Cannot delete directory " + dir, ex);
                }
            }
            var files = Directory.GetFiles(baseDirectory);//.Where(w => w.ToLower() !=);
            foreach (var file in files)
            {
                try
                {
                    if (file.ToLower().Contains("web.config"))
                    {
                        continue;
                    }
                    File.Delete(file);
                }
                catch (Exception ex)
                {
                    TraceLog.WriteError("Cannot delete file " + file, ex);
                }
            }
        }

        public static CdnHelpersContext Current
        {
            get
            {
                return Instance;
            }
        }
        public CdnHelpersConfig Configuration { get { return m_Configuration; } }

        public void Configure(Action<CdnHelpersConfig> configure)
        {
            configure.Invoke(m_Configuration);
            if (ShouldNeedToGenerateCache())
            {
                DeleteFiles(HttpContext.Current.Server.MapPath(CdnDir));

                var server = HttpContext.Current.Server;
                var appRoot = server.MapPath("~");
                var targetPath = CdnPhysicalPath;

                foreach (var folder in m_Configuration.CdnFolders)
                {
                    var physicalPath = server.MapPath(folder);
                    var pathToConcate = physicalPath.Replace(appRoot, string.Empty);

                    //var combined = Path.Combine(targetPath, path);
                    CopyDirectory(new DirectoryInfo(physicalPath), new DirectoryInfo(Path.Combine(targetPath, pathToConcate)));
                }
                using (StreamWriter sw = File.CreateText(Path.Combine(targetPath, "v.dat")))
                {
                    sw.WriteLine(m_Configuration.HashKey);
                }
            }

        }
        private string CdnPhysicalPath
        {
            get
            {
                return HttpContext.Current.Server.MapPath(CdnDir);
            }
        }
        public string GetCssRelativePath(string key)
        {
            var fileName = GetValueFromKey(key);
            if (m_Configuration.DebuggingEnabled.Invoke())
            {
                return VirtualPathUtility.AppendTrailingSlash(CdnDir) + fileName + CssExtension;
            }
            return VirtualPathUtility.AppendTrailingSlash(m_Configuration.CdnEndointUrl) + fileName + CssExtension;
        }
        public string GetJsRelativePath(string key)
        {
            var fileName = GetValueFromKey(key);
            if (m_Configuration.DebuggingEnabled.Invoke())
            {
                return VirtualPathUtility.AppendTrailingSlash(CdnDir) + fileName + JsExtension;
            }
            return VirtualPathUtility.AppendTrailingSlash(m_Configuration.CdnEndointUrl) + fileName + JsExtension;
        }
        private string GetValueFromKey(string key)
        {
            string value;
            if (m_KeyCompress.TryGetValue(key, out value))
            {
                return value;
            }
            value = GetMd5Hash(string.Format("{0}{1}{2}", m_Configuration.HashKey, key));
            return value;
        }
        private void AddToKeyCompress(string key, string value)
        {
            m_KeyCompress.Add(key, value);
        }
        //public string GetSpriteCssRelativePath(string virtualPath);
        public void RegisterCombinedCssFiles(string name, params string[] files)
        {
            var fileName = GetMd5Hash(string.Format("{0}{1}", m_Configuration.HashKey, name));
            var filePath = Path.Combine(CdnPhysicalPath, fileName + CssExtension);
            if (!File.Exists(filePath) || ShouldNeedToGenerateCache())
            {
                var cssCompress = CompressAndCombineCssJsFiles.CombineCssFiles(
                     files.Select(s => HttpContext.Current.Server.MapPath(s)));
                File.WriteAllText(filePath, cssCompress);
            }
            AddToKeyCompress(name, fileName);
        }

        public void RegisterCombinedJsFiles(string name, params string[] files)
        {
            var fileName = GetMd5Hash(string.Format("{0}{1}", m_Configuration.HashKey, name));
            string filePath = Path.Combine(CdnPhysicalPath, fileName + JsExtension);
            if (!File.Exists(filePath) || ShouldNeedToGenerateCache())
            {
                var jsCompress = CompressAndCombineCssJsFiles.CombineJsFiles(
                    files.Select(s => HttpContext.Current.Server.MapPath(s)));

                File.WriteAllText(filePath, jsCompress);
            }
            AddToKeyCompress(name, fileName);
        }

        string GetMd5Hash(string input)
        {
            using (var md5Hash = MD5.Create())
            {
                // Convert the input string to a byte array and compute the hash.
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

                // Create a new Stringbuilder to collect the bytes
                // and create a string.
                var sBuilder = new StringBuilder();

                // Loop through each byte of the hashed data 
                // and format each one as a hexadecimal string.
                for (var i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }

                // Return the hexadecimal string.
                return sBuilder.ToString();
            }
        }

        void CopyDirectory(DirectoryInfo source, DirectoryInfo destination)
        {
            if (!destination.Exists)
            {
                destination.Create();
            }

            // Copy all files.
            var files = source.GetFiles();
            foreach (var file in files)
            {

                file.CopyTo(Path.Combine(destination.FullName,
                    file.Name));
            }

            // Process subdirectories.
            var dirs = source.GetDirectories();//.Where(w => w.Name != ".svn"); ;
            foreach (var dir in dirs)
            {
                if (dir.Name.ToLower() == ".svn")
                {
                    continue;
                }
                // Get destination directory.
                var destinationDir = Path.Combine(destination.FullName, dir.Name);

                // Call CopyDirectory() recursively.
                CopyDirectory(dir, new DirectoryInfo(destinationDir));
            }
        }

    }
}