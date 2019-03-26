using System;
using System.Collections.Generic;
using Microsoft.TemplateEngine.Abstractions;
using Microsoft.TemplateEngine.Abstractions.Json;
using Microsoft.TemplateEngine.Utils;

namespace Microsoft.TemplateEngine.Orchestrator.RunnableProjects
{
    internal class SymbolModelConverter
    {
        internal const string BindSymbolTypeName = "bind";

        private static readonly IReadOnlyDictionary<string, Action<IJsonToken, TypeContainer>> ExtractTypeHandler = new Dictionary<string, Action<IJsonToken, TypeContainer>>(StringComparer.Ordinal)
        { { "type", (token, container) => container.Type = ((IJsonValue)token).Value?.ToString() } };

        // Note: Only ParameterSymbol has a Description property, this it's the only one that gets localization
        // TODO: change how localization gets merged in, don't do it here.
        public static ISymbolModel GetModelForObject(IJsonObject json, IParameterSymbolLocalizationModel localization, string defaultOverride)
        {
            switch (json.ToString(nameof(ISymbolModel.Type)))
            {
                case ParameterSymbol.TypeName:
                    return ParameterSymbol.FromJson(json, localization, defaultOverride);
                case DerivedSymbol.TypeName:
                    return DerivedSymbol.FromJson(json, localization, defaultOverride);
                case ComputedSymbol.TypeName:
                    return ComputedSymbol.FromJson(json);
                case BindSymbolTypeName:
                case GeneratedSymbol.TypeName:
                    return GeneratedSymbol.FromJson(json);
                default:
                    return null;
            }
        }

        public static ISymbolModel Deserialize(IJsonToken token)
        {
            if (token.TokenType != JsonTokenType.Object)
            {
                return null;
            }

            IJsonObject json = (IJsonObject)token;
            TypeContainer container = new TypeContainer();
            json.ExtractValues(container, ExtractTypeHandler);

            switch (container.Type)
            {
                case ParameterSymbol.TypeName:
                    return ParameterSymbol.Deserialize(json);
                case DerivedSymbol.TypeName:
                    return DerivedSymbol.Deserialize(json);
                case ComputedSymbol.TypeName:
                    return ComputedSymbol.Deserialize(json);
                case BindSymbolTypeName:
                case GeneratedSymbol.TypeName:
                    return GeneratedSymbol.Deserialize(json);
                default:
                    return null;
            }
        }

        private class TypeContainer
        {
            public string Type { get; set; }
        }

        internal static IReadOnlyList<IReplacementContext> ReadReplacementContexts(IJsonObject json)
        {
            IJsonArray onlyIf = json.Get<IJsonArray>("onlyIf");

            if (onlyIf != null)
            {
                List<IReplacementContext> contexts = new List<IReplacementContext>();
                foreach (IJsonToken entry in onlyIf)
                {
                    if (entry.TokenType != JsonTokenType.Object)
                    {
                        continue;
                    }

                    string before = entry.ToString("before");
                    string after = entry.ToString("after");
                    contexts.Add(new ReplacementContext(before, after));
                }

                return contexts;
            }
            else
            {
                return Empty<IReplacementContext>.List.Value;
            }
        }
    }
}
