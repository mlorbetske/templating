using System.Collections.Generic;
using Microsoft.TemplateEngine.Abstractions;
using Microsoft.TemplateEngine.Abstractions.Json;
using Microsoft.TemplateEngine.Utils;

namespace Microsoft.TemplateEngine.Edge.Settings.TemplateInfoReaders
{
    public class TemplateInfoReaderVersion1_0_0_2 : TemplateInfoReaderVersion1_0_0_1
    {
        public static new TemplateInfo FromJson(IJsonObject json)
        {
            TemplateInfo info = new TemplateInfo();

            TemplateInfoReaderVersion1_0_0_2 reader = new TemplateInfoReaderVersion1_0_0_2();
            return reader.Read(json);
        }

        protected override void ReadShortNameInfo(IJsonObject json, TemplateInfo info)
        {
            if (info is IShortNameList)
            {
                IJsonToken shortNameToken = json.Get<IJsonToken>(nameof(TemplateInfo.ShortNameList));
                info.ShortNameList = JsonTokenStringOrArrayToCollection(shortNameToken, new string[0]);

                if (info.ShortNameList.Count == 0)
                {
                    // template.json stores the short name(s) in ShortName
                    // but the cache will store it in ShortNameList
                    base.ReadShortNameInfo(json, info);
                }
            }
            else
            {
                base.ReadShortNameInfo(json, info);
            }
        }

        private static IReadOnlyList<string> JsonTokenStringOrArrayToCollection(IJsonToken token, string[] defaultSet)
        {
            if (token == null)
            {
                return defaultSet;
            }

            if (token.TokenType == JsonTokenType.String)
            {
                string tokenValue = ((IJsonValue)token).Value.ToString();
                return new List<string>() { tokenValue };
            }

            return token.ArrayAsStrings();
        }

        protected override ICacheTag ReadOneTag(KeyValuePair<string, IJsonToken> item)
        {
            ICacheTag cacheTag = base.ReadOneTag(item);

            if (cacheTag is IAllowDefaultIfOptionWithoutValue tagWithNoValueDefault)
            {
                tagWithNoValueDefault.DefaultIfOptionWithoutValue = item.Value.ToString(nameof(IAllowDefaultIfOptionWithoutValue.DefaultIfOptionWithoutValue));
                return tagWithNoValueDefault as CacheTag;
            }

            return cacheTag;
        }

        protected override ICacheParameter ReadOneParameter(KeyValuePair<string, IJsonToken> item)
        {
            ICacheParameter param = base.ReadOneParameter(item);

            if (param is IAllowDefaultIfOptionWithoutValue paramWithNoValueDefault)
            {
                paramWithNoValueDefault.DefaultIfOptionWithoutValue = item.Value.ToString(nameof(IAllowDefaultIfOptionWithoutValue.DefaultIfOptionWithoutValue));
                return paramWithNoValueDefault as CacheParameter;
            }

            return param;
        }
    }
}
