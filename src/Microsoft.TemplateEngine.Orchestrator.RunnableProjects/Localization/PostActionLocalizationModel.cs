using System;
using System.Collections.Generic;
using Microsoft.TemplateEngine.Abstractions;
using Microsoft.TemplateEngine.Abstractions.Json;

namespace Microsoft.TemplateEngine.Orchestrator.RunnableProjects.Localization
{
    public class PostActionLocalizationModel : IPostActionLocalizationModel
    {
        // correlates to the culture-neutral action
        public Guid ActionId { get; set; }

        public string Description { get; set; }

        // The order corresponds to the order of the instructions in the same action
        // in the culture neutral TemplateConfigFile
        public IReadOnlyList<string> Instructions { get; set; }

        public static PostActionLocalizationModel FromJson(IJsonObject postActionSection)
        {
            return new PostActionLocalizationModel()
            {
                ActionId = postActionSection.ToGuid(nameof(ActionId)),
                Description = postActionSection.ToString(nameof(Description)),
                Instructions = postActionSection.ArrayAsStrings("manualinstructions")
            };
        }
    }
}
