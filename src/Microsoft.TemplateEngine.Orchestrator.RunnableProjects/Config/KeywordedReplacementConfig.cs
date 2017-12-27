using System;
using System.Collections.Generic;
using Microsoft.TemplateEngine.Abstractions.Mount;
using Microsoft.TemplateEngine.Core;
using Microsoft.TemplateEngine.Core.Contracts;
using Microsoft.TemplateEngine.Core.Operations;
using Newtonsoft.Json.Linq;

namespace Microsoft.TemplateEngine.Orchestrator.RunnableProjects.Config
{
    public class KeywordedReplacementConfig : IOperationConfigWithServiceContainerAccess
    {
        public string Key => KeywordedReplacement.OperationName;

        IEnumerable<IOperationProvider> IOperationConfig.ConfigureFromJObject(JObject rawConfiguration, IDirectory templateRoot)
        {
            return ConfigureFromJObject(null, rawConfiguration, templateRoot);
        }

        public Guid Id => new Guid("765501AC-0023-418F-A09E-4E8FC9B080D1");

        public IEnumerable<IOperationProvider> ConfigureFromJObject(IServiceContainer container, JObject rawConfiguration, IDirectory templateRoot)
        {
            if (container == null || !container.TryGetService(out IKeyworder keyworder))
            {
                yield break;
            }

            string original = rawConfiguration.ToString("original");
            string replacement = rawConfiguration.ToString("replacement");
            string id = rawConfiguration.ToString("id");
            bool onByDefault = rawConfiguration.ToBool("onByDefault");

            JArray onlyIf = rawConfiguration.Get<JArray>("onlyIf");
            TokenConfig coreConfig = original.TokenConfigBuilder();

            if (onlyIf != null)
            {
                foreach (JToken entry in onlyIf.Children())
                {
                    if (!(entry is JObject))
                    {
                        continue;
                    }

                    string before = entry.ToString("before");
                    string after = entry.ToString("after");
                    TokenConfig entryConfig = coreConfig;

                    if (!string.IsNullOrEmpty(before))
                    {
                        entryConfig = entryConfig.OnlyIfBefore(before);
                    }

                    if (!string.IsNullOrEmpty(after))
                    {
                        entryConfig = entryConfig.OnlyIfAfter(after);
                    }

                    yield return new KeywordedReplacement(keyworder, entryConfig, replacement, id, onByDefault);
                }
            }
            else
            {
                yield return new KeywordedReplacement(keyworder, coreConfig, replacement, id, onByDefault);
            }
        }
    }
}
