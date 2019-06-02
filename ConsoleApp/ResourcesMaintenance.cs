using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace ConsoleApp
{
    public class ResourcesMaintenance
    {
        public static void GetOrphanedResources()
        {
            //TODO: :// we need to think about landing page and other instances of resources string interpolation!
            //TODO :// maybe some performance tweaks and thats it.
            Console.WriteLine(Directory.GetCurrentDirectory());
            
            var dir = Directory.GetCurrentDirectory();
            var s = Directory.GetParent(dir);
            while (Path.GetFileName(s.ToString()) != "Zbox")
            {
                s = Directory.GetParent(s.ToString());
            }
            string[] files =
                Directory.GetFiles($@"{s}\Cloudents.Web\Resources\Js\",
                "*.resx", SearchOption.AllDirectories);
            string[] jsFiles = Directory.GetFiles($@"{s}\Cloudents.Web\ClientApp",
                "*", SearchOption.AllDirectories);
            
            //var dic = new Dictionary<string,string[]>();
            foreach (var f in files)
            {
                string content;
                using (StreamReader streamReader = new StreamReader(f, Encoding.UTF8))
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
                        //if (!dic.TryGetValue(file, out var lines))
                        //{
                        //    lines = File.ReadAllLines(file);
                        //    dic[file] = lines;
                        //}
                        string[] lines = File.ReadAllLines(file); 
                        firstOccurrence = lines.FirstOrDefault(l => l.Contains(name));
                        if (!string.IsNullOrEmpty(firstOccurrence))
                        {
                            break;
                        }
                    }
                    if (string.IsNullOrEmpty(firstOccurrence))
                    {
                        Console.WriteLine($"file path: {f}");
                        Console.WriteLine($"element name: {name}");
                        Console.WriteLine("-----------------------");

                        var p = dataElement[i].ParentNode;
                        p.RemoveChild(dataElement[i]);
                    }
                }
                xmlDoc.Save(f);
            }
        }
    }
}
