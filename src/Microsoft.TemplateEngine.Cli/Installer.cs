// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.TemplateEngine.Abstractions;
using Microsoft.TemplateEngine.Abstractions.Mount;
using Microsoft.TemplateEngine.Cli.Installers;
using Microsoft.TemplateEngine.Edge;
using Microsoft.TemplateEngine.Edge.Settings;

namespace Microsoft.TemplateEngine.Cli
{
    internal class Installer : IInstaller
    {
        private readonly IEngineEnvironmentSettings _environmentSettings;
        private readonly Paths _paths;

        public Installer(IEngineEnvironmentSettings environmentSettings)
        {
            _environmentSettings = environmentSettings;
            _paths = new Paths(environmentSettings);
        }

        public void InstallPackages(IEnumerable<string> installationRequests) => InstallPackages(installationRequests, null, false);

        public void InstallPackages(IEnumerable<string> installationRequests, IList<string> nuGetSources) => InstallPackages(installationRequests, nuGetSources, false);

        //TODO: Try to take in nuGetSources as an IReadOnlyList<string> instead of IList
        public void InstallPackages(IEnumerable<string> installationRequests, IList<string> nuGetSources, bool debugAllowDevInstall)
        {
            IEnumerable<IInstallerExtension> installers = _environmentSettings.SettingsLoader.Components.OfType<IInstallerExtension>();
            List<ScanResultEntry> installationResults = new List<ScanResultEntry>();
            bool installed = false;

            foreach (string installationRequest in installationRequests)
            {
                foreach (IInstallerExtension installer in installers.OrderBy(x => x.Order))
                {
                    if (installer.CanHandle(_environmentSettings, _paths, installationRequest))
                    {
                        //TODO: Figure out a better way of handling installation requests,
                        //  right now they must be processed one at a time to avoid confusion
                        //  if not all of the requests succeed
                        IReadOnlyList<ScanResultEntry> mountPoints = installer.Install(_environmentSettings, _paths, new[] { installationRequest }, nuGetSources?.ToList());

                        if (mountPoints.Count > 0)
                        {
                            installationResults.AddRange(mountPoints);
                            installed = true;
                            break;
                        }
                    }
                }

                if (!installed)
                {
                    _environmentSettings.Host.OnNonCriticalError("InvalidPackageSpecification", string.Format(LocalizableStrings.CouldNotFindItemToInstall, installationRequest), null, 0);
                }
            }

            foreach (ScanResultEntry result in installationResults)
            {
                ((SettingsLoader)(_environmentSettings.SettingsLoader)).InstallUnitDescriptorCache.TryAddDescriptorForLocation(result);
            }

            _environmentSettings.SettingsLoader.Save();
        }

        public IEnumerable<string> Uninstall(IEnumerable<string> uninstallRequests)
        {
            List<string> uninstallFailures = new List<string>();
            foreach (string uninstall in uninstallRequests)
            {
                string prefix = Path.Combine(_paths.User.Packages, uninstall);
                IReadOnlyList<MountPointInfo> rootMountPoints = _environmentSettings.SettingsLoader.MountPoints.Where(x =>
                {
                    if (x.ParentMountPointId != Guid.Empty)
                    {
                        return false;
                    }

                    if (uninstall.IndexOfAny(new[] { '/', '\\' }) < 0)
                    {
                        if (x.Place.StartsWith(prefix + ".", StringComparison.OrdinalIgnoreCase) && x.Place.EndsWith(".nupkg", StringComparison.OrdinalIgnoreCase))
                        {
                            return true;
                        }
                    }

                    if (string.Equals(x.Place, uninstall, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                    else if (x.Place.Length > uninstall.Length)
                    {
                        string place = x.Place.Replace('\\', '/');
                        string match = uninstall.Replace('\\', '/');

                        if (match[match.Length - 1] != '/')
                        {
                            match += "/";
                        }

                        return place.StartsWith(match, StringComparison.OrdinalIgnoreCase);
                    }

                    return false;
                }).ToList();

                if (rootMountPoints.Count == 0)
                {
                    uninstallFailures.Add(uninstall);
                    continue;
                }

                HashSet<Guid> mountPoints = new HashSet<Guid>(rootMountPoints.Select(x => x.MountPointId));
                bool isSearchComplete = false;
                while (!isSearchComplete)
                {
                    isSearchComplete = true;
                    foreach (MountPointInfo possibleChild in _environmentSettings.SettingsLoader.MountPoints)
                    {
                        if (mountPoints.Contains(possibleChild.ParentMountPointId))
                        {
                            isSearchComplete &= !mountPoints.Add(possibleChild.MountPointId);
                        }
                    }
                }

                //Find all of the things that refer to any of the mount points we've got
                _environmentSettings.SettingsLoader.RemoveMountPoints(mountPoints);
                ((SettingsLoader)(_environmentSettings.SettingsLoader)).InstallUnitDescriptorCache.RemoveDescriptorsForLocationList(mountPoints);
                _environmentSettings.SettingsLoader.Save();

                foreach (MountPointInfo mountPoint in rootMountPoints)
                {
                    if (mountPoint.Place.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                    {
                        try
                        {
                            _environmentSettings.Host.FileSystem.FileDelete(mountPoint.Place);
                        }
                        catch
                        {
                        }
                    }
                }
            }

            return uninstallFailures;
        }
    }
}
