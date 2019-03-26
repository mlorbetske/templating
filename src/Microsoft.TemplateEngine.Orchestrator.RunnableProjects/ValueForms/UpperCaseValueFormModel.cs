using System.Collections.Generic;
using Microsoft.TemplateEngine.Abstractions.Json;

namespace Microsoft.TemplateEngine.Orchestrator.RunnableProjects.ValueForms
{
    public class UpperCaseValueFormModel : IValueForm
    {
        public string Identifier => "upperCase";

        public string Name { get; }

        public UpperCaseValueFormModel()
        {
        }

        public UpperCaseValueFormModel(string name)
        {
            Name = name;
        }

        public IValueForm FromJson(string name, IJsonObject configuration)
        {
            return new UpperCaseValueFormModel(name);
        }

        public string Process(IReadOnlyDictionary<string, IValueForm> forms, string value)
        {
            return value.ToUpper();
        }
    }
}
