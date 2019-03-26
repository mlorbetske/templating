using System;
using System.Collections.Generic;
using Microsoft.TemplateEngine.Abstractions.Json;
using Microsoft.TemplateEngine.Abstractions.Mount;
using Microsoft.TemplateEngine.Core;
using Microsoft.TemplateEngine.Core.Contracts;
using Microsoft.TemplateEngine.Core.Operations;
using Microsoft.TemplateEngine.Utils.Json;

namespace Microsoft.TemplateEngine.Orchestrator.RunnableProjects.Config
{
    public class BalancedNestingConfig : IOperationConfig
    {
        public string Key => BalancedNesting.OperationName;

        public Guid Id => new Guid("3147965A-08E5-4523-B869-02C8E9A8AAA1");

        public IEnumerable<IOperationProvider> ConfigureFromJson(IJsonObject rawConfiguration, IDirectory templateRoot)
        {
            string startToken = rawConfiguration.ToString("startToken");
            string realEndToken = rawConfiguration.ToString("realEndToken");
            string pseudoEndToken = rawConfiguration.ToString("pseudoEndToken");
            string id = rawConfiguration.ToString("id");
            string resetFlag = rawConfiguration.ToString("resetFlag");
            bool onByDefault = rawConfiguration.ToBool("onByDefault");

            yield return new BalancedNesting(startToken.TokenConfig(), realEndToken.TokenConfig(), pseudoEndToken.TokenConfig(), id, resetFlag, onByDefault);
        }

        public static IJsonObject CreateConfiguration(IJsonDocumentObjectModelFactory domFactory, string startToken, string realEndToken, string pseudoEndToken, string id, string resetFlag)
        {
            IJsonObject config = domFactory.CreateObject();
            config.SetValue("startToken", startToken);
            config.SetValue("realEndToken", realEndToken);
            config.SetValue("pseudoEndToken", pseudoEndToken);
            config.SetValue("id", id);
            config.SetValue("resetFlag", resetFlag);
            return config;
        }
    }
}
