using Microsoft.TemplateEngine.Abstractions.Json;

namespace Microsoft.TemplateEngine.Edge.Settings.TemplateInfoReaders
{
    public class TemplateInfoReaderVersion1_0_0_1 : TemplateInfoReaderVersion1_0_0_0
    {
        public static new TemplateInfo FromJson(IJsonObject json)
        {
            TemplateInfoReaderVersion1_0_0_1 reader = new TemplateInfoReaderVersion1_0_0_1();
            return reader.Read(json);
        }

        public override TemplateInfo Read(IJsonObject json)
        {
            TemplateInfo info = base.Read(json);
            info.HasScriptRunningPostActions = json.ToBool(nameof(TemplateInfo.HasScriptRunningPostActions));

            return info;
        }
    }
}
