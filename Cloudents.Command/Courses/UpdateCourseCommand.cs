using System;
using System.Collections.Generic;
using Cloudents.Core.Entities;

namespace Cloudents.Command.Courses
{
    public class UpdateCourseCommand : ICommand
    {
        public UpdateCourseCommand(long userId, string name, int price, int? subscriptionPrice,
            string description, string? image,
            IEnumerable<UpdateLiveStudyRoomCommand> studyRooms,
            IEnumerable<UpdateDocumentCommand> documents, bool isPublish, long courseId, UpdateCouponCommand? coupon)
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
            Coupon = coupon;
        }
        public long CourseId { get; }
        public UpdateCouponCommand? Coupon { get; }

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

        public class UpdateCouponCommand 
        {
            public UpdateCouponCommand(string code, CouponType couponType,  double value, DateTime expiration)
            {
                Code = code;
                CouponType = couponType;
                Value = value;
                Expiration = expiration;
            }


            public double Value { get; }


            public string Code { get; }
            public CouponType CouponType { get; }
            public DateTime Expiration { get; }
        }
    }
}