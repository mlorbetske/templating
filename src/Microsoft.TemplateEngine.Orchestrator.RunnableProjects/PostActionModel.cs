using System;
using System.Collections.Generic;
using Microsoft.TemplateEngine.Abstractions;
using Microsoft.TemplateEngine.Abstractions.Json;

namespace Microsoft.TemplateEngine.Orchestrator.RunnableProjects
{
    public class PostActionModel : ConditionedConfigurationElementBase, IPostActionModel
    {
        public string Description { get; private set; }

        public Guid ActionId { get; private set; }

        public bool ContinueOnError { get; private set; }

        public IReadOnlyDictionary<string, string> Args { get; private set; }

        // Each key value pair represents a manual instruction option.
        // The key is the text of the instruction
        // The value is a conditional to evaluate to determine if the instruction is valid in this context.
        // The instructions get resolved when turning the model into the actual - at most 1 will be chosen.
        public IReadOnlyList<KeyValuePair<string, string>> ManualInstructionInfo { get; private set; }

        public string ConfigFile { get; private set; }

        public static IReadOnlyList<IPostActionModel> ListFromJson(IJsonArray json, IReadOnlyDictionary<Guid, IPostActionLocalizationModel> localizations)
        {
            List<IPostActionModel> modelList = new List<IPostActionModel>();

            if (json == null)
            {
                return modelList;
            }

            foreach (IJsonToken action in json)
            {
                Guid actionId = action.ToGuid(nameof(ActionId));
                IPostActionLocalizationModel actionLocalizations;
                if (localizations == null || !localizations.TryGetValue(actionId, out actionLocalizations))
                {
                    actionLocalizations = null;
                }

                Dictionary<string, string> args = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                foreach (KeyValuePair<string, IJsonToken> argInfo in action.PropertiesOf("Args"))
                {
                    args.Add(argInfo.Key, ((IJsonValue)argInfo.Value).Value.ToString());
                }

                List<KeyValuePair<string, string>> instructionOptions = new List<KeyValuePair<string, string>>();

                IJsonArray manualInstructions = action.Get<IJsonArray>("ManualInstructions");
                bool useLocalizedInstructions =
                    actionLocalizations != null
                    && manualInstructions != null
                    && actionLocalizations.Instructions.Count == manualInstructions.Count;

                if (manualInstructions != null)
                {
                    for (int i = 0; i < manualInstructions.Count; i++)
                    {
                        string text;
                        if (useLocalizedInstructions)
                        {
                            text = actionLocalizations.Instructions[i];
                        }
                        else
                        {
                            text = manualInstructions[i].ToString("text");
                        }

                        KeyValuePair<string, string> instruction = new KeyValuePair<string, string>(text, manualInstructions[i].ToString("condition"));
                        instructionOptions.Add(instruction);
                    }
                }

                PostActionModel model = new PostActionModel()
                {
                    Condition = action.ToString(nameof(model.Condition)),
                    Description = actionLocalizations?.Description ?? action.ToString(nameof(model.Description)),
                    ActionId = actionId,
                    ContinueOnError = action.ToBool(nameof(model.ContinueOnError)),
                    Args = args,
                    ManualInstructionInfo = instructionOptions,
                    ConfigFile = action.ToString(nameof(model.ConfigFile))
                };

                modelList.Add(model);
            }

            return modelList;
        }
    }
}
