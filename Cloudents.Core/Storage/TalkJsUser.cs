using System;

namespace Cloudents.Core.Storage
{
    [Serializable]
    public class TalkJsUser
    {
        public TalkJsUser(long id, string name)
        {
            Id = id;
            Name = name;
        }

        protected TalkJsUser()
        {
        }

        public long Id { get; private set; }
        public string Name { get; private set; }
        public string Email { get;  set; }
        public string Phone { get;  set; }
        public string PhotoUrl { get;  set; }
    }
}