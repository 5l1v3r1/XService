using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using Castle.DynamicProxy;

namespace XService.Enterprise.Aspects
{
    /// <summary>
    /// Provides an interceptor pipeline implementation
    /// </summary>
    public abstract class AbstractInterceptor : IInterceptor
    {
        /// <summary>
        /// Provides a pipeline for interception
        /// </summary>
        /// <param name="invocation"></param>
        public virtual void Intercept(IInvocation invocation)
        {
            try {
                OnEntered(invocation);
                invocation.Proceed();
                OnExited(invocation);
            } catch (Exception ex) { 
                OnErrored(invocation, ex);
                throw;
            } finally {
                OnFinally(invocation);
            }
        }

        /// <summary>
        /// Provides interception on the join point (invocation) before the join point is invoked
        /// </summary>
        /// <param name="invocation">The join point</param>
        protected virtual void OnEntered(IInvocation invocation) { }

        /// <summary>
        /// Provides interception on the join point (invocation) immediately after the join point is invoked
        /// </summary>
        /// <param name="invocation">The join point</param>
        protected virtual void OnExited(IInvocation invocation) { }

        /// <summary>
        /// Provides interception on the join point (invocation) when the join point errored
        /// </summary>
        /// <param name="invocation">The join point</param>
        /// <param name="ex"></param>
        protected virtual void OnErrored(IInvocation invocation, Exception ex) { }

        /// <summary>
        /// Provides interception on the join point (invocation) when the join point is exiting regardless of it succeeded for failed
        /// </summary>
        /// <param name="invocation"></param>
        protected virtual void OnFinally(IInvocation invocation) { }
    }
}