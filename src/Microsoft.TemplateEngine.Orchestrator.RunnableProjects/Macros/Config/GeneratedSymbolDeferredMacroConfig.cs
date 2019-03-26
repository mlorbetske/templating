using System;
using System.Collections.Generic;
using Microsoft.TemplateEngine.Abstractions.Json;
using Microsoft.TemplateEngine.Core.Contracts;

namespace Microsoft.TemplateEngine.Orchestrator.RunnableProjects.Macros.Config
{
    public class GeneratedSymbolDeferredMacroConfig : IMacroConfig
    {
        public Guid Id => new Guid("12CA34F3-A1B7-4859-B08C-172483C9B0FD");

        public string VariableName { get; private set; }

        public string DataType { get; }

        // comes from GeneratedSymbol.Generator
        // note that for all generated symbols, GeneratedSymbol.Type = "generated"
        public string Type { get; private set;  }

        public IReadOnlyDictionary<string, IJsonToken> Parameters { get; private set; }

        public GeneratedSymbolDeferredMacroConfig(string type, string dataType, string variableName, Dictionary<string, IJsonToken> parameters)
        {
            DataType = dataType;
            Type = type;
            VariableName = variableName;
            Parameters = parameters;
        }
    }
}
