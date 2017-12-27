using System.Collections.Generic;
using Microsoft.TemplateEngine.Abstractions;

namespace Microsoft.TemplateEngine.Orchestrator.RunnableProjects.Config
{
    public interface IServiceContainer
    {
        bool TryGetService<T>(out T service)
            where T : class, IIdentifiedComponent;

        IReadOnlyList<T> GetServices<T>()
            where T : class, IIdentifiedComponent;
    }
}
