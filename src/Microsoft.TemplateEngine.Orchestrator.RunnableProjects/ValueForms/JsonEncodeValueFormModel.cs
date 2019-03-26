using System.Collections.Generic;
using Microsoft.TemplateEngine.Abstractions.Json;

namespace Microsoft.TemplateEngine.Orchestrator.RunnableProjects.ValueForms
{
    public class JsonEncodeValueFormModel : IValueForm
    {
        public string Identifier => "jsonEncode";

        public string Name { get; }

        public JsonEncodeValueFormModel()
        {
        }

        public JsonEncodeValueFormModel(string name)
        {
            Name = name;
        }

        public IValueForm FromJson(string name, IJsonObject configuration)
        {
            return new JsonEncodeValueFormModel(name);
        }

        public string Process(IReadOnlyDictionary<string, IValueForm> forms, string value)
        {
            return value.Replace("\\", "\\\\").Replace("\"", "\\\"");
        }
    }
}
