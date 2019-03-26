using System;
using Microsoft.TemplateEngine.Abstractions.Json;
using Microsoft.TemplateEngine.Abstractions.TemplateUpdates;
using Microsoft.TemplateEngine.Utils.Json;

namespace Microsoft.TemplateEngine.Edge.Settings
{
    internal static class InstallDescriptorCacheJsonSerializer
    {
        public static bool TrySerialize(IJsonDocumentObjectModelFactory domFactory, InstallUnitDescriptorCache descriptorCache, out string serialized)
        {
            IJsonObject cacheObject = domFactory.CreateObject();

            if (JsonSerializerHelpers.TrySerializeDictionary(domFactory, descriptorCache.InstalledItems, JsonSerializerHelpers.GuidKeyConverter, JsonSerializerHelpers.StringValueConverter, out IJsonObject installedItemsObject))
            {
                cacheObject.SetValue(nameof(descriptorCache.InstalledItems), installedItemsObject);
            }
            else
            {
                serialized = null;
                return false;
            }

            if (JsonSerializerHelpers.TrySerializeDictionary(domFactory, descriptorCache.Descriptors, JsonSerializerHelpers.StringKeyConverter, InstallUnitDescriptorToJsonConverter, out IJsonObject descriptorObject))
            {
                cacheObject.SetValue(nameof(descriptorCache.Descriptors), descriptorObject);
            }
            else
            {
                serialized = null;
                return false;
            }

            serialized = cacheObject.GetJsonString();
            return true;
        }

        private static Func<IJsonDocumentObjectModelFactory, IInstallUnitDescriptor, IJsonObject> InstallUnitDescriptorToJsonConverter = (domFactory, descriptor) =>
        {
            IJsonObject descriptorObject = domFactory.CreateObject();

            descriptorObject.SetValue(nameof(IInstallUnitDescriptor.FactoryId), descriptor.FactoryId);

            if (JsonSerializerHelpers.TrySerializeStringDictionary(domFactory, descriptor.Details, out IJsonObject detailsObject))
            {
                descriptorObject.SetValue(nameof(IInstallUnitDescriptor.Details), detailsObject);
            }
            else
            {
                descriptorObject = null;
            }

            return descriptorObject;
        };
    }
}
