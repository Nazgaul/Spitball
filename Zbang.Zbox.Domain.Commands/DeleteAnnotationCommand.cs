using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class DeleteAnnotationCommand : ICommand
    {
        public DeleteAnnotationCommand(long annotationId, long userId)
        {
            AnnotationId = annotationId;
            UserId = userId;
        }
        public long AnnotationId { get; set; }

        public long UserId { get; set; }
    }
}
