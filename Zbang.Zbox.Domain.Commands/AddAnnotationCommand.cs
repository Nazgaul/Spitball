using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class AddAnnotationCommand : ICommand
    {
        public AddAnnotationCommand(string comment, int x, int y, int width,int height, long itemId, int imageId, long userId)
        {
            Comment = comment;
            X = x;
            Y = y;
            Width = width;
            Height = height;
            ItemId = itemId;
            ImageId = imageId;
            UserId = userId;
        }
        public string Comment { get; private set; }

        public int X { get; private set; }
        public int Y { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public long ItemId { get; private set; }
        public int ImageId { get; private set; }

        public long UserId { get; private set; }

        //out parameter
        public long AnnotationId { get; set; }
    }
}
