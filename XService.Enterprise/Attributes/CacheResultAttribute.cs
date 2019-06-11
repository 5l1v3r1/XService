using System;

namespace XService.Enterprise.Attributes
{
    /// <summary>
    /// Defines a join point as being cacheable via interception
    /// </summary>
    /// <code>
    /// [CacheResult(Duration = 1000)]
    /// </code>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
    public class CacheResultAttribute : System.Attribute
    {
        /// <summary>
        /// The duration a result is valid in the cache in milliseconds
        /// </summary>
        /// <value></value>
        public int Duration { get; set; }
    }
}