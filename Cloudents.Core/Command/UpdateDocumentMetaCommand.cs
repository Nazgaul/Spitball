using System.Globalization;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Command
{
    public class UpdateDocumentMetaCommand : ICommand
    {
        public UpdateDocumentMetaCommand(long id, 
            //CultureInfo language,
            int? pageCount)
        {
            Id = id;
           // Language = language;
            PageCount = pageCount;
        }

        public long Id { get; }

        //public CultureInfo Language { get; }

        public int? PageCount { get; }
    }
}