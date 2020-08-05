using System;
using System.Collections.Generic;

namespace Cloudents.Command.Courses
{
    public class UpdateCourseCommand : ICommand
    {
        public UpdateCourseCommand(long userId, string name, int price, int? subscriptionPrice,
            string description, string? image,
            IEnumerable<UpdateLiveStudyRoomCommand> studyRooms,
            IEnumerable<UpdateDocumentCommand> documents, bool isPublish, long courseId)
        {
            UserId = userId;
            Name = name;
            Price = price;
            SubscriptionPrice = subscriptionPrice;
            Description = description;
            Image = image;
            StudyRooms = studyRooms ?? throw new ArgumentNullException(nameof(studyRooms));
            Documents = documents ?? throw new ArgumentNullException(nameof(documents));
            IsPublish = isPublish;
            CourseId = courseId;
        }
        public long CourseId { get; }

        public long UserId { get; }
        public string Name { get; }


        public int Price { get;  }

        public int? SubscriptionPrice { get;  }

        public string Description { get; }

        public string? Image { get;}

        public IEnumerable<UpdateLiveStudyRoomCommand> StudyRooms { get;  }
        public IEnumerable<UpdateDocumentCommand> Documents { get;  }
        public bool IsPublish { get;  }


        public class UpdateLiveStudyRoomCommand
        {
            public UpdateLiveStudyRoomCommand(string name, DateTime date)
            {
                Name = name;
                Date = date;
            }

            public string Name { get;  }

       
            public DateTime Date { get;  }
        }


        public class UpdateDocumentCommand
        {
            public UpdateDocumentCommand(long? id, string? blobName, string name, bool visible)
            {
                BlobName = blobName;
                Name = name;
                Visible = visible;
                Id = id;
            }

            public string? BlobName { get;  }
            public string Name { get;  }

            public bool Visible { get;  }

            public long? Id { get; set; }
        }
    }
}