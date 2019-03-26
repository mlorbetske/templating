using System.Collections.Generic;
using Microsoft.TemplateEngine.Abstractions.Json;

namespace Microsoft.TemplateEngine.Orchestrator.RunnableProjects.ValueForms
{
    public class ChainValueFormModel : IValueForm
    {
        private readonly IReadOnlyList<string> _steps;

        public ChainValueFormModel()
        {
        }

        public ChainValueFormModel(string name, IReadOnlyList<string> steps)
        {
            Name = name;
            _steps = steps;
        }

        public string Identifier => "chain";

        public string Name { get; }

        public IValueForm FromJson(string name, IJsonObject configuration)
        {
            return new ChainValueFormModel(name, configuration.ArrayAsStrings("steps"));
        }

        public string Process(IReadOnlyDictionary<string, IValueForm> forms, string value)
        {
            string result = value;

            foreach (string step in _steps)
            {
                result = forms[step].Process(forms, result);
            }

            return result;
        }
    }
}
