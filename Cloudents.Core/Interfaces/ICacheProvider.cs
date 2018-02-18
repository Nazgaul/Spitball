namespace Cloudents.Core.Interfaces
{
    public interface ICacheProvider
    {
        object Get(string key, string region);

        /// <summary>
        /// Add element to cache
        /// </summary>
        /// <param name="key">the key to same</param>
        /// <param name="region">the region of the key</param>
        /// <param name="value">the value to save</param>
        /// <param name="expire">time till expire</param>
        /// <param name="slideExpiration">true is we want sliding   </param>
        /// <returns>modified object to pass along e.g convert iEnumerable to list</returns>
        object Set(string key, string region, object value, int expire, bool slideExpiration);
    }
}