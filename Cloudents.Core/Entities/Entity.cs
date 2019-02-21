using System;
using System.Collections.Generic;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Entities
{
    public abstract class Entity<T> where T : IEquatable<T>
    {
        public virtual T Id { get; protected set; }


        public override bool Equals(object obj)
        {
            var other = obj as Entity<T>;

            if (ReferenceEquals(other, null))
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (GetRealType() != other.GetRealType())
                return false;

            if (Id.Equals(default) || other.Id.Equals(default))
                return false;

            return EqualityComparer<T>.Default.Equals(Id, other.Id);
        }

        public static bool operator ==(Entity<T> a, Entity<T> b)
        {
            if (ReferenceEquals(a, null) && ReferenceEquals(b, null))
                return true;

            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
                return false;

            return a.Equals(b);
        }

        public static bool operator !=(Entity<T> a, Entity<T> b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return (GetRealType().ToString() + Id).GetHashCode();
        }

        public virtual object Actual => this;

        private Type GetRealType()
        {
            return Actual.GetType();
        }
    }

    public abstract class AggregateRoot : AggregateRoot<long>
    {

    }
    public abstract class AggregateRoot<T>: Entity<T> where T : IEquatable<T>
    {
     

        private readonly List<IEvent> _domainEvents = new List<IEvent>();
        public virtual IReadOnlyList<IEvent> DomainEvents => _domainEvents;

        protected virtual void AddEvent(IEvent newEvent)
        {
            _domainEvents.Add(newEvent);
        }

        public virtual void ClearEvents()
        {
            _domainEvents.Clear();
        }
    }
}