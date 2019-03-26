using System;
using System.Collections.Generic;
using Microsoft.TemplateEngine.Abstractions;
using Microsoft.TemplateEngine.Abstractions.Json;
using Microsoft.TemplateEngine.Utils.Json;

namespace Microsoft.TemplateEngine.Utils
{
    public class CacheTag : ICacheTag, IAllowDefaultIfOptionWithoutValue
    {
        public CacheTag()
        {
        }

        public CacheTag(string description, IReadOnlyDictionary<string, string> choicesAndDescriptions, string defaultValue)
            : this(description, choicesAndDescriptions, defaultValue, null)
        {
        }

        public CacheTag(string description, IReadOnlyDictionary<string, string> choicesAndDescriptions, string defaultValue, string defaultIfOptionWithoutValue)
        {
            Description = description;
            ChoicesAndDescriptions = choicesAndDescriptions.CloneIfDifferentComparer(StringComparer.OrdinalIgnoreCase);
            DefaultValue = defaultValue;
            DefaultIfOptionWithoutValue = defaultIfOptionWithoutValue;
        }

        public string Description { get; }

        public IReadOnlyDictionary<string, string> ChoicesAndDescriptions { get; }

        public string DefaultValue { get; }

        public string DefaultIfOptionWithoutValue { get; set; }

        public IJsonBuilder<ICacheTag> JsonBuilder { get; } = new JsonBuilder<ICacheTag, CacheTag>(() => new CacheTag())
           .Map(p => p.Description)
           .DictionaryOfString().Map<IReadOnlyDictionary<string, string>, Dictionary<string, string>>(p => p.ChoicesAndDescriptions)
           .Map(p => p.DefaultValue)
           .Map(p => p.DefaultIfOptionWithoutValue);
    }
}
