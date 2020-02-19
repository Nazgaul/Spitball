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
            var fields = typeof(T).GetFields(BindingFlags.Public |
                                             BindingFlags.Static |
                                             BindingFlags.DeclaredOnly);

            return fields.Select(f => f.GetValue(null)).Cast<T>();
        }

        public override bool Equals(object obj)
        {
            var otherValue = obj as Enumeration;

            if (otherValue == null)
                return false;

            var typeMatches = GetType().Equals(obj.GetType());
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
            var matchingItem = Parse<T, int>(value, "value", item => item.Id == value);
            return matchingItem;
        }
        public static T FromDisplayName<T>(string displayName) where T : Enumeration
        {
            var matchingItem = Parse<T, string>(displayName, "display name", item =>
                string.Equals(item.Name, displayName, StringComparison.OrdinalIgnoreCase));
            return matchingItem;
        }

        private static T Parse<T, K>(K value, string description, Func<T, bool> predicate) where T : Enumeration
        {
            var matchingItem = GetAll<T>().FirstOrDefault(predicate);

            //if (matchingItem == null)
            //    throw new InvalidOperationException($"'{value}' is not a valid {description} in {typeof(T)}");

            return matchingItem;
        }

        public static bool operator ==(Enumeration left, Enumeration right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Enumeration left, Enumeration right)
        {
            return !Equals(left, right);
        }
    }


    //public abstract class EnumerationString : IComparable
    //{
    //    public string Name { get; private set; }

    //    public string Id { get; private set; }

    //    protected EnumerationString(string id, string name)
    //    {
    //        Id = id;
    //        Name = name;
    //    }

    //    public override string ToString() => Name;

    //    public static IEnumerable<T> GetAll<T>() where T : EnumerationString
    //    {
    //        var fields = typeof(T).GetFields(BindingFlags.Public |
    //                                         BindingFlags.Static |
    //                                         BindingFlags.DeclaredOnly);

    //        return fields.Select(f => f.GetValue(null)).Cast<T>();
    //    }

    //    public override bool Equals(object obj)
    //    {
    //        var otherValue = obj as Enumeration;

    //        if (otherValue == null)
    //            return false;

    //        var typeMatches = GetType().Equals(obj.GetType());
    //        var valueMatches = Id.Equals(otherValue.Id);

    //        return typeMatches && valueMatches;
    //    }

    //    public int CompareTo(object other) => Id.CompareTo(((Enumeration)other).Id);


    //    public static T FromValue<T>(string value) where T : EnumerationString
    //    {
    //        var matchingItem = Parse<T, string>(value, "value", item => item.Id == value);
    //        return matchingItem;
    //    }

    //    private static T Parse<T, K>(K value, string description, Func<T, bool> predicate) where T : EnumerationString
    //    {
    //        var matchingItem = GetAll<T>().FirstOrDefault(predicate);

    //        if (matchingItem == null)
    //            throw new InvalidOperationException($"'{value}' is not a valid {description} in {typeof(T)}");

    //        return matchingItem;
    //    }

    //    public static bool operator ==(EnumerationString left, EnumerationString right)
    //    {
    //        return Equals(left, right);
    //    }

    //    public static bool operator !=(EnumerationString left, EnumerationString right)
    //    {
    //        return !Equals(left, right);
    //    }
    //}

    //public abstract class EnumerationString : Enumeration<string> 
    //{
    //    protected EnumerationString(string id, string name) : base(id, name)
    //    {
    //    }
    //}

    //public static class EnumerationExtensions
    //{
    //    public static IEnumerable<TU> GetAll<TU>(this TU ob) where TU : Enumeration<>
    //    {
    //        var fields = typeof(TU).GetFields(BindingFlags.Public |
    //                                          BindingFlags.Static |
    //                                          BindingFlags.DeclaredOnly);

    //        return fields.Select(f => f.GetValue(null)).Cast<TU>();
    //    }
    //}


    //public abstract class Enumeration<T> where T : IComparable
    //{
    //    public string Name { get; private set; }

    //    public T Id { get; private set; }

    //    protected Enumeration(T id, string name)
    //    {
    //        Id = id;
    //        Name = name;
    //    }

    //    public override string ToString() => Name;



    //    protected bool Equals(Enumeration<T> other)
    //    {
    //        return EqualityComparer<T>.Default.Equals(Id, other.Id);
    //    }

    //    public override bool Equals(object obj)
    //    {
    //        if (ReferenceEquals(null, obj)) return false;
    //        if (ReferenceEquals(this, obj)) return true;
    //        if (obj.GetType() != this.GetType()) return false;
    //        return Equals((Enumeration<T>)obj);
    //    }

    //    public override int GetHashCode()
    //    {
    //        return EqualityComparer<T>.Default.GetHashCode(Id);
    //    }

    //    public static bool operator ==(Enumeration<T> left, Enumeration<T> right)
    //    {
    //        return Equals(left, right);
    //    }

    //    public static bool operator !=(Enumeration<T> left, Enumeration<T> right)
    //    {
    //        return !Equals(left, right);
    //    }

    //    public static IEnumerable<TU> GetAll<TU>() where TU : Enumeration
    //    {
    //        var fields = typeof(TU).GetFields(BindingFlags.Public |
    //                                          BindingFlags.Static |
    //                                          BindingFlags.DeclaredOnly);

    //        return fields.Select(f => f.GetValue(null)).Cast<TU>();
    //    }
    //}
}