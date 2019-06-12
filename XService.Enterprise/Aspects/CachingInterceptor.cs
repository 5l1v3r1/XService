using System;
using System.Linq;
using System.Runtime.Caching;
using Castle.DynamicProxy;
using XService.Enterprise.Attributes;
using XService.Enterprise.Contracts;

namespace XService.Enterprise.Aspects {
    public class CachingInterceptor : IInterceptor {
        private readonly ICacheProvider _cache;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cache"></param>
        public CachingInterceptor(ICacheProvider cache) {
            _cache = cache;
        }

        /// <summary>
        /// Logs the join point (invocation) and supplied arguments before the join point is invoked
        /// </summary>
        /// <param name="invocation"></param>
        public void Intercept(IInvocation invocation) {
            var cacheAttr = GetCacheResultAttribute(invocation);

            if (cacheAttr == null) {
                invocation.Proceed();
                return;
            }

            string key = GetInvocationSignature(invocation);

            if (_cache.Contains(key)) {
                invocation.ReturnValue = _cache.Get(key);
                return;
            }

            invocation.Proceed();
            var result = invocation.ReturnValue;

            if (result != null) {
                _cache.Put(key, result, cacheAttr.Duration);
            }
        }

        /// <summary>
        /// Gets the CacheResultAttribute from the join point
        /// </summary>
        /// <param name="invocation"></param>
        /// <returns></returns>
        private CacheResultAttribute GetCacheResultAttribute(IInvocation invocation) {
            return Attribute.GetCustomAttribute(
                invocation.MethodInvocationTarget,
                typeof(CacheResultAttribute)
            )
            as CacheResultAttribute;
        }

        /// <summary>
        /// Gets the invocation signature for use ask the cache join point key
        /// </summary>
        /// <param name="invocation"></param>
        /// <returns></returns>
        private string GetInvocationSignature(IInvocation invocation) {
            return String.Format("{0}-{1}-{2}",
                invocation.TargetType.FullName,
                invocation.Method.Name,
                String.Join("-", invocation.Arguments.Select(a => (a ?? "").ToString()).ToArray())
            );
        }
    }
}
