using Microsoft.TemplateEngine.Abstractions;
using Microsoft.TemplateEngine.Abstractions.Json;

namespace Microsoft.TemplateEngine.Orchestrator.RunnableProjects
{
    public class DerivedSymbol : BaseValueSymbol
    {
        internal const string TypeName = "derived";

        public string ValueTransform { get; set; }

        public string ValueSource { get; set; }

        public static ISymbolModel FromJson(IJsonObject json, IParameterSymbolLocalizationModel localization, string defaultOverride)
        {
            DerivedSymbol symbol = FromJson<DerivedSymbol>(json, localization, defaultOverride);

            symbol.ValueTransform = json.ToString(nameof(ValueTransform));
            symbol.ValueSource = json.ToString(nameof(ValueSource));

            return symbol;
        }
    }
}
