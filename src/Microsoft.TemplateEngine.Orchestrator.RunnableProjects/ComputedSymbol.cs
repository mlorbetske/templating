using System.Collections.Generic;
using Microsoft.TemplateEngine.Abstractions.Json;
using Microsoft.TemplateEngine.Utils;

namespace Microsoft.TemplateEngine.Orchestrator.RunnableProjects
{
    public class ComputedSymbol : ISymbolModel
    {
        internal const string TypeName = "computed";

        public string DataType { get; private set; }

        public string Value { get; internal set; }

        public string Type { get; private set; }

        public string Evaluator { get; private set; }

        public IReadOnlyList<IReplacementContext> ReplacementContexts => Empty<IReplacementContext>.List.Value;

        string ISymbolModel.Binding
        {
            get { return null; }
            set { }
        }

        string ISymbolModel.Replaces
        {
            get { return null; }
            set { }
        }

        public static ISymbolModel FromJson(IJsonObject json)
        {
            ComputedSymbol sym = new ComputedSymbol
            {
                DataType = json.ToString(nameof(DataType)),
                Value = json.ToString(nameof(Value)),
                Type = json.ToString(nameof(Type)),
                Evaluator = json.ToString(nameof(Evaluator))
            };

            return sym;
        }
    }
}
