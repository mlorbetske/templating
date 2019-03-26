using Microsoft.TemplateEngine.Abstractions.Json;
using Microsoft.TemplateEngine.Core.Contracts;

namespace Microsoft.TemplateEngine.Orchestrator.RunnableProjects
{
    public class CustomOperationModel : ICustomOperationModel
    {
        public string Type { get; set; }

        public string Condition { get; set; }

        public IJsonObject Configuration { get; set; }

        public static ICustomOperationModel FromJson(IJsonObject json)
        {
            CustomOperationModel model = new CustomOperationModel
            {
                Type = json.ToString(nameof(Type)),
                Condition = json.ToString(nameof(Condition)),
                Configuration = json.Get<IJsonObject>(nameof(Configuration)),
            };

            return model;
        }
    }
}
