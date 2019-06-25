using System.Linq;
using Microsoft.Extensions.Logging;

namespace XService.Enterprise.Aspects {
    public class LoggingInterceptor : AbstractInterceptor {
        private readonly ILogger<LoggingInterceptor> _Logger;
        private string _invocationName;
        private string _invocationArguments;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="writer">The output writer</param>
        public LoggingInterceptor(ILogger<LoggingInterceptor> logger) {
            _Logger = logger;
        }

        /// <summary>
        /// Logs the join point (invocation) and supplied arguments before the join point is invoked
        /// </summary>
        /// <param name="invocation"></param>
        protected override void OnEntered(Castle.DynamicProxy.IInvocation invocation) {
            _invocationName = $"{invocation.TargetType.Name}.{invocation.Method.Name}";
            _invocationArguments = string.Join(", ", invocation.Arguments.Select(arg => (arg ?? "").ToString()));

            _Logger?.LogInformation($"Call to: {_invocationName}");
            _Logger?.LogInformation($"   args: {_invocationArguments}");
        }

        /// <summary>
        /// Logs the join point (invocation) after the join point is invoked
        /// </summary>
        /// <param name="invocation"></param>
        protected override void OnExited(Castle.DynamicProxy.IInvocation invocation) {
            _Logger?.LogInformation($"Exited: {_invocationName}");
            _Logger?.LogInformation($"With Result:{invocation.ReturnValue}");
        }

        /// <summary>
        /// Logs the join point (invocation) when and error in the point or dowstream interceptors
        /// have errored
        /// </summary>
        /// <param name="invocation"></param>
        /// <param name="ex"></param>
        protected override void OnErrored(Castle.DynamicProxy.IInvocation invocation, System.Exception ex) {
            _Logger?.LogError($"Errored: {_invocationName}");
            _Logger?.LogError($"       : {ex.ToString()}");
        }
    }
}
