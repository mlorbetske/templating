using System;
using System.Collections.Generic;
using Microsoft.TemplateEngine.Abstractions;
using Microsoft.TemplateEngine.Abstractions.Json;
using Microsoft.TemplateEngine.Utils;

namespace Microsoft.TemplateEngine.Edge.Settings.TemplateInfoReaders
{
    public static class TemplateInfoReaderInitialVersion
    {
        public static TemplateInfo FromJson(IJsonObject entry)
        {
            TemplateInfo info = new TemplateInfo();

            info.ConfigMountPointId = Guid.Parse(entry.ToString(nameof(TemplateInfo.ConfigMountPointId)));
            info.Author = entry.ToString(nameof(TemplateInfo.Author));
            IJsonArray classificationsArray = entry.Get<IJsonArray>(nameof(TemplateInfo.Classifications));

            List<string> classifications = new List<string>();
            info.Classifications = classifications;
            //using (Timing.Over("Read classifications"))
            foreach (IJsonToken item in classificationsArray)
            {
                classifications.Add(((IJsonValue)item).Value.ToString());
            }

            info.DefaultName = entry.ToString(nameof(TemplateInfo.DefaultName));
            info.Description = entry.ToString(nameof(TemplateInfo.Description));
            info.Identity = entry.ToString(nameof(TemplateInfo.Identity));
            info.GeneratorId = Guid.Parse(entry.ToString(nameof(TemplateInfo.GeneratorId)));
            info.GroupIdentity = entry.ToString(nameof(TemplateInfo.GroupIdentity));
            info.Name = entry.ToString(nameof(TemplateInfo.Name));
            info.ShortName = entry.ToString(nameof(TemplateInfo.ShortName));

            // tags are just "name": "description"
            // e.g.: "language": "C#"
            IJsonObject tagsObject = entry.Get<IJsonObject>(nameof(TemplateInfo.Tags));
            Dictionary<string, ICacheTag> tags = new Dictionary<string, ICacheTag>();
            info.Tags = tags;
            foreach (KeyValuePair<string, IJsonToken> item in tagsObject.Properties())
            {
                Dictionary<string, string> choicesAndDescriptions = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                choicesAndDescriptions.Add(((IJsonValue)item.Value).Value.ToString(), string.Empty);
                ICacheTag cacheTag = new CacheTag(
                    string.Empty,       // description
                    choicesAndDescriptions,
                    ((IJsonValue)item.Value).Value.ToString());

                tags.Add(item.Key, cacheTag);
            }

            info.ConfigPlace = entry.ToString(nameof(TemplateInfo.ConfigPlace));
            info.LocaleConfigMountPointId = Guid.Parse(entry.ToString(nameof(TemplateInfo.LocaleConfigMountPointId)));
            info.LocaleConfigPlace = entry.ToString(nameof(TemplateInfo.LocaleConfigPlace));

            return info;
        }
    }
}
