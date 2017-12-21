using Microsoft.TemplateEngine.Abstractions;

namespace Microsoft.TemplateEngine.Core.Contracts
{
    public interface IKeyworder : IIdentifiedComponent
    {
        string EscapeKeywords(string input);
    }
}
