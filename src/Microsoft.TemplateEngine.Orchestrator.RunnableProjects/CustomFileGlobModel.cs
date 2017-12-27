using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.TemplateEngine.Core.Contracts;
using Microsoft.TemplateEngine.Orchestrator.RunnableProjects.Config;
using Newtonsoft.Json.Linq;

namespace Microsoft.TemplateEngine.Orchestrator.RunnableProjects
{
    public class CustomFileGlobModel : ConditionedConfigurationElementBase, ICustomFileGlobModelWithServiceList
    {
        public string Glob { get; set; }

        public IReadOnlyList<ICustomOperationModel> Operations { get; set; }

        // TODO: reference to built-in conditional config ???

        public IVariableConfig VariableFormat { get; set; }

        public string FlagPrefix { get; set; }

        public static CustomFileGlobModel FromJObject(JObject globData, string globName)
        {
            // setup the variable config
            IVariableConfig variableConfig;
            if (globData.TryGetValue(nameof(VariableFormat), StringComparison.OrdinalIgnoreCase, out JToken variableData))
            {
                variableConfig = VariableConfig.FromJObject((JObject)variableData);
            }
            else
            {
                variableConfig = VariableConfig.DefaultVariableSetup(null);
            }

            // setup the custom operations
            List<ICustomOperationModel> customOpsForGlob = new List<ICustomOperationModel>();
            if (globData.TryGetValue("Operations", StringComparison.OrdinalIgnoreCase, out JToken operationData))
            {
                foreach (JObject operationConfig in ((JArray)operationData).OfType<JObject>())
                {
                    customOpsForGlob.Add(CustomOperationModel.FromJObject(operationConfig));
                }
            }

            IReadOnlyList<Guid> serviceList = null;
            if (globData.TryGetValue("Services", StringComparison.OrdinalIgnoreCase, out JToken services))
            {
                serviceList = services.ArrayAsGuids();
            }

            CustomFileGlobModel globModel = new CustomFileGlobModel()
            {
                Glob = globName,
                Operations = customOpsForGlob,
                VariableFormat = variableConfig,
                FlagPrefix = globData.ToString(nameof(FlagPrefix)),
                Condition = globData.ToString(nameof(Condition)),
                AvailableServices = serviceList,
            };

            return globModel;
        }

        public IReadOnlyList<Guid> AvailableServices { get; private set; }
    }
}
