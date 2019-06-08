using Microsoft.Extensions.Logging;

namespace XService.Business.Rules
{
  public class SampleRule : IRule
  {
    private ILogger<SampleRule> _Logger;

    public SampleRule(ILogger<SampleRule> logger) {
        _Logger = logger;
    }

    public string Execute(string s)
    {
        _Logger?.LogInformation("Trace from sample rule");
        return s.ToUpper();
    }
  }
}