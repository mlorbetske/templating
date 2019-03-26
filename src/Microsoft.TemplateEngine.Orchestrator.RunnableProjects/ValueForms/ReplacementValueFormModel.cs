using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.TemplateEngine.Abstractions.Json;

namespace Microsoft.TemplateEngine.Orchestrator.RunnableProjects.ValueForms
{
    public class ReplacementValueFormModel : IValueForm
    {
        private readonly Regex _match;
        private readonly string _replacment;

        public ReplacementValueFormModel()
        {
        }

        public ReplacementValueFormModel(string name, string pattern, string replacement)
        {
            _match = new Regex(pattern);
            _replacment = replacement;
            Name = name;
        }

        public string Identifier => "replace";

        public string Name { get; }

        public IValueForm FromJson(string name, IJsonObject configuration)
        {
            return new ReplacementValueFormModel(name, configuration.ToString("pattern"), configuration.ToString("replacement"));
        }

        public string Process(IReadOnlyDictionary<string, IValueForm> forms, string value)
        {
            return _match.Replace(value, _replacment);
        }
    }
}
