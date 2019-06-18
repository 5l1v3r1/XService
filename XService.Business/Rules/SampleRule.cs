using Microsoft.Extensions.Logging;

namespace XService.Business.Rules
{
    public class SampleRule : IRule
    {
        private readonly ILogger<SampleRule> _Logger;

        public SampleRule(ILogger<SampleRule> logger)
        {
            _Logger = logger;
        }

        public string Execute(Models.SampleModel data)
        {
            _Logger?.LogInformation("Trace from sample rule");
            return data.Name.ToUpper();
        }
    }
}
