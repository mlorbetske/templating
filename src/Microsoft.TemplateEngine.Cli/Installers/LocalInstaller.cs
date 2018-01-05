using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.TemplateEngine.Abstractions;
using Microsoft.TemplateEngine.Abstractions.PhysicalFileSystem;
using Microsoft.TemplateEngine.Edge.Settings;

namespace Microsoft.TemplateEngine.Cli.Installers
{
    public class LocalInstaller : IInstallerExtension
    {
        public int Order { get; } = 0;

        public Guid Id { get; } = new Guid("DA7B1DD8-7CCE-4509-AA0A-497072A5C5E2");

        public bool CanHandle(IEngineEnvironmentSettings environmentSettings, IPaths paths, string installationRequest)
        {
            string pkg = installationRequest.Trim();
            pkg = environmentSettings.Environment.ExpandEnvironmentVariables(pkg);
            string pattern = null;

            int wildcardIndex = pkg.IndexOfAny(new[] { '*', '?' });

            if (wildcardIndex > -1)
            {
                int lastSlashBeforeWildcard = pkg.LastIndexOfAny(new[] { '\\', '/' });
                if (lastSlashBeforeWildcard >= 0)
                {
                    pattern = pkg.Substring(lastSlashBeforeWildcard + 1);
                    pkg = pkg.Substring(0, lastSlashBeforeWildcard);
                }
            }

            if (pattern != null)
            {
                string fullDirectory = new DirectoryInfo(pkg).FullName;
                return paths.EnumerateFileSystemEntries(fullDirectory, pattern).Any();
            }

            return paths.Exists(pkg);
        }

        public IReadOnlyList<Guid> Install(IEngineEnvironmentSettings environmentSettings, IPaths paths, IReadOnlyList<string> installationRequests, IReadOnlyList<string> sources)
        {
            return InstallLocalPackages(environmentSettings, installationRequests);
        }

        public static IReadOnlyList<Guid> InstallLocalPackages(IEngineEnvironmentSettings environmentSettings, IReadOnlyList<string> packageNames)
        {
            List<Guid> resultingMountPointIds = new List<Guid>();
            List<string> toInstall = new List<string>();

            foreach (string package in packageNames)
            {
                if (package == null)
                {
                    continue;
                }

                string pkg = package.Trim();
                pkg = environmentSettings.Environment.ExpandEnvironmentVariables(pkg);
                string pattern = null;

                int wildcardIndex = pkg.IndexOfAny(new[] { '*', '?' });

                if (wildcardIndex > -1)
                {
                    int lastSlashBeforeWildcard = pkg.LastIndexOfAny(new[] { '\\', '/' });
                    if (lastSlashBeforeWildcard >= 0)
                    {
                        pattern = pkg.Substring(lastSlashBeforeWildcard + 1);
                        pkg = pkg.Substring(0, lastSlashBeforeWildcard);
                    }
                }

                try
                {
                    if (pattern != null)
                    {
                        string fullDirectory = new DirectoryInfo(pkg).FullName;
                        string fullPathGlob = Path.Combine(fullDirectory, pattern);
                        ((SettingsLoader)(environmentSettings.SettingsLoader)).UserTemplateCache.Scan(fullPathGlob, out IReadOnlyList<Guid> contentMountPointIds);
                        resultingMountPointIds.AddRange(contentMountPointIds);
                    }
                    else if (environmentSettings.Host.FileSystem.DirectoryExists(pkg) || environmentSettings.Host.FileSystem.FileExists(pkg))
                    {
                        string packageLocation = new DirectoryInfo(pkg).FullName;
                        ((SettingsLoader)(environmentSettings.SettingsLoader)).UserTemplateCache.Scan(packageLocation, out IReadOnlyList<Guid> contentMountPointIds);
                        resultingMountPointIds.AddRange(contentMountPointIds);
                    }
                    else
                    {
                        environmentSettings.Host.OnNonCriticalError("InvalidPackageSpecification", string.Format(LocalizableStrings.CouldNotFindItemToInstall, pkg), null, 0);
                    }
                }
                catch (Exception ex)
                {
                    environmentSettings.Host.OnNonCriticalError("InvalidPackageSpecification", string.Format(LocalizableStrings.BadPackageSpec, pkg), null, 0);

                    if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DOTNET_NEW_DEBUG")))
                    {
                        environmentSettings.Host.OnNonCriticalError("InvalidPackageSpecificationDetails", ex.ToString(), null, 0);
                    }
                    else
                    {
                        environmentSettings.Host.OnNonCriticalError("InvalidPackageSpecificationDetails", ex.Message, null, 0);
                    }
                }
            }

            return resultingMountPointIds;
        }
    }
}
