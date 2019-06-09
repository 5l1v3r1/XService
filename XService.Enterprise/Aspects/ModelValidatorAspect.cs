using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace XService.Enterprise.Aspects
{
  public class ModelValidatorAspect : AbstractInterceptor
  {
    public ModelValidatorAspect() { }

    /// <summary>
    /// Attempts to perform argument data validation before the join point is executed.
    /// In the event of an model validation error, the join point will not be invoked and
    /// the interception will throw a ValidationException
    /// </summary>
    /// <param name="invocation">The join point</param>
    protected override void OnEntered(Castle.DynamicProxy.IInvocation invocation) {
        var isErrored = false;
        var messages = new List<string>();
        
        // Setup validation for every argument
        foreach( var arg in invocation.Arguments) {
            // wire up a context for validation - ideally this would be cached
            var context = new ValidationContext(arg, serviceProvider:null, items:null);
            var results = new List<ValidationResult>();
            
            
            // validate the argument
            var isValid = Validator.TryValidateObject(arg, context, results);
            if(!isValid) {
                results.ForEach( result => messages.Add($"{result.MemberNames} -> {result.ErrorMessage}"));
                isErrored = true;
            }
        }

        // Report out errors if they occured
        if(isErrored) {
            var message = string.Join(System.Environment.NewLine, messages);
            throw new ValidationException(message);
        }
    }
  }
}