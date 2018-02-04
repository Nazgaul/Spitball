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
        private readonly IRepository<File> _itemRepository;

        public UpdateThumbnailCommandHandler(IRepository<File> itemRepository)
        {
            _itemRepository = itemRepository;
        }

        public void Handle(UpdateThumbnailCommand command)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));

            var file = _itemRepository.Load(command.ItemId);
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
            _itemRepository.Save(file);
        }
    }
}
