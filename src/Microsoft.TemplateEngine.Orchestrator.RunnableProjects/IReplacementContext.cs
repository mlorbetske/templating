using Microsoft.TemplateEngine.Abstractions.Json;

namespace Microsoft.TemplateEngine.Orchestrator.RunnableProjects
{
    public interface IReplacementContext : IJsonSerializable<IReplacementContext>
    {
        string OnlyIfBefore { get; }

        string OnlyIfAfter { get; }
    }
}
