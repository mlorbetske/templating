using System;
using System.Collections.Generic;
using Microsoft.TemplateEngine.Abstractions.Json;
using Microsoft.TemplateEngine.Orchestrator.RunnableProjects.ValueForms;

namespace Microsoft.TemplateEngine.Orchestrator.RunnableProjects
{
    public static class ValueFormRegistry
    {
        private static readonly IReadOnlyDictionary<string, IValueForm> FormLookup = SetupFormLookup();

        private static IReadOnlyDictionary<string, IValueForm> SetupFormLookup()
        {
            Dictionary<string, IValueForm> lookup = new Dictionary<string, IValueForm>(StringComparer.OrdinalIgnoreCase);
            IValueForm x = new ReplacementValueFormModel();
            lookup[x.Identifier] = x;
            x = new ChainValueFormModel();
            lookup[x.Identifier] = x;
            x = new XmlEncodeValueFormModel();
            lookup[x.Identifier] = x;
            x = new JsonEncodeValueFormModel();
            lookup[x.Identifier] = x;
            x = new IdentityValueForm();
            lookup[x.Identifier] = x;

            x = new DefaultSafeNameValueFormModel();
            lookup[x.Identifier] = x;
            x = new DefaultLowerSafeNameValueFormModel();
            lookup[x.Identifier] = x;
            x = new DefaultSafeNamespaceValueFormModel();
            lookup[x.Identifier] = x;
            x = new DefaultLowerSafeNamespaceValueFormModel();
            lookup[x.Identifier] = x;

            x = new LowerCaseValueFormModel();
            lookup[x.Identifier] = x;
            x = new LowerCaseInvariantValueFormModel();
            lookup[x.Identifier] = x;
            x = new UpperCaseValueFormModel();
            lookup[x.Identifier] = x;
            x = new UpperCaseInvariantValueFormModel();
            lookup[x.Identifier] = x;

            return lookup;
        }

        public static IReadOnlyDictionary<string, IValueForm> AllForms
        {
            get
            {
                return FormLookup;
            }
        }

        public static IValueForm GetForm(string name, IJsonObject obj)
        {
            string identifier = obj.ToString("identifier");

            if (!FormLookup.TryGetValue(identifier, out IValueForm value))
            {
                return FormLookup[IdentityValueForm.FormName].FromJson(name, obj);
            }

            return value.FromJson(name, obj);
        }
    }
}
