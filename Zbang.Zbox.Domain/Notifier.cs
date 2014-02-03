using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zbang.Zbox.Infrastructure.Mail;
using Zbang.Zbox.Infrastructure.Repositories;
using System.Threading.Tasks;


namespace Zbang.Zbox.Domain
{
    public class Notifier : INotifier
    {
        //Fields
        private IMailManager m_MailManager;
        private IRepository<Box> m_BoxRepository;
        private IRepository<User> m_UserRepository;

        //string boxName, userCreatedEmail;
        IEnumerable<Guid> m_UsersSubscribed;

        //Ctor
        public Notifier(IMailManager mailManager, IRepository<Box> boxRepository, IRepository<User> userRepository)
        {
            m_MailManager = mailManager;
            m_BoxRepository = boxRepository;
            m_UserRepository = userRepository;
        }

        //Methods
        public void Notify(int boxId, Guid userId, NotificationEventType eventType, string unityName, CreateMailParams parameters)
        {
            GetUserSubscribers(boxId, userId);
            SendNotification(eventType, unityName, parameters);
        }

        private void GetUserSubscribers(int boxId, Guid userId)
        {
            Box box = m_BoxRepository.Get(boxId);
            //GetEventDatails(box, userId);
            //Gets the users that get notification but exlude the user that made the action
            m_UsersSubscribed = box.GetRules().Where(x => x.notificationSetting == NotificationSettings.On && x.user != userId).Select(s => s.user).Distinct();
        }

        //private void GetEventDatails(Box box, Guid userId)
        //{
        //    User userCreated = m_UserRepository.Get(userId);
        //    boxName = box.BoxName;
        //    userCreatedEmail = userCreated.Email;
        //}

        private void SendNotification(NotificationEventType eventType, string unityName, CreateMailParams parameters)
        {
            foreach (var user in m_UsersSubscribed)
            {
                User userDetail = m_UserRepository.Get(user);
                if (userDetail != null)
                {
                    Task.Factory.StartNew(() =>
                    {
                        m_MailManager.SendEmail(parameters, unityName, userDetail.Email);
                    });

                }

                //m_MailManager.SendEmail(
            }
        }
    }

    public enum NotificationEventType
    {
        Subscription,
        Updates,
        Delete
    }
}
