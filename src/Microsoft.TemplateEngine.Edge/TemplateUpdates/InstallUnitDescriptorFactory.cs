using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.TemplateEngine.Abstractions;
using Microsoft.TemplateEngine.Abstractions.Mount;
using Microsoft.TemplateEngine.Abstractions.TemplateUpdates;
using Microsoft.TemplateEngine.Abstractions.Json;

namespace Microsoft.TemplateEngine.Edge.TemplateUpdates
{
    public static class InstallUnitDescriptorFactory
    {
        public static bool TryParse(IEngineEnvironmentSettings environmentSettings, IJsonObject descriptorObj, out IInstallUnitDescriptor parsedDescriptor)
        {
            if (descriptorObj == null)
            {
                parsedDescriptor = null;
                return false;
            }

            if (!descriptorObj.TryGetValue(nameof(IInstallUnitDescriptor.FactoryId), StringComparison.OrdinalIgnoreCase, out IJsonToken factoryIdToken)
                || (factoryIdToken == null)
                || (factoryIdToken.TokenType != JsonTokenType.String)
                || !Guid.TryParse(((IJsonValue)factoryIdToken).Value.ToString(), out Guid factoryId)
                || !environmentSettings.SettingsLoader.Components.TryGetComponent(factoryId, out IInstallUnitDescriptorFactory factory))
            {
                parsedDescriptor = null;
                return false;
            }

            Dictionary<string, string> details = new Dictionary<string, string>();
            foreach (KeyValuePair<string, IJsonToken> property in descriptorObj.PropertiesOf(nameof(IInstallUnitDescriptor.Details)))
            {
                if (property.Value.TokenType != JsonTokenType.String)
                {
                    parsedDescriptor = null;
                    return false;
                }

                details[property.Key] = ((IJsonValue)property.Value).Value.ToString();
            }

            if (factory.TryCreateFromDetails(details, out IInstallUnitDescriptor descriptor))
            {
                parsedDescriptor = descriptor;
                return true;
            }

            parsedDescriptor = null;
            return false;
        }

        // For creating descriptors.
        public static bool TryCreateFromMountPoint(IEngineEnvironmentSettings environmentSettings, IMountPoint mountPoint, out IReadOnlyList<IInstallUnitDescriptor> descriptorList)
        {
            foreach (IInstallUnitDescriptorFactory factory in environmentSettings.SettingsLoader.Components.OfType<IInstallUnitDescriptorFactory>().ToList())
            {
                if (factory.TryCreateFromMountPoint(mountPoint, out descriptorList))
                {
                    return true;
                }
            }

            descriptorList = null;
            return false;
        }
    }
}
