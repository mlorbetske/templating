using System;
using System.Collections.Generic;
using Microsoft.TemplateEngine.Abstractions;
using Microsoft.TemplateEngine.Abstractions.Json;
using Microsoft.TemplateEngine.Utils.Json;

namespace Microsoft.TemplateEngine.Edge.Settings
{
    internal static class TemplateCacheJsonSerializer
    {
        public static bool TrySerialize(IJsonDocumentObjectModelFactory domFactory, TemplateCache cache, out string serialized)
        {
            IJsonObject serializedCache = domFactory.CreateObject();

            List<IJsonObject> templateObjects = new List<IJsonObject>();
            foreach (TemplateInfo template in cache.TemplateInfo)
            {
                if (TrySerializeTemplate(domFactory, template, out IJsonObject serializedTemplate))
                {
                    templateObjects.Add(serializedTemplate);
                }
                else
                {
                    serialized = null;
                    return false;
                }
            }

            serializedCache.SetValue(nameof(TemplateCache.TemplateInfo), templateObjects);

            serialized = serializedCache.GetJsonString();
            return true;
        }

        private static bool TrySerializeTemplate(IJsonDocumentObjectModelFactory domFactory, TemplateInfo template, out IJsonObject serializedTemplate)
        {
            try
            {
                serializedTemplate = domFactory.CreateObject();
                serializedTemplate.SetValue(nameof(TemplateInfo.ConfigMountPointId), template.ConfigMountPointId);
                serializedTemplate.SetValue(nameof(TemplateInfo.Author), template.Author);
                serializedTemplate.SetValue(nameof(TemplateInfo.Classifications), template.Classifications);
                serializedTemplate.SetValue(nameof(TemplateInfo.DefaultName), template.DefaultName);
                serializedTemplate.SetValue(nameof(TemplateInfo.Description), template.Description);
                serializedTemplate.SetValue(nameof(TemplateInfo.Identity), template.Identity);
                serializedTemplate.SetValue(nameof(TemplateInfo.GeneratorId), template.GeneratorId);
                serializedTemplate.SetValue(nameof(TemplateInfo.GroupIdentity), template.GroupIdentity);
                serializedTemplate.SetValue(nameof(TemplateInfo.Precedence), template.Precedence);
                serializedTemplate.SetValue(nameof(TemplateInfo.Name), template.Name);
                serializedTemplate.SetValue(nameof(TemplateInfo.ShortNameList), template.ShortNameList);

                if (JsonSerializerHelpers.TrySerializeDictionary(domFactory, template.Tags, JsonSerializerHelpers.StringKeyConverter, SerializeCacheTag, out IJsonObject tagListObject)
                        && tagListObject != null)
                {
                    serializedTemplate.SetValue(nameof(TemplateInfo.Tags), tagListObject);
                }
                else
                {
                    serializedTemplate = null;
                    return false;
                }

                if (JsonSerializerHelpers.TrySerializeDictionary(domFactory, template.CacheParameters, JsonSerializerHelpers.StringKeyConverter, SerializeCacheParameter, out IJsonObject cacheParamObject)
                        && cacheParamObject != null)
                {
                    serializedTemplate.SetValue(nameof(TemplateInfo.CacheParameters), cacheParamObject);
                }
                else
                {
                    serializedTemplate = null;
                    return false;
                }

                // Note: Parameters are not serialized. Everything needed from them is in Tags & CacheParameters

                serializedTemplate.SetValue(nameof(TemplateInfo.ConfigPlace), template.ConfigPlace);
                serializedTemplate.SetValue(nameof(TemplateInfo.LocaleConfigMountPointId), template.LocaleConfigMountPointId);
                serializedTemplate.SetValue(nameof(TemplateInfo.LocaleConfigPlace), template.LocaleConfigPlace);
                serializedTemplate.SetValue(nameof(TemplateInfo.HostConfigMountPointId), template.HostConfigMountPointId);
                serializedTemplate.SetValue(nameof(TemplateInfo.HostConfigPlace), template.HostConfigPlace);
                serializedTemplate.SetValue(nameof(TemplateInfo.ThirdPartyNotices), template.ThirdPartyNotices);

                if (JsonSerializerHelpers.TrySerializeDictionary(domFactory, template.BaselineInfo, JsonSerializerHelpers.StringKeyConverter, SerializeBaseline, out IJsonObject baselineInfoObject)
                        && baselineInfoObject != null)
                {
                    serializedTemplate.SetValue(nameof(TemplateInfo.BaselineInfo), baselineInfoObject);
                }
                else
                {
                    serializedTemplate = null;
                    return false;
                }

                serializedTemplate.SetValue(nameof(TemplateInfo.HasScriptRunningPostActions), template.HasScriptRunningPostActions);
                serializedTemplate.SetValue(nameof(TemplateInfo.ConfigTimestampUtc), template.ConfigTimestampUtc.GetValueOrDefault().ToString());

                return true;
            }
            catch
            {
                serializedTemplate = null;
                return false;
            }
        }

        // TODO: after IAllowDefaultIfOptionWithoutValue is rolled up into ICacheParameter, get rid of the extra check for it.
        private static Func<IJsonDocumentObjectModelFactory, ICacheTag, IJsonObject> SerializeCacheTag = (domFactory, tag) =>
        {
            IJsonObject tagObject = domFactory.CreateObject();
            tagObject.SetValue(nameof(ICacheTag.Description), tag.Description);

            if (JsonSerializerHelpers.TrySerializeStringDictionary(domFactory, tag.ChoicesAndDescriptions, out IJsonObject serializedChoicesAndDescriptions))
            {
                tagObject.SetValue(nameof(ICacheTag.ChoicesAndDescriptions), serializedChoicesAndDescriptions);
            }
            else
            {
                tagObject = null;
            }

            tagObject.SetValue(nameof(ICacheTag.DefaultValue), tag.DefaultValue);

            if (tag is IAllowDefaultIfOptionWithoutValue tagWithNoValueDefault
                    && !string.IsNullOrEmpty(tagWithNoValueDefault.DefaultIfOptionWithoutValue))
            {
                tagObject.SetValue(nameof(IAllowDefaultIfOptionWithoutValue.DefaultIfOptionWithoutValue), tagWithNoValueDefault.DefaultIfOptionWithoutValue);
            }

            return tagObject;
        };

        // TODO: after IAllowDefaultIfOptionWithoutValue is rolled up into ICacheParameter, get rid of the extra check for it.
        private static Func<IJsonDocumentObjectModelFactory, ICacheParameter, IJsonObject> SerializeCacheParameter = (domFactory, param) =>
        {
            IJsonObject paramObject = domFactory.CreateObject();
            paramObject.SetValue(nameof(ICacheParameter.DataType), param.DataType);
            paramObject.SetValue(nameof(ICacheParameter.DefaultValue), param.DefaultValue);
            paramObject.SetValue(nameof(ICacheParameter.Description), param.Description);

            if (param is IAllowDefaultIfOptionWithoutValue paramWithNoValueDefault
                && !string.IsNullOrEmpty(paramWithNoValueDefault.DefaultIfOptionWithoutValue))
            {
                paramObject.SetValue(nameof(IAllowDefaultIfOptionWithoutValue.DefaultIfOptionWithoutValue), paramWithNoValueDefault.DefaultIfOptionWithoutValue);
            }

            return paramObject;
        };

        private static Func<IJsonDocumentObjectModelFactory, IBaselineInfo, IJsonObject> SerializeBaseline = (domFactory, baseline) =>
        {
            IJsonObject baselineObject = domFactory.CreateObject();
            baselineObject.SetValue(nameof(IBaselineInfo.Description), baseline.Description);
            if (JsonSerializerHelpers.TrySerializeStringDictionary(domFactory, baseline.DefaultOverrides, out IJsonObject defaultsObject))
            {
                baselineObject.SetValue(nameof(IBaselineInfo.DefaultOverrides), defaultsObject);
            }
            else
            {
                baselineObject = null;
            }

            return baselineObject;
        };
    }
}
