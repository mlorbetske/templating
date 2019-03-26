using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.TemplateEngine.Abstractions;
using Microsoft.TemplateEngine.Abstractions.Json;
using Microsoft.TemplateEngine.Abstractions.Mount;
using Microsoft.TemplateEngine.Core.Contracts;
using Microsoft.TemplateEngine.Utils;
using Microsoft.TemplateEngine.Utils.Json;

namespace Microsoft.TemplateEngine.Orchestrator.RunnableProjects
{
    internal class SimpleConfigModelJsonModel : IJsonSerializable<SimpleConfigModelJsonModel>
    {
        public IJsonBuilder<SimpleConfigModelJsonModel> JsonBuilder { get; } = new JsonBuilder<SimpleConfigModelJsonModel, SimpleConfigModelJsonModel>(() => new SimpleConfigModelJsonModel())
            .Map(p => p.Author)
            .ListOfString().Map(p => p.Classifications)
            .Map(p => p.DefaultName)
            .Map(p => p.Description)
            .Map(p => p.GroupIdentity)
            .Map(p => p.Precedence)
            .ListOfGuid().Map(p => p.Guids)
            .Map(p => p.Identity)
            .Map(p => p.Name)
            .Map(p => p.SourceName)
            .Map(p => p.PlaceholderFilename)
            .Map(p => p.GeneratorVersions)
            .ListOfString().Map(p => p.ShortNameList)
            .List<SimpleConfigModelJsonModel, SimpleConfigModelJsonModel, ExtendedFileSource>().Map(p => p.Sources, () => new List<ExtendedFileSource>())
            .Dictionary<SimpleConfigModelJsonModel, SimpleConfigModelJsonModel, IBaselineInfo, BaselineCacheInfo>().Map(p => p.Baselines, () => new Dictionary<string, IBaselineInfo>(StringComparer.Ordinal))
            .Dictionary<SimpleConfigModelJsonModel, SimpleConfigModelJsonModel, ICacheTag, CacheTag>().Map(p => p.Tags, () => new Dictionary<string, ICacheTag>(StringComparer.Ordinal))
            .Dictionary<SimpleConfigModelJsonModel, SimpleConfigModelJsonModel, ISymbolModel, ParameterSymbol>().Map(p => p.Symbols, () => new Dictionary<string, ISymbolModel>(StringComparer.Ordinal));


        public string Author { get; set; }

        public List<string> Classifications { get; set; }

        public string DefaultName { get; set; }

        public string Description { get; set; }

        public string GroupIdentity { get; set; }

        public int Precedence { get; set; }

        public List<Guid> Guids { get; set; }

        public string Identity { get; set; }

        public string Name { get; set; }

        public string SourceName { get; set; }

        public string PlaceholderFilename { get; set; }

        public string GeneratorVersions { get; set; }

        public List<string> ShortNameList { get; set; }

        public List<ExtendedFileSource> Sources { get; private set; }

        public Dictionary<string, IBaselineInfo> Baselines { get; set; }

        public Dictionary<string, ICacheTag> Tags { get; set; }

        public Dictionary<string, ISymbolModel> Symbols { get; set; }
    }
}
