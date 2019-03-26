using System.Collections.Generic;
using Microsoft.TemplateEngine.Abstractions.Json;

namespace Microsoft.TemplateEngine.Orchestrator.RunnableProjects.ValueForms
{
    public interface IValueForm
    {
        string Identifier { get; }

        string Name { get; }

        string Process(IReadOnlyDictionary<string, IValueForm> forms, string value);

        IValueForm FromJson(string name, IJsonObject configuration);
    }
}
