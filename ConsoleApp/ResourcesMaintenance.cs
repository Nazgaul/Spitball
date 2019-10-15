using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace ConsoleApp
{
    public static class ResourcesMaintenance
    {
        private static readonly Dictionary<string, string[]> _fileContentCache = new Dictionary<string, string[]>();
        private static void DeleteUnusedResources()
        {
            //TODO: :// we need to think about landing page and other instances of resources string interpolation!
            //TODO :// maybe some performance tweaks and thats it.
            Console.WriteLine(Directory.GetCurrentDirectory());

            var directoryName = Directory.GetCurrentDirectory();
            //var s = Directory.GetParent(directoryName);
            while (!Directory.GetFiles(directoryName, "*.sln").Any())
            {
                directoryName = Directory.GetParent(directoryName).ToString();
            }
            string[] resourceFiles =
                Directory.GetFiles($@"{directoryName}\Cloudents.Web\Resources\Js\",
                "*.resx", SearchOption.AllDirectories);
            string[] jsFiles = Directory.GetFiles($@"{directoryName}\Cloudents.Web\ClientApp",
                "*", SearchOption.AllDirectories);

            //var dic = new Dictionary<string,string[]>();
            for (int j = resourceFiles.Length - 1; j >= 0; j--)
            {
                var resourceFile = resourceFiles[j];
                string content;
                using (StreamReader streamReader = new StreamReader(resourceFile, Encoding.UTF8))
                {
                    content = streamReader.ReadToEnd();
                }
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(content);
                XmlNodeList dataElement = xmlDoc.GetElementsByTagName("data");


                //foreach (XmlNode element in dataElement)
                for (int i = dataElement.Count - 1; i >= 0; i--)
                {
                    var name = dataElement[i].Attributes["name"].Value;
                    string firstOccurrence = null;
                    foreach (string file in jsFiles)
                    {
                        if (!_fileContentCache.TryGetValue(file, out var lines))
                        {
                            lines = File.ReadAllLines(file);
                            _fileContentCache[file] = lines;
                        }
                        //string[] lines = File.ReadAllLines(file); 
                        firstOccurrence = lines.FirstOrDefault(l => l.Contains(name));
                        if (!string.IsNullOrEmpty(firstOccurrence))
                        {
                            break;
                        }
                    }
                    if (string.IsNullOrEmpty(firstOccurrence))
                    {
                        Console.WriteLine($"file path: {resourceFile}, element name: {name}");
                        Console.WriteLine($"element name: {name}");

                        var p = dataElement[i].ParentNode;
                        p.RemoveChild(dataElement[i]);
                    }
                }
                xmlDoc.Save(resourceFile);
                dataElement = xmlDoc.GetElementsByTagName("data");
                if (dataElement.Count == 0)
                {
                    var file = new FileInfo(resourceFile);
                    file.Delete();

                    //Need to remove the file
                }
            }
        }

        public static void DeleteStuffFromJs()
        {
            //RemoveComments();
            DeleteUnusedFontSvg();
            DeleteUnusedResources();
        }


        private static void RemoveComments()
        {

            var blocks = new Regex(@"\/\*[\s\S]*?\*\/|([^:]|^)\/\/.*$", RegexOptions.Multiline);
            Console.WriteLine("Delete unused svg");
            Console.WriteLine(Directory.GetCurrentDirectory());

            var directoryName = Directory.GetCurrentDirectory();
            //var s = Directory.GetParent(directoryName);
            while (!Directory.GetFiles(directoryName, "*.sln").Any())
            {
                directoryName = Directory.GetParent(directoryName).ToString();
            }


            string[] jsFiles = Directory.GetFiles($@"{directoryName}\Cloudents.Web\ClientApp",
                "*", SearchOption.AllDirectories);

            foreach (string file in jsFiles)
            {
                if (!_fileContentCache.TryGetValue(file, out var lines))
                {
                    lines = File.ReadAllLines(file);
                    _fileContentCache[file] = lines;
                }

                foreach (Match match in blocks.Matches(string.Join(Environment.NewLine,lines)))
                {
                    
                }
            }
        }

        private static void DeleteUnusedFontSvg()
        {
            //TODO: :// we need to think about landing page and other instances of resources string interpolation!
            //TODO :// maybe some performance tweaks and thats it.
            Console.WriteLine("Delete unused svg");
            Console.WriteLine(Directory.GetCurrentDirectory());

            var directoryName = Directory.GetCurrentDirectory();
            //var s = Directory.GetParent(directoryName);
            while (!Directory.GetFiles(directoryName, "*.sln").Any())
            {
                directoryName = Directory.GetParent(directoryName).ToString();
            }
            string[] svgFiles =
                Directory.GetFiles($@"{directoryName}\Cloudents.Web\ClientApp\font-icon\",
                "*.*", SearchOption.AllDirectories);
            string[] jsFiles = Directory.GetFiles($@"{directoryName}\Cloudents.Web\ClientApp",
                "*", SearchOption.AllDirectories);

            //var dic = new Dictionary<string,string[]>();
            for (int j = svgFiles.Length - 1; j >= 0; j--)
            {
                var svgFile = svgFiles[j];
                var svgFileInfo = new FileInfo(svgFile);
                var exists = false;
                foreach (string file in jsFiles)
                {
                    if (!_fileContentCache.TryGetValue(file, out var lines))
                    {
                        lines = File.ReadAllLines(file);
                        _fileContentCache[file] = lines;
                    }
                    //string[] lines = File.ReadAllLines(file); 
                    if (lines.Any(l => l.Contains($"sbf-{Path.GetFileNameWithoutExtension(svgFileInfo.Name)}")))
                    {
                        Console.WriteLine($"{svgFileInfo.Name} was found");
                        exists = true;
                        break;

                    }

                    if (lines.Any(l => l.Contains($"{svgFileInfo.Name}")))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"{svgFileInfo.Name} uses inline svg");
                        Console.ResetColor();
                        exists = true;
                        break;

                    }
                }


                if (!exists)
                {
                    svgFileInfo.Delete();
                }

                //Need to remove the file

            }
        }
    }
}
