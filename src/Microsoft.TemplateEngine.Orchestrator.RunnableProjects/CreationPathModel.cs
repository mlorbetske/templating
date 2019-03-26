using System.Collections.Generic;
using Microsoft.TemplateEngine.Abstractions.Json;

namespace Microsoft.TemplateEngine.Orchestrator.RunnableProjects
{
    public class CreationPathModel : ConditionedConfigurationElementBase, ICreationPathModel
    {
        public string PathOriginal { get; set; }

        public string PathResolved { get; set; }

        public static IReadOnlyList<ICreationPathModel> ListFromJson(IJsonArray jsonData)
        {
            List<ICreationPathModel> modelList = new List<ICreationPathModel>();

            if (jsonData == null)
            {
                return modelList;
            }

            foreach (IJsonToken pathInfo in jsonData)
            {
                ICreationPathModel pathModel = new CreationPathModel()
                {
                    PathOriginal = pathInfo.ToString("path"),
                    Condition = pathInfo.ToString("condition")
                };

                modelList.Add(pathModel);
            }

            return modelList;
        }
    }
}
