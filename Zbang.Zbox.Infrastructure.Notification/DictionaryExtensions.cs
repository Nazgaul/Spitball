using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.Infrastructure.Notifications
{
    public static class DictionaryExtensions
    {
        public static TValue GetValueOrDefault<TValue>(this IDictionary<string, object> dictionary, string key)
        {
            TValue result;
            dictionary.TryGetValue(key, out result);
            return result;
        }
        public static bool TryGetValue<TValue>(this IDictionary<string, object> dictionary, string key, out TValue value)
        {
            object obj;
            if (dictionary != null && dictionary.TryGetValue(key, out obj) && obj is TValue)
            {
                value = (TValue)((object)obj);
                return true;
            }
            value = default(TValue);
            return false;
        }

        public static void SetOrClearValue<T>(this IDictionary<string, object> dictionary, string key, T value)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException("dictionary");
            }
            if (EqualityComparer<T>.Default.Equals(value, default(T)))
            {
                dictionary.Remove(key);
                return;
            }
            dictionary[key] = value;
        }

    }
}
