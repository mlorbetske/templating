using System;
using System.Collections.Generic;
using Microsoft.TemplateEngine.Abstractions.Json;

namespace Microsoft.TemplateEngine.Orchestrator.RunnableProjects
{
    public class GeneratedSymbol : ISymbolModel
    {
        internal const string TypeName = "generated";

        public string DataType { get; set; }

        public string Binding { get; set; }

        public string Replaces { get; set; }

        // Refers to the Type property value of a concrete IMacro
        public string Generator { get; set; }

        public IReadOnlyDictionary<string, IJsonToken> Parameters { get; set; }

        public string Type { get; set; }

        public IReadOnlyList<IReplacementContext> ReplacementContexts { get; set; }

        public static GeneratedSymbol FromJson(IJsonObject json)
        {
            GeneratedSymbol sym = new GeneratedSymbol
            {
                Binding = json.ToString(nameof(Binding)),
                Generator = json.ToString(nameof(Generator)),
                DataType = json.ToString(nameof(DataType)),
                Parameters = json.ToJsonTokenDictionary(StringComparer.Ordinal, nameof(Parameters)),
                Type = json.ToString(nameof(Type)),
                Replaces = json.ToString(nameof(Replaces)),
                ReplacementContexts = SymbolModelConverter.ReadReplacementContexts(json)
            };

            return sym;
        }
    }
}
