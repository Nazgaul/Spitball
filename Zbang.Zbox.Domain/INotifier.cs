using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zbang.Zbox.Infrastructure.Events;
using Zbang.Zbox.Infrastructure.Mail;

namespace Zbang.Zbox.Domain
{
    public interface INotifier
    {
        void Notify(int boxId, Guid userId, NotificationEventType eventType, string unityName, CreateMailParams parameters);
    }
}
