﻿using System;
using System.IO;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.Storage;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class UpdateThumbnailCommandHandler : ICommandHandler<UpdateThumbnailCommand>
    {
        private readonly IRepository<File> m_ItemRepository;
        private readonly IRepository<Box> m_BoxRepository;
        private readonly IBlobProvider m_BlobProvider;

        public UpdateThumbnailCommandHandler(IRepository<File> itemRepository,
            IRepository<Box> boxRepository,
            IBlobProvider blobProvider)
        {
            m_ItemRepository = itemRepository;
            m_BoxRepository = boxRepository;
            m_BlobProvider = blobProvider;
        }
        public void Handle(UpdateThumbnailCommand command)
        {
            if (command == null) throw new ArgumentNullException("command");

            var file = m_ItemRepository.Get(command.ItemId);
            if (file == null)
            {
                throw new ArgumentException("file does not exisits");
            }
            if (file.ItemContentUrl != command.OldBlobName)
            {
                throw new ArgumentException("file id does not match blob id");
            }
            if (!string.IsNullOrWhiteSpace(command.BlobName))
            {
                file.ItemContentUrl = command.BlobName;
                file.Name = Path.GetFileNameWithoutExtension(file.Name) + Path.GetExtension(command.BlobName);
            }

            file.Content = command.FileContent;
            if (string.IsNullOrWhiteSpace(command.ThumbnailUrl))
            {
                 m_ItemRepository.Save(file);
                return;
            }
            //if (file.ThumbnailBlobName == Zbang.Zbox.Infrastructure.Thumbnail.ThumbnailProvider.DefaultFileTypePicture)
            //{
            //    UpdateThumbail(command, file);
            //    m_ItemRepository.Save(file);
            //    return;
            //}
            //if (file.ThumbnailBlobName.ToUpper().Contains("THUMBNAIL"))
            //{
            //    m_ItemRepository.Save(file);
            //    return;
            //}
            UpdateThumbail(command, file);
            m_ItemRepository.Save(file);

        }

        private void UpdateThumbail(UpdateThumbnailCommand command, File file)
        {
            var thumbnailUrl = m_BlobProvider.GetThumbnailUrl(command.ThumbnailUrl);
            file.UpdateThumbnail(command.ThumbnailUrl, thumbnailUrl);
            //file.ThumbnailBlobName = command.ThumbnailUrl;


            if (string.IsNullOrEmpty(file.Box.Picture))
            {
                file.Box.AddPicture(command.ThumbnailUrl, thumbnailUrl);
                m_BoxRepository.Save(file.Box);
            }
           

        }
    }
}
