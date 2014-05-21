using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Zbox.Infrastructure.Thumbnail
{
    public class ThumbnailProvider : IThumbnailProvider, IAcademicBoxThumbnailProvider
    {
        //const string UnknownFile = "unknown";
        //const string DefaultImageTypePicture = "imagev1.jpg";
        public const string DefaultFileTypePicture = "filev4.jpg";
        public const string ExcelFileTypePicture = "excelv4.jpg";
        public const string ImageFileTypePicture = "imagev4.jpg";
        public const string PdfFileTypePicture = "pdfv4.jpg";
        public const string PowerPointFileTypePicture = "powerv4.jpg";
        public const string SoundFileTypePicture = "soundv4.jpg";
        public const string VideoFileTypePicture = "videov4.jpg";
        public const string WordFileTypePicture = "wordv4.jpg";
        public const string LinkTypePicture = "linkv2.jpg";

        private readonly List<string> m_DefaultPictures = new List<string>
        {
            DefaultFileTypePicture,
            ExcelFileTypePicture,
            ImageFileTypePicture,
            PdfFileTypePicture,
            PowerPointFileTypePicture,
            SoundFileTypePicture,
            VideoFileTypePicture,
            WordFileTypePicture,
            LinkTypePicture
        };

        private readonly IBlobProvider m_BlobProvider;

        public ThumbnailProvider(IBlobProvider blobProvider/*, IFileConvertFactory fileConvertor*/)
        {
            m_BlobProvider = blobProvider;
           // InitializeBlob();
        }

        //private void InitializeThumbnailMapper()
        //{
        //    m_ThumbnailMapper = new Dictionary<string, string>
        //                            {
        //                                {".tif", DefaultImageTypePicture},
        //                                {".tiff", DefaultImageTypePicture},
        //                                {".bmp", DefaultImageTypePicture},
        //                                {".gif", DefaultImageTypePicture},
        //                                {".png", DefaultImageTypePicture},
        //                                {".tga", DefaultImageTypePicture},
        //                                {".jpg", DefaultImageTypePicture},
        //                                {".jpeg", DefaultImageTypePicture},
        //                                {".pdf", "pdfv1.jpg"},
        //                                {".doc", "wordv1.jpg"},
        //                                {".docx", "wordv1.jpg"},
        //                                {".ppt", "powerpointv1.jpg"},
        //                                {".pptx", "powerpointv1.jpg"},
        //                                {".xls", "excelv1.jpg"},
        //                                {".xlsx", "excelv1.jpg"},
        //                                {".aac", "musicv1.jpg"},
        //                                {".aif", "musicv1.jpg"},
        //                                {".iff", "musicv1.jpg"},
        //                                {".m3u", "musicv1.jpg"},
        //                                {".m4a", "musicv1.jpg"},
        //                                {".mid", "musicv1.jpg"},
        //                                {".mp3", "musicv1.jpg"},
        //                                {".mpa", "musicv1.jpg"},
        //                                {".ra", "musicv1.jpg"},
        //                                {".wav", "musicv1.jpg"},
        //                                {".wma", "musicv1.jpg"},
        //                                {".avi", "videov1.jpg"},
        //                                {".3g2", "videov1.jpg"},
        //                                {".3gp", "videov1.jpg"},
        //                                {".asf", "videov1.jpg"},
        //                                {".asx", "videov1.jpg"},
        //                                {".flv", "videov1.jpg"},
        //                                {".mov", "videov1.jpg"},
        //                                {".mp4", "videov1.jpg"},
        //                                {".mpg", "videov1.jpg"},
        //                                {".rm", "videov1.jpg"},
        //                                {".swf", "videov1.jpg"},
        //                                {".vob", "videov1.jpg"},
        //                                {".wmv", "videov1.jpg"},
        //                                {".ogg", "videov1.jpg"},
        //                                {".ogv", "videov1.jpg"},
        //                                {".NoExtension","linkv2.jpg"},
        //                                {UnknownFile, DefaultFileTypePicture}
        //                            };
        //}

        #region initStorage
        private async void InitializeBlob()
        {
            var tasks = new List<Task>();
            foreach (var thumbnail in m_DefaultPictures)
            {
                if (m_BlobProvider.CheckIfFileThumbnailExists(thumbnail))
                {
                    continue;
                }
                var resourceStream = ReadResource(thumbnail);
                if (resourceStream == null) continue;
                tasks.Add(m_BlobProvider.UploadFileThumbnailAsync(thumbnail, resourceStream, "image/jpg"));
            }
            await Task.WhenAll(tasks);
            
            InitializeAcademicBoxThumbnails();
        }

        public const int NumberOfAcademicBoxThumbnail = 10;
        private void InitializeAcademicBoxThumbnails()
        {
            for (int i = 0; i < NumberOfAcademicBoxThumbnail; i++)
            {
                var fileName = "AcademicBox_" + (i + 1) + "V3.png";
                if (m_BlobProvider.CheckIfFileThumbnailExists(fileName))
                {
                    continue;
                }
                var resourceStream = ReadResource("AcademicBox." + (i + 1) + ".png");
                if (resourceStream == null) continue;
                m_BlobProvider.UploadFileThumbnail(fileName, resourceStream, "image/png");
            }
        }

        #endregion
        public string GetAcademicBoxThumbnail()
        {
            var rand = RandomProvider.GetThreadRandom();
            var pos = rand.Next(NumberOfAcademicBoxThumbnail);
            return "AcademicBox_" + (pos + 1) + "V3.png";
        }

        private Stream ReadResource(string resource)
        {
            var assembly = Assembly.GetExecutingAssembly();
            return assembly.GetManifestResourceStream(string.Format("Zbang.Zbox.Infrastructure.Thumbnail.Resources.{0}", resource));
        }


    }
}
