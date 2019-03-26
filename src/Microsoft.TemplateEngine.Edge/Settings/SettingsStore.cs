using System;
using System.Collections.Generic;
using Microsoft.TemplateEngine.Abstractions.Json;
using Microsoft.TemplateEngine.Abstractions.Mount;

namespace Microsoft.TemplateEngine.Edge.Settings
{
    public class SettingsStore
    {
        // NOTE: when the current version changes, a corresponding change in TemplateInfo.cs is needed, to get the correct template cache version reader to fire.
        internal static readonly string CurrentVersion = "1.0.0.3";

        public SettingsStore()
        {
            MountPoints = new List<MountPointInfo>();
            ComponentGuidToAssemblyQualifiedName = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            ComponentTypeToGuidList = new Dictionary<string, HashSet<Guid>>();
            ProbingPaths = new HashSet<string>();
            Version = string.Empty;
        }

        public SettingsStore(IJsonObject obj)
            : this()
        {
            if (obj.TryGetValue(nameof(Version), StringComparison.OrdinalIgnoreCase, out IJsonToken versionToken))
            {
                Version = versionToken.GetJsonString();
            }

            if (obj.TryGetValue("MountPoints", StringComparison.OrdinalIgnoreCase, out IJsonToken mountPointsToken))
            {
                IJsonArray mountPointsArray = mountPointsToken as IJsonArray;
                if (mountPointsArray != null)
                {
                    foreach (IJsonToken entry in mountPointsArray)
                    {
                        if (entry != null && entry.TokenType == JsonTokenType.Object)
                        {
                            Guid parentMountPointId;
                            Guid mountPointFactoryId;
                            Guid mountPointId;

                            IJsonObject mp = (IJsonObject) entry;
                            IJsonToken parentMountPointIdToken;
                            if (!mp.TryGetValue("ParentMountPointId", StringComparison.OrdinalIgnoreCase, out parentMountPointIdToken) || parentMountPointIdToken == null || parentMountPointIdToken.TokenType != JsonTokenType.String || !Guid.TryParse(((IJsonValue)parentMountPointIdToken).Value.ToString(), out parentMountPointId))
                            {
                                continue;
                            }

                            IJsonToken mountPointFactoryIdToken;
                            if (!mp.TryGetValue("MountPointFactoryId", StringComparison.OrdinalIgnoreCase, out mountPointFactoryIdToken) || mountPointFactoryIdToken == null || mountPointFactoryIdToken.TokenType != JsonTokenType.String || !Guid.TryParse(((IJsonValue)mountPointFactoryIdToken).Value.ToString(), out mountPointFactoryId))
                            {
                                continue;
                            }

                            IJsonToken mountPointIdToken;
                            if (!mp.TryGetValue("MountPointId", StringComparison.OrdinalIgnoreCase, out mountPointIdToken) || mountPointIdToken == null || mountPointIdToken.TokenType != JsonTokenType.String || !Guid.TryParse(((IJsonValue)mountPointIdToken).Value.ToString(), out mountPointId))
                            {
                                continue;
                            }

                            IJsonToken placeToken;
                            if (!mp.TryGetValue("Place", StringComparison.OrdinalIgnoreCase, out placeToken) || placeToken == null || placeToken.TokenType != JsonTokenType.String)
                            {
                                continue;
                            }

                            string place = ((IJsonValue)placeToken).Value.ToString();
                            MountPointInfo mountPoint = new MountPointInfo(parentMountPointId, mountPointFactoryId, mountPointId, place);
                            MountPoints.Add(mountPoint);
                        }
                    }
                }
            }

            if (obj.TryGetValue("ComponentGuidToAssemblyQualifiedName", StringComparison.OrdinalIgnoreCase, out IJsonToken componentGuidToAssemblyQualifiedNameToken))
            {
                IJsonObject componentGuidToAssemblyQualifiedNameObject = componentGuidToAssemblyQualifiedNameToken as IJsonObject;
                if (componentGuidToAssemblyQualifiedNameObject != null)
                {
                    foreach (KeyValuePair<string, IJsonToken> entry in componentGuidToAssemblyQualifiedNameObject.Properties())
                    {
                        if (entry.Value != null && entry.Value.TokenType == JsonTokenType.String)
                        {
                            ComponentGuidToAssemblyQualifiedName[entry.Key] = ((IJsonValue)entry.Value).Value.ToString();
                        }
                    }
                }
            }

            if (obj.TryGetValue("ProbingPaths", StringComparison.OrdinalIgnoreCase, out IJsonToken probingPathsToken))
            {
                IJsonArray probingPathsArray = probingPathsToken as IJsonArray;
                if (probingPathsArray != null)
                {
                    foreach (IJsonToken path in probingPathsArray)
                    {
                        if (path != null && path.TokenType == JsonTokenType.String)
                        {
                            ProbingPaths.Add(((IJsonValue)path).Value.ToString());
                        }
                    }
                }
            }

            if (obj.TryGetValue("ComponentTypeToGuidList", StringComparison.OrdinalIgnoreCase, out IJsonToken componentTypeToGuidListToken))
            {
                IJsonObject componentTypeToGuidListObject = componentTypeToGuidListToken as IJsonObject;
                if (componentTypeToGuidListObject != null)
                {
                    foreach (KeyValuePair<string, IJsonToken> entry in componentTypeToGuidListObject.Properties())
                    {
                        IJsonArray values = entry.Value as IJsonArray;

                        if (values != null)
                        {
                            HashSet<Guid> set = new HashSet<Guid>();
                            ComponentTypeToGuidList[entry.Key] = set;

                            foreach (IJsonToken value in values)
                            {
                                if (value != null && value.TokenType == JsonTokenType.String)
                                {
                                    Guid id;
                                    if (Guid.TryParse(((IJsonValue)value).Value.ToString(), out id))
                                    {
                                        set.Add(id);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public void SetVersionToCurrent()
        {
            Version = CurrentVersion;
        }

        [JsonProperty]
        public string Version { get; private set; }

        [JsonProperty]
        public List<MountPointInfo> MountPoints { get; }

        [JsonProperty]
        public Dictionary<string, string> ComponentGuidToAssemblyQualifiedName { get; }

        [JsonProperty]
        public HashSet<string> ProbingPaths { get; }

        [JsonProperty]
        public Dictionary<string, HashSet<Guid>> ComponentTypeToGuidList { get; }
    }
}
