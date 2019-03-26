// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Microsoft.TemplateEngine.Abstractions.Json;
using Microsoft.TemplateEngine.Utils.Json;

namespace Microsoft.TemplateEngine.Cli
{
    public class HostSpecificTemplateData : IJsonSerializable<HostSpecificTemplateData>
    {
        private const string AlwaysShowKey = "alwaysShow";
        private const string IsHiddenKey = "isHidden";
        private const string LongNameKey = "longName";
        private const string ShortNameKey = "shortName";

        public static HostSpecificTemplateData Default { get; } = new HostSpecificTemplateData();

        public HashSet<string> HiddenParameterNames
        {
            get
            {
                HashSet<string> hiddenNames = new HashSet<string>(StringComparer.Ordinal);
                foreach (KeyValuePair<string, IReadOnlyDictionary<string, string>> paramInfo in SymbolInfo)
                {
                    if (paramInfo.Value.TryGetValue(IsHiddenKey, out string hiddenStringValue)
                        && bool.TryParse(hiddenStringValue, out bool hiddenBoolValue)
                        && hiddenBoolValue)
                    {
                        hiddenNames.Add(paramInfo.Key);
                    }
                }

                return hiddenNames;
            }
        }

        public bool IsHidden { get; set; }

        public IJsonBuilder<HostSpecificTemplateData> JsonBuilder => new JsonBuilder<HostSpecificTemplateData, HostSpecificTemplateData>(() => new HostSpecificTemplateData())
            .ListOfString().Map<IReadOnlyList<string>, List<string>>(p => p.UsageExamples)
            .Map(p => p.IsHidden)
            .Dictionary(Wrapper.For<IReadOnlyDictionary<string, string>, Dictionary<string, string>>(builder => builder.DictionaryOfString().Map<IReadOnlyDictionary<string, string>, Dictionary<string, string>>(p => p.Value)))
                .Map<IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>>, Dictionary<string, IReadOnlyDictionary<string, string>>>(p => p.SymbolInfo);

        public Dictionary<string, string> LongNameOverrides
        {
            get
            {
                Dictionary<string, string> map = new Dictionary<string, string>();

                foreach (KeyValuePair<string, IReadOnlyDictionary<string, string>> paramInfo in SymbolInfo)
                {
                    if (paramInfo.Value.TryGetValue(LongNameKey, out string longNameOverride))
                    {
                        map.Add(paramInfo.Key, longNameOverride);
                    }
                }

                return map;
            }
        }

        public HashSet<string> ParametersToAlwaysShow
        {
            get
            {
                HashSet<string> parametersToAlwaysShow = new HashSet<string>(StringComparer.Ordinal);
                foreach (KeyValuePair<string, IReadOnlyDictionary<string, string>> paramInfo in SymbolInfo)
                {
                    if (paramInfo.Value.TryGetValue(AlwaysShowKey, out string alwaysShowValue)
                        && bool.TryParse(alwaysShowValue, out bool alwaysShowBoolValue)
                        && alwaysShowBoolValue)
                    {
                        parametersToAlwaysShow.Add(paramInfo.Key);
                    }
                }

                return parametersToAlwaysShow;
            }
        }

        public Dictionary<string, string> ShortNameOverrides
        {
            get
            {
                Dictionary<string, string> map = new Dictionary<string, string>();

                foreach (KeyValuePair<string, IReadOnlyDictionary<string, string>> paramInfo in SymbolInfo)
                {
                    if (paramInfo.Value.TryGetValue(ShortNameKey, out string shortNameOverride))
                    {
                        map.Add(paramInfo.Key, shortNameOverride);
                    }
                }

                return map;
            }
        }

        public IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> SymbolInfo { get; private set; }

        public List<string> UsageExamples { get; set; }

        public string DisplayNameForParameter(string parameterName)
        {
            if (SymbolInfo.TryGetValue(parameterName, out IReadOnlyDictionary<string, string> configForParam)
                && configForParam.TryGetValue(LongNameKey, out string longName))
            {
                return longName;
            }

            return parameterName;
        }
    }
}
