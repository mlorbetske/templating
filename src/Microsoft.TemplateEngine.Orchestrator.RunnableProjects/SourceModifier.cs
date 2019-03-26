using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.TemplateEngine.Abstractions.Json;
using Microsoft.TemplateEngine.Abstractions.Mount;
using Microsoft.TemplateEngine.Utils;
using Microsoft.TemplateEngine.Utils.Json;

namespace Microsoft.TemplateEngine.Orchestrator.RunnableProjects
{
    public class IceEntry
    {
        private IReadOnlyList<string> _entries;

        private string _fileSource;

        public IceEntry(IReadOnlyList<string> defaults)
        {
            _entries = defaults;
        }

        public static IceEntry Deserialize(IJsonToken token, Func<IceEntry> creator)
        {
            IceEntry result = creator();

            if (token.TokenType == JsonTokenType.String)
            {
                result._fileSource = (string)((IJsonValue)token).Value;
                result._entries = null;
                return result;
            }

            if (token.TokenType == JsonTokenType.Null)
            {
                result._entries = null;
            }

            List<string> entries = new List<string>();
            foreach (IJsonToken entry in (IJsonArray)token)
            {
                string value = ((IJsonValue)token).Value?.ToString();
                entries.Add(value);
            }

            result._entries = entries;
            return result;
        }

        public static IJsonToken Serialize(IJsonDocumentObjectModelFactory domFactory, IceEntry entry) => null;

        public IReadOnlyList<string> Expand(IFileSystemInfo sourceFile)
        {
            if (string.IsNullOrEmpty(_fileSource))
            {
                return _entries;
            }

            IReadOnlyList<string> entries;
            if ((_fileSource.IndexOfAny(Path.GetInvalidPathChars()) != -1)
                || !sourceFile.Parent.FileInfo(_fileSource).Exists)
            {
                entries = new string[] { _fileSource };
            }
            else
            {
                using (Stream excludeList = sourceFile.Parent.FileInfo(_fileSource).OpenRead())
                using (TextReader reader = new StreamReader(excludeList, Encoding.UTF8, true, 4096, true))
                {
                    entries = reader.ReadToEnd().Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                }
            }

            return entries;
        }
    }

    public class SourceModifier : IJsonSerializable<SourceModifier>
    {
        public string Condition { get; private set; }

        public IceEntry CopyOnly { get; private set; }

        public IceEntry Exclude { get; private set; }

        public IceEntry Include { get; private set; }

        public IJsonBuilder<SourceModifier> JsonBuilder { get; } = new JsonBuilder<SourceModifier, SourceModifier>(() => new SourceModifier())
            .Map(p => p.Condition)
            .Map(p => p.Include, IceEntry.Serialize, IceEntry.Deserialize, () => new IceEntry(IceDeserializerUtil.IncludePatternDefaults))
            .Map(p => p.CopyOnly, IceEntry.Serialize, IceEntry.Deserialize, () => new IceEntry(IceDeserializerUtil.CopyOnlyPatternDefaults))
            .Map(p => p.Exclude, IceEntry.Serialize, IceEntry.Deserialize, () => new IceEntry(IceDeserializerUtil.ExcludePatternDefaults))
            .DictionaryOfString().Map<IReadOnlyDictionary<string, string>, Dictionary<string, string>>(p => p.Rename);

        public IReadOnlyDictionary<string, string> Rename { get; private set; }
    }
}
