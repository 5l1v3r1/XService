namespace XService.Enterprise.Contracts {
    public interface ICacheProvider {
        /// <summary>
        /// Returns an object if it exists from the cache
        /// </summary>
        /// <param name="key">The object key</param>
        /// <returns></returns>
        object Get(string key);

        /// <summary>
        /// Puts an object in the cache with a specific lifetime
        /// </summary>
        /// <param name="key">The object key</param>
        /// <param name="value">The object</param>
        /// <param name="duration">The cache lifetime</param>
        void Put(string key, object value, int duration);

        /// <summary>
        /// Determines if an object is in the cache
        /// </summary>
        /// <param name="key">The object key</param>
        /// <returns></returns>
        bool Contains(string key);
    }
}