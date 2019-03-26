using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.TemplateEngine.Abstractions.Json;
using Microsoft.TemplateEngine.Core.Contracts;

namespace Microsoft.TemplateEngine.Orchestrator.RunnableProjects
{
    public class CustomFileGlobModel : ConditionedConfigurationElementBase, ICustomFileGlobModel
    {
        public string Glob { get; set; }

        public IReadOnlyList<ICustomOperationModel> Operations { get; set; }

        // TODO: reference to built-in conditional config ???

        public IVariableConfig VariableFormat { get; set; }

        public string FlagPrefix { get; set; }

        public static CustomFileGlobModel FromJson(IJsonObject globData, string globName)
        {
            // setup the variable config
            IVariableConfig variableConfig;
            if (globData.TryGetValue(nameof(VariableFormat), StringComparison.OrdinalIgnoreCase, out IJsonToken variableData))
            {
                variableConfig = VariableConfig.FromJson((IJsonObject)variableData);
            }
            else
            {
                variableConfig = VariableConfig.DefaultVariableSetup(null);
            }

            // setup the custom operations
            List<ICustomOperationModel> customOpsForGlob = new List<ICustomOperationModel>();
            if (globData.TryGetValue("Operations", StringComparison.OrdinalIgnoreCase, out IJsonToken operationData))
            {
                foreach (IJsonObject operationConfig in ((IJsonArray)operationData).OfType<IJsonObject>())
                {
                    customOpsForGlob.Add(CustomOperationModel.FromJson(operationConfig));
                }
            }

            CustomFileGlobModel globModel = new CustomFileGlobModel()
            {
                Glob = globName,
                Operations = customOpsForGlob,
                VariableFormat = variableConfig,
                FlagPrefix = globData.ToString(nameof(FlagPrefix)),
                Condition = globData.ToString(nameof(Condition))
            };

            return globModel;
        }
    }
}
