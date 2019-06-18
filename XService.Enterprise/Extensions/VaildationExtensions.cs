using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace XService.Enterprise.Extensions
{
    public static class VaildationExtensions
    {
        /// <summary>
        /// Performs validation on an objet
        /// </summary>
        /// <param name="source"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static List<ValidationResult> ValidateObject(this object source, out bool result)
        {
            // wire up a context for validation - ideally this may need to be cached to optimize
            ValidationContext context = new ValidationContext(source, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            result = Validator.TryValidateObject(source, context, results, true);
            return results;
        }
    }
}
