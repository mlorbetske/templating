using System.Collections.Generic;
using Microsoft.TemplateEngine.Abstractions.Json;

namespace Microsoft.TemplateEngine.Abstractions
{
    public interface ICacheTag : IJsonSerializable<ICacheTag>
    {
        string Description { get; }

        IReadOnlyDictionary<string, string> ChoicesAndDescriptions { get; }

        string DefaultValue { get; }
    }
}
