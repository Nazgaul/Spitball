using Cloudents.Core;
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
        private static readonly Dictionary<string, string[]> FileContentCache = new Dictionary<string, string[]>();
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

            for (int j = resourceFiles.Length - 1; j >= 0; j--)
            {
                var resourceFile = resourceFiles[j];

                var v = Path.GetFileNameWithoutExtension(resourceFile).Contains('.');

                //ResXResourceReader rr = new ResXResourceReader(resourceFile);
                //rr.UseResXDataNodes = true;

                //IDictionaryEnumerator dict = rr.GetEnumerator();
                //while (dict.MoveNext())
                //{
                //    ResXDataNode node = (ResXDataNode)dict.Value;
                //    Console.WriteLine(string.Format("{0} {1} {2}",node.Name,node.GetValue((ITypeResolutionService)null),node.Comment));


                //    var resourceString = $"{Path.GetFileNameWithoutExtension(resourceFile).Split('.')[0]}_{node.Name}";
                //    var occurrence = false;
                //    foreach (string file in jsFiles)
                //    {
                //        if (!_fileContentCache.TryGetValue(file, out var lines))
                //        {
                //            lines = File.ReadAllLines(file);
                //            _fileContentCache[file] = lines;
                //        }
                //        //string[] lines = File.ReadAllLines(file); 
                //        occurrence = lines.Any(l => l.Contains(resourceString, StringComparison.OrdinalIgnoreCase));
                //        if (occurrence)
                //        {
                //            break;
                //        }
                //    }

                //    if (!occurrence)
                //    {
                //        Console.WriteLine($"file path: {resourceFile}, element name: {node.Name}");

                //        var p = dataElement[i].ParentNode;
                //        p.RemoveChild(dataElement[i]);
                //    }

                //}

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
                    var element = dataElement[i];
                    var occurrence = false;
                    foreach (var elementChildNode in element.ChildNodes)
                    {
                        if (elementChildNode is XmlElement p)
                        {
                            if (p.InnerText == "dynamic")
                            {
                                occurrence = true;
                                break;
                            }
                        }
                    }

                    if (occurrence)
                    {
                        break;
                    }

                    var name = dataElement[i].Attributes["name"].Value;

                    var resourceString = $"{Path.GetFileNameWithoutExtension(resourceFile).Split('.')[0]}_{name}";

                    foreach (string file in jsFiles)
                    {
                        if (!FileContentCache.TryGetValue(file, out var lines))
                        {
                            lines = File.ReadAllLines(file);
                            FileContentCache[file] = lines;
                        }
                        //string[] lines = File.ReadAllLines(file); 
                        occurrence = lines.Any(l => l.Contains(resourceString, StringComparison.OrdinalIgnoreCase));
                        if (occurrence)
                        {
                            break;
                        }
                    }
                    if (!occurrence)
                    {
                        Console.WriteLine($"file path: {resourceFile}, element name: {name}");

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
                    Console.WriteLine($"Deleting: {resourceFile}");
                    //Need to remove the file
                }
            }
        }

        public static void DeleteStuffFromJs()
        {
            //RemoveImages();
            //RemoveComments();
            DeleteUnusedResources();

            DeleteUnusedFontSvg();
        }

        private static void RemoveImages()
        {
            var directoryName = Directory.GetCurrentDirectory();
            //var s = Directory.GetParent(directoryName);
            while (!Directory.GetFiles(directoryName, "*.sln").Any())
            {
                directoryName = Directory.GetParent(directoryName).ToString();
            }

            var allFiles = Directory.GetFiles($@"{directoryName}\Cloudents.Web\ClientApp",
                "*", SearchOption.AllDirectories);


            var images = allFiles.Where(w =>
                FileTypesExtension.Image.Extensions.Contains(Path.GetExtension(w), StringComparer.OrdinalIgnoreCase));

            foreach (var imageFullPath in images)
            {
                var image = Path.GetFileName(imageFullPath);
                string firstOccurrence = null;
                foreach (string file in allFiles)
                {
                    if (!FileContentCache.TryGetValue(file, out var lines))
                    {
                        lines = File.ReadAllLines(file);
                        FileContentCache[file] = lines;
                    }
                    firstOccurrence = lines.FirstOrDefault(l => l.Contains(image));
                    if (!string.IsNullOrEmpty(firstOccurrence))
                    {
                        break;
                    }



                }

                if (string.IsNullOrEmpty(firstOccurrence))
                {
                    Console.WriteLine($"Deleting image {imageFullPath}");
                    File.Delete(imageFullPath);
                }
            }

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
                if (!FileContentCache.TryGetValue(file, out var lines))
                {
                    lines = File.ReadAllLines(file);
                    FileContentCache[file] = lines;
                }

                foreach (Match match in blocks.Matches(string.Join(Environment.NewLine, lines)))
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
                    if (!FileContentCache.TryGetValue(file, out var lines))
                    {
                        lines = File.ReadAllLines(file);
                        FileContentCache[file] = lines;
                    }
                    //string[] lines = File.ReadAllLines(file); 
                    if (lines.Any(l => l.Contains($"sbf-{Path.GetFileNameWithoutExtension(svgFileInfo.Name)}")))
                    {
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
