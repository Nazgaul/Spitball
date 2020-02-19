using Cloudents.Core.Interfaces;
using System;
using System.Collections.Generic;

namespace Cloudents.Core.Entities
{

    public abstract class Entity<T> : IEntity where T : IEquatable<T>
    {
        public virtual T Id { get; protected set; }



        public override bool Equals(object obj)
        {
            if (!(obj is Entity<T> other))
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
            // ReSharper disable once NonReadonlyMemberInGetHashCode nhibernate
            return (GetRealType().ToString() + Id).GetHashCode();
        }

        // ReSharper disable once VirtualMemberNeverOverridden.Global nhibernate
        public virtual object Actual => this;

        private Type GetRealType()
        {
            return Actual.GetType();
        }

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


    public interface IEntity
    {
        IReadOnlyList<IEvent> DomainEvents { get; }
        void ClearEvents();
    }

    public interface IAggregateRoot
    {
        //IReadOnlyList<IEvent> DomainEvents { get; }
        //void ClearEvents();
    }
}