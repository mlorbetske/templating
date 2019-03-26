using System.Collections.Generic;
using Microsoft.TemplateEngine.Abstractions.Json;

namespace Microsoft.TemplateEngine.Orchestrator.RunnableProjects.ValueForms
{
    public class LowerCaseInvariantValueFormModel : IValueForm
    {
        public string Identifier => "lowerCaseInvariant";

        public string Name { get; }

        public LowerCaseInvariantValueFormModel()
        {
        }

        public LowerCaseInvariantValueFormModel(string name)
        {
            Name = name;
        }

        public IValueForm FromJson(string name, IJsonObject configuration)
        {
            return new LowerCaseInvariantValueFormModel(name);
        }

        public string Process(IReadOnlyDictionary<string, IValueForm> forms, string value)
        {
            return value.ToLowerInvariant();
        }
    }
}
