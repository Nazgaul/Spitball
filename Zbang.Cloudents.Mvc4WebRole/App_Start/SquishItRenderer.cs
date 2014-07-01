using System;
using System.IO;
using System.Text;
using Zbang.Zbox.Infrastructure.Storage;

namespace Zbang.Cloudents.Mvc4WebRole
{
    public class SquishItRenderer : SquishIt.Framework.Renderers.IRenderer
    {
        public void Render(string content, string outputPath)
        {
            var compress = new Compress();
            var bytes = compress.CompressToGzip(Encoding.UTF8.GetBytes(content));
            var dir = Path.GetDirectoryName(outputPath);
            if (dir == null)
            {
                throw new NullReferenceException("directory");
            }
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            File.WriteAllBytes(outputPath, bytes);
        }
    }
}