using System;
using System.Collections.Generic;
using Microsoft.TemplateEngine.Abstractions;
using Microsoft.TemplateEngine.Abstractions.Json;
using Microsoft.TemplateEngine.Orchestrator.RunnableProjects.ValueForms;

namespace Microsoft.TemplateEngine.Orchestrator.RunnableProjects
{
    public abstract class BaseValueSymbol : ISymbolModel
    {
        public string Binding { get; set; }

        public string DefaultValue { get; set; }

        public string Description { get; set; }

        public SymbolValueFormsModel Forms { get; set; }

        public bool IsRequired { get; set; }

        public string Type { get; protected set; }

        public string Replaces { get; set; }

        public string DataType { get; set; }

        public string FileRename { get; set; }

        public IReadOnlyList<IReplacementContext> ReplacementContexts { get; set; }

        protected static T FromJson<T>(IJsonObject json, IParameterSymbolLocalizationModel localization, string defaultOverride)
            where T: BaseValueSymbol, new()
        {
            T symbol = new T
            {
                Binding = json.ToString(nameof(Binding)),
                DefaultValue = defaultOverride ?? json.ToString(nameof(DefaultValue)),
                Description = localization?.Description ?? json.ToString(nameof(Description)) ?? string.Empty,
                FileRename = json.ToString(nameof(FileRename)),
                IsRequired = json.ToBool(nameof(IsRequired)),
                Type = json.ToString(nameof(Type)),
                Replaces = json.ToString(nameof(Replaces)),
                DataType = json.ToString(nameof(DataType)),
                ReplacementContexts = SymbolModelConverter.ReadReplacementContexts(json)
            };

            if (!json.TryGetValue(nameof(symbol.Forms), StringComparison.OrdinalIgnoreCase, out IJsonToken formsToken) || !(formsToken is IJsonObject formsObject))
            {
                // no value forms explicitly defined, use the default ("identity")
                symbol.Forms = SymbolValueFormsModel.Default;
            }
            else
            {
                // the config defines forms for the symbol. Use them.
                symbol.Forms = SymbolValueFormsModel.FromJson(formsObject);
            }

            return symbol;
        }
    }
}
