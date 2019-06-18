using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using XService.Enterprise.Extensions;

namespace XService.Enterprise.Aspects
{
    public class ModelValidatorInterceptor : AbstractInterceptor
    {
        public ModelValidatorInterceptor() { }

        /// <summary>
        /// Attempts to perform argument data validation before the join point is executed.
        /// In the event of an model validation error, the join point will not be invoked and
        /// the interception will throw a ValidationException
        /// </summary>
        /// <param name="invocation">The join point or point of invocation</param>
        protected override void OnEntered(Castle.DynamicProxy.IInvocation invocation)
        {
            var isErrored = false;
            var messages = new List<string>();

            // Setup validation for every argument
            foreach (var arg in invocation.Arguments)
            {
                var results = arg.ValidateObject(out bool isValid);
                if (!isValid)
                {
                    results.ForEach(result => messages.Add($"{result.MemberNames} -> {result.ErrorMessage}"));
                    isErrored = true;
                }
            }

            // Report out errors if they occured
            if (isErrored)
            {
                var message = string.Join(System.Environment.NewLine, messages);
                throw new ValidationException(message);
            }
        }
    }
}
