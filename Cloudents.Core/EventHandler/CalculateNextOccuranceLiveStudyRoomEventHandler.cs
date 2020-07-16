using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.EventHandler
{
    public sealed class CalculateNextOccurenceLiveStudyRoomEventHandler : IEventHandler<EndStudyRoomSessionEvent> , IDisposable
    {
        private readonly ICronService _cronService;
        private readonly IUnitOfWork _unitOfWork;


        public CalculateNextOccurenceLiveStudyRoomEventHandler(ICronService cronService, IUnitOfWork unitOfWork)
        {
            _cronService = cronService;
            _unitOfWork = unitOfWork;
        }

        [SuppressMessage("ReSharper", "AsyncConverter.AsyncAwaitMayBeElidedHighlighting", Justification = "disposible")]
        public async Task HandleAsync(EndStudyRoomSessionEvent eventMessage, CancellationToken token)
        {
            if (eventMessage.Session.StudyRoom is BroadCastStudyRoom recurringSession 
                && recurringSession.Schedule != null)
            {
                if (recurringSession.BroadcastTime < DateTime.UtcNow)
                {
                    return;
                }
                var dateTime = _cronService.GetNextOccurrence(recurringSession.Schedule!.CronString);
                if (dateTime < recurringSession.Schedule.End)
                {
                    recurringSession.BroadcastTime = dateTime;
                }
                await _unitOfWork.CommitAsync(token);

            }

        }

        public void Dispose()
        {
            _unitOfWork.Dispose();
        }
    }
}