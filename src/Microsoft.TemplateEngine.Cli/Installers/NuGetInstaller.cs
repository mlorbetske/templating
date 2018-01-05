using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.TemplateEngine.Abstractions;
using Microsoft.TemplateEngine.Abstractions.PhysicalFileSystem;
using Microsoft.TemplateEngine.Edge.Settings;

namespace Microsoft.TemplateEngine.Cli.Installers
{
    public class NuGetInstaller : IInstallerExtension
    {
        public int Order { get; } = 10000;

        public Guid Id { get; } = new Guid("3B31A2C9-527B-4179-A011-B4AD1C73FAB5");

        public bool CanHandle(IEngineEnvironmentSettings environmentSettings, IPaths paths, string installationRequest)
        {
            if (installationRequest.IndexOfAny(new[] { '/', '\\', '_', '-' }) > -1)
            {
                return false;
            }

            if (installationRequest.IndexOf("::", StringComparison.Ordinal) < 0)
            {
                installationRequest += "::*";
            }

            return Package.TryParse(installationRequest, out Package _);
        }

        public IReadOnlyList<ScanResultEntry> Install(IEngineEnvironmentSettings environmentSettings, IPaths paths, IReadOnlyList<string> installationRequests, IReadOnlyList<string> sources)
        {
            return InstallRemotePackages(environmentSettings, paths, installationRequests, sources);
        }

        private IReadOnlyList<ScanResultEntry> InstallRemotePackages(IEngineEnvironmentSettings environmentSettings, IPaths paths, IReadOnlyList<string> installationReqeuests, IReadOnlyList<string> nuGetSources)
        {
            List<Package> packages = new List<Package>();

            foreach (string installationRequest in installationReqeuests)
            {
                string request = installationRequest;
                if (request.IndexOf("::", StringComparison.Ordinal) < 0)
                {
                    request += "::*";
                }

                if (Package.TryParse(request, out Package package))
                {
                    packages.Add(package);
                }
            }

            const string packageRef = @"    <PackageReference Include=""{0}"" Version=""{1}"" />";
            const string projectFile = @"<Project ToolsVersion=""15.0"" Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <TargetFrameworks>netcoreapp1.0</TargetFrameworks>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Remove=""Microsoft.NETCore.App"" />
{0}
  </ItemGroup>
</Project>";

            paths.CreateDirectory(paths.User.ScratchDir);
            string proj = Path.Combine(paths.User.ScratchDir, "restore.csproj");
            StringBuilder references = new StringBuilder();

            foreach (Package pkg in packages)
            {
                references.AppendLine(string.Format(packageRef, pkg.Name, pkg.Version));
            }

            string content = string.Format(projectFile, references.ToString());
            paths.WriteAllText(proj, content);

            paths.CreateDirectory(paths.User.Packages);
            string restored = Path.Combine(paths.User.ScratchDir, "Packages");

            int additionalSlots = nuGetSources?.Count * 2 ?? 0;

            string[] restoreArgs = new string[3 + additionalSlots];
            restoreArgs[0] = proj;
            restoreArgs[1] = "--packages";
            restoreArgs[2] = restored;

            if (nuGetSources != null)
            {
                for (int i = 0; i < nuGetSources.Count; ++i)
                {
                    restoreArgs[3 + 2 * i] = "--source";
                    restoreArgs[4 + 2 * i] = nuGetSources[i];
                }
            }

            Dotnet.Restore(restoreArgs).ForwardStdOut().ForwardStdErr().Execute();

            List<string> newLocalPackages = new List<string>();
            foreach (string packagePath in paths.EnumerateFiles(restored, "*.nupkg", SearchOption.AllDirectories))
            {
                string path = Path.Combine(paths.User.Packages, Path.GetFileName(packagePath));
                paths.Copy(packagePath, path);
                newLocalPackages.Add(path);
            }

            paths.DeleteDirectory(paths.User.ScratchDir);
            return LocalInstaller.InstallLocalPackages(environmentSettings, newLocalPackages);
        }
    }
}
