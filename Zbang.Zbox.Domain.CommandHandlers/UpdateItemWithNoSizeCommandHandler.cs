using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class UpdateItemWithNoSizeCommandHandler : ICommandHandlerAsync<UpdateItemWithNoSizeCommand>
    {
        private readonly IBlobProvider2<FilesContainerName> m_BlobProvider;
        private readonly IRepository<File> m_FileProvider;
        private readonly IQueueProvider m_QueueProvider;



        public UpdateItemWithNoSizeCommandHandler(IBlobProvider2<FilesContainerName> blobProvider, IRepository<File> fileProvider, IQueueProvider queueProvider)
        {
            m_BlobProvider = blobProvider;
            m_FileProvider = fileProvider;
            m_QueueProvider = queueProvider;
        }

        public async Task HandleAsync(UpdateItemWithNoSizeCommand message)
        {
            var usersToUpdate = new List<long>();
            foreach (var itemId in message.ItemIds)
            {
                var file = m_FileProvider.Load(itemId);
                var size = await m_BlobProvider.SizeAsync(file.ItemContentUrl);
                file.ShouldMakeDirty = () => false;
                file.Size = size;
                usersToUpdate.Add(file.UploaderId);
                m_FileProvider.Save(file);
            }
            await m_QueueProvider.InsertMessageToTranactionAsync(new QuotaData(usersToUpdate));

        }
    }
}
