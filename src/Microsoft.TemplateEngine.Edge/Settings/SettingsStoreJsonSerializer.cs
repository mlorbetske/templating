using System;
using System.Collections.Generic;
using Microsoft.TemplateEngine.Abstractions.Json;
using Microsoft.TemplateEngine.Abstractions.Mount;
using Microsoft.TemplateEngine.Utils.Json;

namespace Microsoft.TemplateEngine.Edge.Settings
{
    internal static class SettingsStoreJsonSerializer
    {
        public static bool TrySerialize(IJsonDocumentObjectModelFactory domFactory, SettingsStore settingsStore, out string serialized)
        {
            IJsonObject storeObject = domFactory.CreateObject();

            storeObject.SetValue(nameof(SettingsStore.Version), domFactory.CreateValue(settingsStore.Version));

            if (JsonSerializerHelpers.TrySerializeIEnumerable(domFactory, settingsStore.MountPoints, SerializeMountPoint, out IJsonArray mountPointListObject)
                && JsonSerializerHelpers.TrySerializeStringDictionary(domFactory, settingsStore.ComponentGuidToAssemblyQualifiedName, out IJsonObject componentGuidToAssemblyQualifiedNameObject)
                && JsonSerializerHelpers.TrySerializeIEnumerable(domFactory, settingsStore.ProbingPaths, JsonSerializerHelpers.StringValueConverter, out IJsonArray probingPathsObject)
                && TrySerializeComponentTypeToGuidList(domFactory, settingsStore.ComponentTypeToGuidList, out IJsonObject componentTypeToGuidListObject))
            {
                storeObject.SetValue(nameof(SettingsStore.MountPoints), mountPointListObject);
                storeObject.SetValue(nameof(SettingsStore.ComponentGuidToAssemblyQualifiedName), componentGuidToAssemblyQualifiedNameObject);
                storeObject.SetValue(nameof(SettingsStore.ProbingPaths), probingPathsObject);
                storeObject.SetValue(nameof(SettingsStore.ComponentTypeToGuidList), componentTypeToGuidListObject);

                serialized = storeObject.GetJsonString();
                return true;
            }

            serialized = null;
            return false;
        }

        private static Func<IJsonDocumentObjectModelFactory, MountPointInfo, IJsonObject> SerializeMountPoint = (domFactory, mountPointInfo) =>
        {
            IJsonObject mountPointObject = domFactory.CreateObject();

            mountPointObject.SetValue(nameof(MountPointInfo.ParentMountPointId), mountPointInfo.ParentMountPointId);
            mountPointObject.SetValue(nameof(MountPointInfo.MountPointFactoryId), mountPointInfo.MountPointFactoryId);
            mountPointObject.SetValue(nameof(MountPointInfo.MountPointId), mountPointInfo.MountPointId);
            mountPointObject.SetValue(nameof(MountPointInfo.Place), mountPointInfo.Place);

            return mountPointObject;
        };

        private static bool TrySerializeComponentTypeToGuidList(IJsonDocumentObjectModelFactory domFactory, IReadOnlyDictionary<string, HashSet<Guid>> componentTypeToGuidList, out IJsonObject componentTypeToGuidListObject)
        {
            try
            {
                componentTypeToGuidListObject = domFactory.CreateObject();

                foreach (KeyValuePair<string, HashSet<Guid>> componentTypeEntry in componentTypeToGuidList)
                {
                    if (JsonSerializerHelpers.TrySerializeIEnumerable(domFactory, componentTypeEntry.Value, JsonSerializerHelpers.GuidValueConverter, out IJsonArray componentTypeEntryObject))
                    {
                        componentTypeToGuidListObject.SetValue(componentTypeEntry.Key, componentTypeEntryObject);
                    }
                    else
                    {
                        componentTypeToGuidListObject = null;
                        return false;
                    }
                }

                return true;
            }
            catch
            {
                componentTypeToGuidListObject = null;
                return false;
            }
        }
    }
}
