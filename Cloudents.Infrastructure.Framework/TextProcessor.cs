using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Infrastructure.Framework
{
    public class TextProcessor : IPreviewProvider
    {




        private string _text;
        public void Init(Func<Stream> stream)
        {
            using (var sr2 = stream())
            {
                using (var sr = new StreamReader(sr2, GetEncoding(sr2)))
                {
                    _text = sr.ReadToEnd();
                }
            }
        }

        public void Init(Func<string> stream)
        {
            _text = File.ReadAllText(stream());
        }

        private static Encoding GetEncoding(Stream sr)
        {
            // Read the BOM
            var bom = new byte[4];
            sr.Read(bom, 0, 4);

            sr.Seek(0, SeekOrigin.Begin);
            // Analyze the BOM
            if (bom[0] == 0x2b && bom[1] == 0x2f && bom[2] == 0x76) return Encoding.UTF7;
            if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf) return Encoding.UTF8;
            if (bom[0] == 0xff && bom[1] == 0xfe) return Encoding.Unicode; //UTF-16LE
            if (bom[0] == 0xfe && bom[1] == 0xff) return Encoding.BigEndianUnicode; //UTF-16BE
            if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff) return Encoding.UTF32;
            return Encoding.ASCII;
        }

        public (string text, int pagesCount) ExtractMetaContent()
        {
            return (_text, 1);
        }

        public async Task ProcessFilesAsync(IEnumerable<int> previewDelta, Func<Stream, string, Task> pagePreviewCallback, CancellationToken token)
        {
            var enumerable = previewDelta.ToList();

            //first, create a dummy bitmap just to get a graphics object
            using (Image img = new Bitmap(1, 1))
            {
                using (var drawing = Graphics.FromImage(img))
                using (var font = new Font(FontFamily.GenericSansSerif, 12))
                {
                    var i = 0;
                    foreach (var splitInPart in SplitInParts(_text, 1000))
                    {
                        if (enumerable.Contains(i))
                        {
                            continue;
                        }
                        var textSize = drawing.MeasureString(splitInPart, font);
                        using (var img2 = new Bitmap((int)textSize.Width, (int)textSize.Height))
                        {

                            using (var drawing2 = Graphics.FromImage(img2))
                            {

                                //paint the background
                                drawing2.Clear(Color.White);

                                //create a brush for the text
                                using (Brush textBrush = new SolidBrush(Color.Black))
                                {

                                    drawing2.DrawString(splitInPart, font, textBrush, 0, 0);

                                    drawing2.Save();
                                }

                            }

                            using (var ms = new MemoryStream())
                            {
                                img2.Save(ms, ImageFormat.Jpeg);
                                await pagePreviewCallback(ms, $"{i}.jpg");
                            }

                            i++;
                        }
                    }
                }
            }


        }


        private static IEnumerable<string> SplitInParts(string s, int partLength)
        {
            if (s == null)
                throw new ArgumentNullException(nameof(s));
            if (partLength <= 0)
                throw new ArgumentException("Part length has to be positive.", nameof(partLength));

            for (var i = 0; i < s.Length; i += partLength)
                yield return s.Substring(i, Math.Min(partLength, s.Length - i));
        }

        //public void Init(Func<string> path)
        //{
        //    _text = File.ReadAllText(path());// sr.ReadToEnd();
        //}
    }
}
