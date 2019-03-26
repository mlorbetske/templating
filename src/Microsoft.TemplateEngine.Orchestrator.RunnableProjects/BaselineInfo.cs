using System.Collections.Generic;
using Microsoft.TemplateEngine.Abstractions;
using Microsoft.TemplateEngine.Abstractions.Json;
using Microsoft.TemplateEngine.Utils.Json;

namespace Microsoft.TemplateEngine.Orchestrator.RunnableProjects
{
    public class BaselineInfo : IBaselineInfo
    {
        public string Description { get; set; }

        public IReadOnlyDictionary<string, string> DefaultOverrides { get; set; }

        public IJsonBuilder<IBaselineInfo> JsonBuilder { get; } = new JsonBuilder<IBaselineInfo, BaselineInfo>(() => new BaselineInfo())
            .Map(p => p.Description)
            .DictionaryOfString().Map<IReadOnlyDictionary<string, string>, Dictionary<string, string>>(p => p.DefaultOverrides);
    }
}
