﻿using Cloudents.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;

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
                Directory.GetFiles($@"{directoryName}\Cloudents.Web\ClientApp\src\locales\",
                "*.json", SearchOption.AllDirectories);
            string[] jsFiles = Directory.GetFiles($@"{directoryName}\Cloudents.Web\ClientApp\src",
                "*", SearchOption.AllDirectories);

            for (int j = resourceFiles.Length - 1; j >= 0; j--)
            {
                var resourceFile = resourceFiles[j];




                string content = File.ReadAllText(resourceFile);
                //using (StreamReader streamReader = new StreamReader(resourceFile, Encoding.UTF8))
                //{
                //    content = streamReader.ReadToEnd();
                //}

                var values = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
              

                foreach (var (key, value) in values)
                {
                    
                

                //foreach (XmlNode element in dataElement)
                //for (int i = values.Count - 1; i >= 0; i--)
                //{
                   // var element = values[i];
                    var occurrence = false;
                    //foreach (var elementChildNode in element.ChildNodes)
                    //{
                    //    if (elementChildNode is XmlElement p)
                    //    {
                    //        if (p.InnerText == "dynamic")
                    //        {
                    //            occurrence = true;
                    //            break;
                    //        }
                    //    }
                    //}

                    //if (occurrence)
                    //{
                    //    break;
                    //}

                   // var name = dataElement[i].Attributes["name"].Value;

                   // var resourceString = $"{Path.GetFileNameWithoutExtension(resourceFile).Split('.')[0]}_{name}";

                    foreach (string file in jsFiles)
                    {
                        var path = Path.GetExtension(file);
                        var extensionsNotToSearch = new string[]
                        {
                            ".less", ".json", ".css"
                        };
                        if (extensionsNotToSearch.Contains(path, StringComparer.OrdinalIgnoreCase))
                        {
                            continue;
                        }
                        if (!FileContentCache.TryGetValue(file, out var lines))
                        {
                            lines = File.ReadAllLines(file);
                            FileContentCache[file] = lines;
                        }
                        //string[] lines = File.ReadAllLines(file); 
                        occurrence = lines.Any(l => l.Contains(key, StringComparison.OrdinalIgnoreCase));
                        if (occurrence)
                        {
                            break;
                        }
                    }
                    if (!occurrence)
                    {
                        Console.WriteLine($"file path: {key}");
                        values.Remove(key);
                        //var p = dataElement[i].ParentNode;
                        //p.RemoveChild(dataElement[i]);
                    }
                }

                var x = JsonConvert.SerializeObject(values, Formatting.Indented);
                File.WriteAllText(resourceFile,x);
                //using (var streamReader = new StreamWriter(resourceFile, Encoding.UTF8))
                //{
                //    content = streamReader.ReadToEnd();
                //}

                //xmlDoc.Save(resourceFile);
                //dataElement = xmlDoc.GetElementsByTagName("data");
                //if (dataElement.Count == 0)
                //{
                //    var file = new FileInfo(resourceFile);
                //    file.Delete();
                //    Console.WriteLine($"Deleting: {resourceFile}");
                //    //Need to remove the file
                //}
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
                Directory.GetFiles($@"{directoryName}\Cloudents.Web\ClientApp\src\font-icon\",
                "*.*", SearchOption.AllDirectories);
            string[] jsFiles = Directory.GetFiles($@"{directoryName}\Cloudents.Web\ClientApp\src\",
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
