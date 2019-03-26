using System.Collections.Generic;
using Microsoft.TemplateEngine.Abstractions;
using Microsoft.TemplateEngine.Abstractions.Json;

namespace Microsoft.TemplateEngine.Orchestrator.RunnableProjects
{
    public interface ISymbolModel : IJsonSerializable<ISymbolModel>
    {
        string Type { get; }

        string Binding { get; set; }

        string Replaces { get; set; }

        IReadOnlyList<IReplacementContext> ReplacementContexts { get; }

        ISymbolModel PrepareForUse(IParameterSymbolLocalizationModel localization, string defaultOverride);
    }
}
