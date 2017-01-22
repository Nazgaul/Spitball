using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class UpdateItemCourseTagCommand : ICommand
    {
        public UpdateItemCourseTagCommand(long itemId, string boxName, string boxCode, string boxProfessor)
        {
            ItemId = itemId;
            BoxName = boxName;
            BoxCode = boxCode;
            BoxProfessor = boxProfessor;
        }

        public long ItemId { get; private set; }
        public string BoxName { get; private set; }
        public string BoxCode { get; private set; }
        public string BoxProfessor { get; private set; }
    }
}
