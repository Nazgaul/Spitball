using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zbang.Zbox.Infrastructure.Storage;
using System.IO;

namespace Zbang.Zbox.Infrastructure.WebWorkerRoleJoinData.FileConvert
{
    public class FileConvertFactory : IFileConvertFactory
    {
        private Dictionary<string, ProductSelector> m_ConvertorMaps = new Dictionary<string, ProductSelector>
                                                                          {
                {"xls",ProductSelector.Cell },
                {"xlsx",ProductSelector.Cell},
                {"xlsm",ProductSelector.Cell},
                {"xltx",ProductSelector.Cell},
                {"ods",ProductSelector.Cell},
                {"csv",ProductSelector.Cell},
                {"rtf",ProductSelector.Words},
                {"docx",ProductSelector.Words},
                {"doc",ProductSelector.Words},
                {"ppt",ProductSelector.Slides},
                {"pot",ProductSelector.Slides},
                {"pps",ProductSelector.Slides},
                {"pptx",ProductSelector.Slides2007},
                {"potx",ProductSelector.Slides2007},
                {"ppxs",ProductSelector.Slides2007},
                {"ppsx",ProductSelector.Slides2007}
                ,{"pdf",ProductSelector.Pdf}
        };
        IBlobProvider m_BlobProvider;
        public FileConvertFactory(IBlobProvider blobProvider)
        {
            m_BlobProvider = blobProvider;
        }
        public bool CanConvertFile(string fileName)
        {
            var product = ChooseConvertor(fileName);
            return product != ProductSelector.None;

        }
        public Convertor GetConvertor(string fileName)
        {
            switch (ChooseConvertor(fileName))
            {
                case ProductSelector.Cell:
                    return new ExcelConvertor(m_BlobProvider, fileName);
                case ProductSelector.Words:
                    return new WordConvertor(m_BlobProvider, fileName);
                case ProductSelector.Slides:
                    return new PowerPointConvertor(m_BlobProvider, fileName);
                case ProductSelector.Slides2007:
                    return new PowerPoint2007Convertor(m_BlobProvider, fileName);
                case ProductSelector.Pdf:
                    return new PdfConvertor(m_BlobProvider, fileName);
                default:
                    throw new ArgumentException("Unfamiliar extension to convert");
            }
        }

        private ProductSelector ChooseConvertor(string fileName)
        {
            var product = ProductSelector.None;
            var extension = ExtractExtension(fileName);
            m_ConvertorMaps.TryGetValue(extension.ToLower(), out product);
            return product;
        }

        private string ExtractExtension(string fileName)
        {
            var extension = Path.GetExtension(fileName);
            if (extension.StartsWith("."))
            {
                extension = extension.Remove(0, 1);
            }
            return extension;
        }

        private enum ProductSelector
        {
            None, Cell, Words, Slides, Slides2007, Pdf
        }
    }


}
