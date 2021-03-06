﻿using System;
using System.Collections.Generic;

namespace Cloudents.Command.Courses
{
    public class CreateCourseCommand : ICommand
    {
        public CreateCourseCommand(long userId, string name, int price, int? subscriptionPrice,
            string description, string? image,
            IEnumerable<CreateLiveStudyRoomCommand> studyRooms,
            IEnumerable<CreateDocumentCommand> documents, bool isPublish)
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
        }

        public long UserId { get; }
        public string Name { get; }


        public int Price { get;  }

        public int? SubscriptionPrice { get;  }

        public string Description { get; }

        public string? Image { get;}

        public IEnumerable<CreateLiveStudyRoomCommand> StudyRooms { get;  }
        public IEnumerable<CreateDocumentCommand> Documents { get;  }
        public bool IsPublish { get;  }

        public long Id { get; set; }


        public class CreateLiveStudyRoomCommand
        {
            public CreateLiveStudyRoomCommand(string name, DateTime date)
            {
                Name = name;
                Date = date;
            }

            public string Name { get;  }

       
            public DateTime Date { get;  }
        }


        public class CreateDocumentCommand
        {
            public CreateDocumentCommand(string blobName, string name, bool visible)
            {
                BlobName = blobName;
                Name = name;
                Visible = visible;
            }

            public string BlobName { get;  }
            public string Name { get;  }

            public bool Visible { get;  }
        }
    }
}