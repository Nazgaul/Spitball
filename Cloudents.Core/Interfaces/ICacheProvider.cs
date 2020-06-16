using System;

namespace Cloudents.Core.Interfaces
{
    public interface ICacheProvider 
    {
        object? Get(string key, string region);


        /// <summary>
        /// Return element from cache
        /// </summary>
        /// <typeparam name="T">return T or null if not found</typeparam>
        /// <param name="key"></param>
        /// <param name="region"></param>
        /// <returns></returns>
        T Get<T>(string key, string region);

        /// <summary>
        /// Add element to cache
        /// </summary>
        /// <param name="key">the key to same</param>
        /// <param name="region">the region of the key</param>
        /// <param name="value">the value to save</param>
        /// <param name="expire">time till expire in seconds</param>
        /// <param name="slideExpiration">true is we want sliding   </param>
        //void Set(string key, string region, object? value, int expire, bool slideExpiration);


        void Set<T>(string key, string region, T value, TimeSpan expire, bool slideExpiration);

        void Set(string key, string region, object? value, TimeSpan expire, bool slideExpiration);

        bool Exists(string key, string region);

        void DeleteRegion(string region);
        void DeleteKey(string region, string key);
    }
}