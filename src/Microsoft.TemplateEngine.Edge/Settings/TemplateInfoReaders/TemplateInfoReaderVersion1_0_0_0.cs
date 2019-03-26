using System;
using System.Collections.Generic;
using Microsoft.TemplateEngine.Abstractions;
using Microsoft.TemplateEngine.Abstractions.Json;
using Microsoft.TemplateEngine.Utils;

namespace Microsoft.TemplateEngine.Edge.Settings.TemplateInfoReaders
{
    public class TemplateInfoReaderVersion1_0_0_0
    {
        public static TemplateInfo FromJson(IJsonObject json)
        {
            TemplateInfoReaderVersion1_0_0_0 reader = new TemplateInfoReaderVersion1_0_0_0();
            return reader.Read(json);
        }

        public virtual TemplateInfo Read(IJsonObject json)
        {
            TemplateInfo info = new TemplateInfo();

            ReadPrimaryInformation(json, info);
            info.Tags = ReadTags(json);
            info.CacheParameters = ReadParameters(json);

            return info;
        }

        protected void ReadPrimaryInformation(IJsonObject json, TemplateInfo info)
        {
            info.ConfigMountPointId = Guid.Parse(json.ToString(nameof(TemplateInfo.ConfigMountPointId)));
            info.Author = json.ToString(nameof(TemplateInfo.Author));
            IJsonArray classificationsArray = json.Get<IJsonArray>(nameof(TemplateInfo.Classifications));

            List<string> classifications = new List<string>();
            info.Classifications = classifications;
            //using (Timing.Over("Read classifications"))
            foreach (IJsonToken item in classificationsArray)
            {
                classifications.Add(((IJsonValue)item).Value.ToString());
            }

            info.DefaultName = json.ToString(nameof(TemplateInfo.DefaultName));
            info.Description = json.ToString(nameof(TemplateInfo.Description));
            info.Identity = json.ToString(nameof(TemplateInfo.Identity));
            info.GeneratorId = Guid.Parse(json.ToString(nameof(TemplateInfo.GeneratorId)));
            info.GroupIdentity = json.ToString(nameof(TemplateInfo.GroupIdentity));
            info.Precedence = json.ToInt32(nameof(TemplateInfo.Precedence));
            info.Name = json.ToString(nameof(TemplateInfo.Name));

            ReadShortNameInfo(json, info);

            info.ConfigPlace = json.ToString(nameof(TemplateInfo.ConfigPlace));
            info.LocaleConfigMountPointId = Guid.Parse(json.ToString(nameof(TemplateInfo.LocaleConfigMountPointId)));
            info.LocaleConfigPlace = json.ToString(nameof(TemplateInfo.LocaleConfigPlace));

            info.HostConfigMountPointId = Guid.Parse(json.ToString(nameof(TemplateInfo.HostConfigMountPointId)));
            info.HostConfigPlace = json.ToString(nameof(TemplateInfo.HostConfigPlace));
            info.ThirdPartyNotices = json.ToString(nameof(TemplateInfo.ThirdPartyNotices));

            IJsonObject baselineJson = json.Get<IJsonObject>(nameof(ITemplateInfo.BaselineInfo));
            Dictionary<string, IBaselineInfo> baselineInfo = new Dictionary<string, IBaselineInfo>();
            if (baselineJson != null)
            {
                foreach (KeyValuePair<string, IJsonToken> item in baselineJson.Properties())
                {
                    IBaselineInfo baseline = new BaselineCacheInfo()
                    {
                        Description = item.Value.ToString(nameof(IBaselineInfo.Description)),
                        DefaultOverrides = item.Value.ToStringDictionary(propertyName: nameof(IBaselineInfo.DefaultOverrides))
                    };
                    baselineInfo.Add(item.Key, baseline);
                }
            }
            info.BaselineInfo = baselineInfo;
        }

        protected virtual void ReadShortNameInfo(IJsonObject json, TemplateInfo info)
        {
            info.ShortName = json.ToString(nameof(TemplateInfo.ShortName));
        }

        protected virtual IReadOnlyDictionary<string, ICacheTag> ReadTags(IJsonObject json)
        {
            Dictionary<string, ICacheTag> tags = new Dictionary<string, ICacheTag>();
            IJsonObject tagsObject = json.Get<IJsonObject>(nameof(TemplateInfo.Tags));

            if (tagsObject != null)
            {
                foreach (KeyValuePair<string, IJsonToken> item in tagsObject.Properties())
                {
                    tags[item.Key] = ReadOneTag(item);
                }
            }

            return tags;
        }

        protected virtual ICacheTag ReadOneTag(KeyValuePair<string, IJsonToken> item)
        {
            Dictionary<string, string> choicesAndDescriptions = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            IJsonObject cdToken = item.Value.Get<IJsonObject>(nameof(ICacheTag.ChoicesAndDescriptions));
            foreach (KeyValuePair<string, IJsonToken> cdPair in cdToken.Properties())
            {
                choicesAndDescriptions.Add(cdPair.Key, ((IJsonValue)cdPair.Value).Value.ToString());
            }

            return new CacheTag(
                item.Value.ToString(nameof(ICacheTag.Description)),
                choicesAndDescriptions,
                item.Value.ToString(nameof(ICacheTag.DefaultValue)));
        }

        protected virtual IReadOnlyDictionary<string, ICacheParameter> ReadParameters(IJsonObject json)
        {
            Dictionary<string, ICacheParameter> cacheParams = new Dictionary<string, ICacheParameter>();
            IJsonObject cacheParamsObject = json.Get<IJsonObject>(nameof(TemplateInfo.CacheParameters));

            if (cacheParamsObject != null)
            {
                foreach (KeyValuePair<string, IJsonToken> item in cacheParamsObject.Properties())
                {
                    cacheParams[item.Key] = ReadOneParameter(item);
                }
            }

            return cacheParams;
        }

        protected virtual ICacheParameter ReadOneParameter(KeyValuePair<string, IJsonToken> item)
        {
            return new CacheParameter
            {
                DataType = item.Value.ToString(nameof(ICacheParameter.DataType)),
                DefaultValue = item.Value.ToString(nameof(ICacheParameter.DefaultValue)),
                Description = item.Value.ToString(nameof(ICacheParameter.Description))
            };
        }
    }
}
