using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Cloudents.Core
{
    public abstract class Enumeration : IComparable
    {
        public string Name { get; }

        public int Id { get; }

        protected Enumeration(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public override string ToString() => Name;

       

        public static IEnumerable<T> GetAll<T>() where T : Enumeration
        {
           // var type = typeof(T);
           //TODO need to cache this
            var fields = typeof(T).GetFields(BindingFlags.Public |
                                             BindingFlags.Static |
                                             BindingFlags.DeclaredOnly);

            return fields.Where(w=> !w.IsLiteral).Select(f => f.GetValue(null)).Cast<T>();
        }

        public override bool Equals(object obj)
        {
            var otherValue = obj as Enumeration;

            if (otherValue == null)
                return false;

            var typeMatches = GetType() == obj.GetType();
            var valueMatches = Id.Equals(otherValue.Id);

            return typeMatches && valueMatches;
        }

        

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public int CompareTo(object other) => Id.CompareTo(((Enumeration)other).Id);


        public static T FromValue<T>(int value) where T : Enumeration
        {
            var matchingItem = Parse<T>(item => item.Id == value);
            return matchingItem;
        }

        protected static T FromDisplayName<T>(string displayName) where T : Enumeration
        {
            var matchingItem = Parse<T>(
                item => string.Equals(item.Name, displayName, StringComparison.OrdinalIgnoreCase));
            return matchingItem;
        }

        private static T Parse<T>(Func<T, bool> predicate) where T : Enumeration
        {
            var all = GetAll<T>();
            var matchingItem = all.FirstOrDefault(predicate);
           

            return matchingItem;
        }

        public static bool operator ==(Enumeration? left, Enumeration? right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Enumeration? left, Enumeration? right)
        {
            return !Equals(left, right);
        }
    }
   
}