using System.Collections.Generic;
using Microsoft.TemplateEngine.Abstractions;
using Microsoft.TemplateEngine.Abstractions.Mount;
using Microsoft.TemplateEngine.Core.Contracts;
using Newtonsoft.Json.Linq;

namespace Microsoft.TemplateEngine.Orchestrator.RunnableProjects.Config
{
    public interface IOperationConfigWithServiceContainerAccess : IOperationConfig
    {
        IEnumerable<IOperationProvider> ConfigureFromJObject(IServiceContainer container, JObject rawConfiguration, IDirectory templateRoot);
    }
}
