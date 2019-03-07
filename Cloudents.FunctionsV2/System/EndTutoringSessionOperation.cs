using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Message.Email;
using Microsoft.Azure.WebJobs;

namespace Cloudents.FunctionsV2.System
{
   public class EndTutoringSessionOperation : ISystemOperation<EndTutoringSessionMessage>
   {
       private readonly IVideoProvider _videoProvider;

       public EndTutoringSessionOperation(IVideoProvider videoProvider)
       {
           _videoProvider = videoProvider;
       }

       public async Task DoOperationAsync(EndTutoringSessionMessage msg, IBinder binder, CancellationToken token)
       {
           await _videoProvider.CloseRoomAsync(msg.RoomId);
        }
    }
}
