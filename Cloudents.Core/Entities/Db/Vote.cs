using System;

namespace Cloudents.Core.Entities.Db
{
    public class Vote
    {
        public Guid Id { get; set; }
        public User User { get; set; }
        public Document Document { get; set; }
    }
}