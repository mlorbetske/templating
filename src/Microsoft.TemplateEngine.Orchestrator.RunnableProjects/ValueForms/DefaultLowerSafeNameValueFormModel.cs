using System.Collections.Generic;
using Microsoft.TemplateEngine.Abstractions.Json;

namespace Microsoft.TemplateEngine.Orchestrator.RunnableProjects.ValueForms
{
    public class DefaultLowerSafeNameValueFormModel : DefaultSafeNameValueFormModel
    {
        public new static readonly string FormName = "lower_safe_name";
        private readonly string _name;

        public DefaultLowerSafeNameValueFormModel()
            : base()
        {
        }

        public DefaultLowerSafeNameValueFormModel(string name)
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
            return new DefaultLowerSafeNameValueFormModel(name);
        }
    }
}
