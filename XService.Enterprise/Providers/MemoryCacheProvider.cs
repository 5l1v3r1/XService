using System;
using System.Runtime.Caching;

namespace XService.Enterprise.Providers {
    public class MemoryCacheProvider : Contracts.ICacheProvider {
        /// <inheritdoc />
        public object Get(string key) {
            return MemoryCache.Default[key];
        }

        /// <inheritdoc />
        public void Put(string key, object value, int duration) {
            if (duration <= 0) {
                throw new ArgumentException("Duration cannot be less or equal to zero", "duration");
            }

            var policy = new CacheItemPolicy {
                AbsoluteExpiration = DateTime.Now.AddMilliseconds(duration)
            };

            MemoryCache.Default.Set(key, value, policy);
        }

        /// <inheritdoc />
        public bool Contains(string key) {
            return MemoryCache.Default[key] != null;
        }
    }
}