//using Cloudents.Core.Enum;
//using System;
//using System.Diagnostics.CodeAnalysis;

//namespace Cloudents.Core.Entities
//{
//    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "Nhibernate")]
//    [SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
//    public class University : Entity<Guid>
//    {
//        //public University(string name, string country) : this()
//        //{
//        //    Name = name.Replace("+", "-");
//        //    Country = country;
//        //    RowDetail = new DomainTimeStamp();
//        //    State = ItemState.Pending;
//        //}

//        protected University()
//        {

//        }

//        public virtual void Approve()
//        {
//            //TODO: maybe put an event to that
//            if (State == ItemState.Pending)
//            {
//                State = ItemState.Ok;
//            }
//        }


//        //public virtual void Rename(string newName)
//        //{
//        //    Name = newName;
//        //}

//        //public virtual Guid Id { get; protected set; }


//        public virtual string Name { get; protected set; }

//        /// <summary>
//        /// Used as extra synonym to add to university search
//        /// </summary>
//        public virtual string Extra { get; protected set; }

//        public virtual string Country { get; protected set; }

//        public virtual string Image { get; set; }
//        public virtual int UsersCount { get; set; }

//        public virtual DomainTimeStamp RowDetail { get; protected set; }

//        //  private readonly IList<Document> _documents = new List<Document>();
//        // public virtual IReadOnlyList<Document> Documents => _documents.ToList();
//        /*
//        private readonly IList<Question> _questions = new List<Question>();
//        public virtual IReadOnlyList<Question> Questions => _questions.ToList();

//        private readonly IList<User> _users = new List<User>();
//        public virtual IReadOnlyList<User> Users => _users.ToList();
//        */
//        public virtual ItemState? State { get; protected set; }
//    }
//}
