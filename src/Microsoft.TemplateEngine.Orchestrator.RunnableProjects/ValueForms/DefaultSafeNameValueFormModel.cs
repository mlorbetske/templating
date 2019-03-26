using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.TemplateEngine.Abstractions.Json;

namespace Microsoft.TemplateEngine.Orchestrator.RunnableProjects.ValueForms
{
    public class DefaultSafeNameValueFormModel : IValueForm
    {
        private readonly string _name;

        public DefaultSafeNameValueFormModel()
            : this(null)
        {
        }

        public DefaultSafeNameValueFormModel(string name)
        {
            _name = name;
        }

        public static readonly string FormName = "safe_name";

        public virtual string Identifier => FormName;

        public virtual string Name => _name ?? Identifier;

        public virtual IValueForm FromJson(string name, IJsonObject configuration)
        {
            return new DefaultSafeNameValueFormModel(name);
        }

        public virtual string Process(IReadOnlyDictionary<string, IValueForm> forms, string value)
        {
            string workingValue = Regex.Replace(value, @"(^\s+|\s+$)", "");
            workingValue = Regex.Replace(workingValue, @"(((?<=\.)|^)(?=\d)|\W)", "_");

            return workingValue;
        }
    }
}
