using System;
using Microsoft.TemplateEngine.Abstractions.Json;

namespace Microsoft.TemplateEngine.Edge.Settings.TemplateInfoReaders
{
    public class TemplateInfoReaderVersion1_0_0_3 : TemplateInfoReaderVersion1_0_0_2
    {
        public static new TemplateInfo FromJson(IJsonObject json)
        {
            TemplateInfoReaderVersion1_0_0_3 reader = new TemplateInfoReaderVersion1_0_0_3();
            return reader.Read(json);
        }

        public override TemplateInfo Read(IJsonObject json)
        {
            TemplateInfo info = base.Read(json);
            info.ConfigTimestampUtc = (DateTime?) json[nameof(TemplateInfo.ConfigTimestampUtc)];
            return info;
        }
    }
}
