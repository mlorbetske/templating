using System.Collections.Generic;
using Microsoft.TemplateEngine.Abstractions.Json;

namespace Microsoft.TemplateEngine.Abstractions
{
    public interface IBaselineInfo : IJsonSerializable<IBaselineInfo>
    {
        string Description { get; }

        IReadOnlyDictionary<string, string> DefaultOverrides { get; }
    }
}
