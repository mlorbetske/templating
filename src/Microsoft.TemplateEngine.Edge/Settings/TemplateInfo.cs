using System;
using System.Collections.Generic;
using Microsoft.TemplateEngine.Abstractions;
using Microsoft.TemplateEngine.Edge.Settings.TemplateInfoReaders;
using Microsoft.TemplateEngine.Utils;
using Microsoft.TemplateEngine.Abstractions.Json;
using Microsoft.TemplateEngine.Utils.Json;

namespace Microsoft.TemplateEngine.Edge.Settings
{
    public class TemplateInfo : ITemplateInfo, IShortNameList, ITemplateWithTimestamp
    {
        public TemplateInfo()
        {
            ShortNameList = new List<string>();
        }

        private static readonly Func<IJsonObject, TemplateInfo> _defaultReader;
        private static readonly IReadOnlyDictionary<string, Func<IJsonObject, TemplateInfo>> _infoVersionReaders;

        // Note: Be sure to keep the versioning consistent with SettingsStore
        static TemplateInfo()
        {
            Dictionary<string, Func<IJsonObject, TemplateInfo>> versionReaders = new Dictionary<string, Func<IJsonObject, TemplateInfo>>
            {
                { "1.0.0.0", TemplateInfoReaderVersion1_0_0_0.FromJson },
                { "1.0.0.1", TemplateInfoReaderVersion1_0_0_1.FromJson },
                { "1.0.0.2", TemplateInfoReaderVersion1_0_0_2.FromJson },
                { "1.0.0.3", TemplateInfoReaderVersion1_0_0_3.FromJson }
            };
            _infoVersionReaders = versionReaders;

            _defaultReader = TemplateInfoReaderInitialVersion.FromJson;
        }

        public static TemplateInfo FromJson(IJsonObject entry, string cacheVersion)
        {
            if (string.IsNullOrEmpty(cacheVersion) || !_infoVersionReaders.TryGetValue(cacheVersion, out Func<IJsonObject, TemplateInfo> infoReader))
            {
                infoReader = _defaultReader;
            }

            return infoReader(entry);
        }

        public IReadOnlyList<ITemplateParameter> Parameters
        {
            get
            {
                if (_parameters == null)
                {
                    List<ITemplateParameter> parameters = new List<ITemplateParameter>();

                    foreach (KeyValuePair<string, ICacheTag> tagInfo in Tags)
                    {
                        ITemplateParameter param = new TemplateParameter
                        {
                            Name = tagInfo.Key,
                            Documentation = tagInfo.Value.Description,
                            DefaultValue = tagInfo.Value.DefaultValue,
                            Choices = tagInfo.Value.ChoicesAndDescriptions,
                            DataType = "choice"
                        };

                        if (param is IAllowDefaultIfOptionWithoutValue paramWithNoValueDefault
                            && tagInfo.Value is IAllowDefaultIfOptionWithoutValue tagWithNoValueDefault)
                        {
                            paramWithNoValueDefault.DefaultIfOptionWithoutValue = tagWithNoValueDefault.DefaultIfOptionWithoutValue;
                            parameters.Add(paramWithNoValueDefault as TemplateParameter);
                        }
                        else
                        {
                            parameters.Add(param);
                        }
                    }

                    foreach (KeyValuePair<string, ICacheParameter> paramInfo in CacheParameters)
                    {
                        ITemplateParameter param = new TemplateParameter
                        {
                            Name = paramInfo.Key,
                            Documentation = paramInfo.Value.Description,
                            DataType = paramInfo.Value.DataType,
                            DefaultValue = paramInfo.Value.DefaultValue,
                        };

                        if (param is IAllowDefaultIfOptionWithoutValue paramWithNoValueDefault
                            && paramInfo.Value is IAllowDefaultIfOptionWithoutValue infoWithNoValueDefault)
                        {
                            paramWithNoValueDefault.DefaultIfOptionWithoutValue = infoWithNoValueDefault.DefaultIfOptionWithoutValue;
                            parameters.Add(paramWithNoValueDefault as TemplateParameter);
                        }
                        else
                        {
                            parameters.Add(param);
                        }
                    }

                    _parameters = parameters;
                }

                return _parameters;
            }
        }
        private IReadOnlyList<ITemplateParameter> _parameters;


        public Guid ConfigMountPointId { get; set; }

        public string Author { get; set; }

        public List<string> Classifications { get; set; }

        public string DefaultName { get; set; }

        public string Description { get; set; }

        public string Identity { get; set; }

        public Guid GeneratorId { get; set; }

        public string GroupIdentity { get; set; }

        public int Precedence { get; set; }

        public string Name { get; set; }

        public string ShortName
        {
            get => ShortNameList.Count > 0 ? ShortNameList[0] : string.Empty;
            set
            {
                if (ShortNameList.Count > 0)
                {
                    throw new Exception("Can't set the short name when the ShortNameList already has entries.");
                }

                ShortNameList = new List<string>() { value };
            }
        }

        public IReadOnlyList<string> ShortNameList { get; set; }

        public IReadOnlyDictionary<string, ICacheTag> Tags
        {
            get => _tags;
            set
            {
                _tags = value;
                _parameters = null;
            }
        }
        private IReadOnlyDictionary<string, ICacheTag> _tags;

        public IReadOnlyDictionary<string, ICacheParameter> CacheParameters
        {
            get => _cacheParameters;
            set
            {
                _cacheParameters = value;
                _parameters = null;
            }
        }
        private IReadOnlyDictionary<string, ICacheParameter> _cacheParameters;

        public string ConfigPlace { get; set; }

        public Guid LocaleConfigMountPointId { get; set; }

        public string LocaleConfigPlace { get; set; }

        public Guid HostConfigMountPointId { get; set; }

        public string HostConfigPlace { get; set; }

        public string ThirdPartyNotices { get; set; }

        public IReadOnlyDictionary<string, IBaselineInfo> BaselineInfo { get; set; }

        public bool HasScriptRunningPostActions { get; set; }

        public DateTime? ConfigTimestampUtc { get; set; }

        IReadOnlyList<string> ITemplateInfo.Classifications => Classifications;

        IReadOnlyDictionary<string, ICacheTag> ITemplateInfo.Tags => Tags;

        IReadOnlyDictionary<string, ICacheParameter> ITemplateInfo.CacheParameters => CacheParameters;

        IReadOnlyDictionary<string, IBaselineInfo> ITemplateInfo.BaselineInfo => BaselineInfo;

        public IJsonBuilder<ITemplateInfo> JsonBuilder { get; } = new JsonBuilder<ITemplateInfo, TemplateInfo>(() => new TemplateInfo())
            .Map(p => p.ConfigMountPointId)
            .Map(p => p.Author)
            .ListOfString().Map<IReadOnlyList<string>, List<string>>(prop => prop.Classifications)
            .Map(p => p.DefaultName)
            .Map(p => p.Description)
            .Map(p => p.Identity)
            .Map(p => p.GeneratorId)
            .Map(p => p.GroupIdentity)
            .Map(p => p.Precedence)
            .Map(p => p.Name)
            .Map(p => p.ShortName)
            .ListOfString().Map<IReadOnlyList<string>, List<string>>(p => p.ShortNameList)
            .Dictionary<ITemplateInfo, TemplateInfo, ICacheTag, CacheTag>().Map<IReadOnlyDictionary<string, ICacheTag>, Dictionary<string, ICacheTag>>(p => p.Tags)
            .Dictionary<ITemplateInfo, TemplateInfo, ICacheParameter, CacheParameter>().Map<IReadOnlyDictionary<string, ICacheParameter>, Dictionary<string, ICacheParameter>>(p => p.CacheParameters)
            .Map(p => p.ConfigPlace)
            .Map(p => p.LocaleConfigMountPointId)
            .Map(p => p.LocaleConfigPlace)
            .Map(p => p.HostConfigMountPointId)
            .Map(p => p.HostConfigPlace)
            .Map(p => p.ThirdPartyNotices)
            .Dictionary<ITemplateInfo, TemplateInfo, IBaselineInfo, BaselineCacheInfo>().Map<IReadOnlyDictionary<string, IBaselineInfo>, Dictionary<string, IBaselineInfo>>(p => p.BaselineInfo)
            .Map(p => p.HasScriptRunningPostActions)
            .Map(p => p.ConfigTimestampUtc);
    }
}
