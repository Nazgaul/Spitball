using System;
using System.IO;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class UpdateThumbnailCommandHandler : ICommandHandler<UpdateThumbnailCommand>
    {
        private readonly IRepository<File> m_ItemRepository;

        public UpdateThumbnailCommandHandler(IRepository<File> itemRepository)
        {
            m_ItemRepository = itemRepository;
        }
        public void Handle(UpdateThumbnailCommand command)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));

            var file = m_ItemRepository.Load(command.ItemId);
            if (!string.IsNullOrWhiteSpace(command.BlobName))
            {
                file.ItemContentUrl = command.BlobName;
                file.Name = Path.GetFileNameWithoutExtension(file.Name) + Path.GetExtension(command.BlobName);
            }
            if (!string.IsNullOrWhiteSpace(command.FileContent))
            {
                file.Content = System.Net.WebUtility.HtmlEncode(command.FileContent).RemoveEndOfString(200);
                // command.FileContent = null;
            }
            //else
            //{

            //}
            file.Md5 = command.Md5;
            m_ItemRepository.Save(file);
        }
    }
}
