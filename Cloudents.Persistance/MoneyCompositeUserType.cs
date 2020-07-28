using System;
using System.Data.Common;
using Cloudents.Core.Entities;
using NHibernate;
using NHibernate.Engine;
using NHibernate.Type;
using NHibernate.UserTypes;

namespace Cloudents.Persistence
{
    public class MoneyCompositeUserType : ICompositeUserType
    {
        public object GetPropertyValue(object component, int property)
        {
            Money money = (Money)component;
            if (property == 0)
            {
                return money.Amount;
            }
            else
            {
                return money.Currency;
            }
        }

        public void SetPropertyValue(object component, int property, object value)
        {
            throw new InvalidOperationException("Money is an immutable object. SetPropertyValue isn't supported.");
        }

        public bool Equals(object x, object y)
        {
            if (x == y) return true;

            if (x == null || y == null) return false;

            return x.Equals(y);
        }

        public int GetHashCode(object x)
        {
            return x.GetHashCode();
        }

        public object? NullSafeGet(DbDataReader dr, string[] names, ISessionImplementor session, object owner)
        {
            if (dr == null)
            {
                return null;
            }
            string amountColumn = names[0];
            string currencyColumn = names[1];

            var amountValue = NHibernateUtil.Double.NullSafeGet(dr, amountColumn, session, owner);
            if (amountValue == null)
            {
                return null;
            }
            double val = (double)amountValue;
            var  currency = NHibernateUtil.String.NullSafeGet(dr, currencyColumn, session, owner)?.ToString();
            if (string.IsNullOrEmpty(currency))
            {
                return null;
            } 
            Money money = new Money(val, currency);

            return money;
        }

        public void NullSafeSet(DbCommand cmd, object value, int index, bool[] settable, ISessionImplementor session)
        {
            //if (value == null)
            //    return;
            double? amount = null;
            string? currency = null;
            if (value != null)
            {
                amount = ((Money)value).Amount;
                currency = ((Money)value).Currency;
            }
            NHibernateUtil.Double.NullSafeSet(cmd, amount, index, session);
            NHibernateUtil.String.NullSafeSet(cmd, currency, index + 1, session);
        }

        public object? DeepCopy(object value)
        {
            if (value == null)
            {
                return null;
            }
            return new Money(((Money)value).Amount, ((Money)value).Currency);
        }

        public object? Disassemble(object value, ISessionImplementor session)
        {
            return DeepCopy(value);
        }

        public object? Assemble(object cached, ISessionImplementor session, object owner)
        {
            return DeepCopy(cached);
        }

        public object? Replace(object original, object target, ISessionImplementor session, object owner)
        {
            return DeepCopy(original);
        }

        public string[] PropertyNames => new [] { "Amount", "Currency" };
        public IType[] PropertyTypes => new IType[] { NHibernateUtil.Double, NHibernateUtil.String };
        public Type ReturnedClass => typeof(Money);
        public bool IsMutable => false;
    }
}