using System.Collections.Generic;
using Microsoft.TemplateEngine.Abstractions.Json;

namespace Microsoft.TemplateEngine.Orchestrator.RunnableProjects.ValueForms
{
    public class DefaultLowerSafeNamespaceValueFormModel : DefaultSafeNamespaceValueFormModel
    {
        public new static readonly string FormName = "lower_safe_namespace";
        private readonly string _name;

        public DefaultLowerSafeNamespaceValueFormModel()
            : base()
        {
        }

        public DefaultLowerSafeNamespaceValueFormModel(string name)
            : base(name)
        {
            _name = name;
        }

        public override string Identifier => _name ?? FormName;

        public override string Process(IReadOnlyDictionary<string, IValueForm> forms, string value)
        {
            return base.Process(forms, value).ToLowerInvariant();
        }

        public override IValueForm FromJson(string name, IJsonObject configuration)
        {
            return new DefaultLowerSafeNamespaceValueFormModel(name);
        }
    }
}
