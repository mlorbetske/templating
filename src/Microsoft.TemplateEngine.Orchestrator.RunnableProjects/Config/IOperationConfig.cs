using System.Collections.Generic;
using Microsoft.TemplateEngine.Abstractions;
using Microsoft.TemplateEngine.Abstractions.Json;
using Microsoft.TemplateEngine.Abstractions.Mount;
using Microsoft.TemplateEngine.Core.Contracts;

namespace Microsoft.TemplateEngine.Orchestrator.RunnableProjects.Config
{
    public interface IOperationConfig : IIdentifiedComponent
    {
        string Key { get; }

        IEnumerable<IOperationProvider> ConfigureFromJson(IJsonObject rawConfiguration, IDirectory templateRoot);
    }
}
