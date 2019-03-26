using System.Collections.Generic;
using Microsoft.TemplateEngine.Abstractions.Json;

namespace Microsoft.TemplateEngine.Orchestrator.RunnableProjects.ValueForms
{
    public class IdentityValueForm : IValueForm
    {
        public static readonly string FormName = "identity";

        public string Identifier => FormName;

        public string Name { get; }

        public IdentityValueForm()
        {
        }

        public IdentityValueForm(string name)
        {
            Name = name;
        }

        public IValueForm FromJson(string name, IJsonObject configuration)
        {
            return new IdentityValueForm(name);
        }

        public string Process(IReadOnlyDictionary<string, IValueForm> forms, string value)
        {
            return value;
        }
    }
}
