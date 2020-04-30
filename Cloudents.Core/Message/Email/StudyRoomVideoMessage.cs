//using System;
//using System.Globalization;
//using Cloudents.Core.Entities;
//using Cloudents.Core.Message.System;

//namespace Cloudents.Core.Message.Email
//{
//    public class StudyRoomVideoMessage : ISystemQueueMessage
//    {
//        public StudyRoomVideoMessage(string tutorFirstName, string userName, DateTime dateTime, string to, Language info)
//        {
//            TutorFirstName = tutorFirstName;
//            UserName = userName;
//            DateTime = dateTime;
//            //DownloadLink = downloadLink;
//            To = to;
//            Info = info;
//        }

//        // [DataMember(Name = "tutorName")]
//        public string TutorFirstName { get;private set; }
//        // [DataMember(Name = "firstName")]
//        public string UserName { get; private set; }
//        // [DataMember(Name = "firstName")]
//        public DateTime DateTime { get; private set; }

//        public Uri DownloadLink { get;  set; }

//        public string To { get; private set; }

//        public CultureInfo Info { get;private set; }
//    }
//}