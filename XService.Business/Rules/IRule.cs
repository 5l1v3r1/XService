using XService.Business.Models;

namespace XService.Business.Rules
{
    public interface IRule
    {
        string Execute(SampleModel data);
    }
}
