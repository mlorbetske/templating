using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.TemplateEngine.Abstractions;
using Microsoft.TemplateEngine.Abstractions.PhysicalFileSystem;
using Microsoft.TemplateEngine.Cli.Installers;
using Microsoft.TemplateEngine.Edge.Settings;

namespace Microsoft.TemplateEngine.Extensions.GitInstaller
{
    public class GitInstaller : IInstallerExtension
    {
        public int Order => 500;

        public Guid Id { get; } = new Guid("039C5A28-D542-4908-BC75-8437CCE7216D");

        public bool CanHandle(IEngineEnvironmentSettings environmentSettings, IPaths paths, string installationRequest)
        {
            return GitSource.TryParseGitSource(installationRequest, out GitSource gitSource);
        }

        public IReadOnlyList<ScanResultEntry> Install(IEngineEnvironmentSettings environmentSettings, IPaths paths, IReadOnlyList<string> installationRequests, IReadOnlyList<string> sources)
        {
            List<string> newLocalPackages = new List<string>();

            foreach (string request in installationRequests)
            {
                if (GitSource.TryParseGitSource(request, out GitSource source))
                {
                    string targetPath = Path.Combine(paths.User.BaseDir, "Git", source.RepositoryName);
                    Git.Result processResult = Git.Clone(source.GitUrl, targetPath).Execute();
                    bool cloneSuccessful = processResult.ExitCode == 0;
                    if (cloneSuccessful)
                    {
                        newLocalPackages.Add(Path.Combine(targetPath, source.SubFolder));
                    }
                }
            }

            IReadOnlyList<ScanResultEntry> result = LocalInstaller.InstallLocalPackages(environmentSettings, newLocalPackages);

            foreach (ScanResultEntry entry in result)
            {
                string path = entry.Location;
                SetAllFilesToNormal(environmentSettings.Host.FileSystem, path);
                environmentSettings.Host.FileSystem.WriteAllText(Path.Combine(path, ".leave"), "");
            }

            foreach (string path in newLocalPackages)
            {
                string target = Path.Combine(path, ".leave");
                if (paths.FileExists(target))
                {
                    paths.DeleteFile(target);
                }
                else
                {
                    paths.DeleteDirectory(path);
                }
            }

            return result;
        }

        public void SetAllFilesToNormal(IPhysicalFileSystem fileSystem, string path)
        {
            if (fileSystem.DirectoryExists(path))
            {
                foreach (string file in fileSystem.EnumerateFiles(path, "*", SearchOption.AllDirectories))
                {
                    fileSystem.SetFileAttributes(file, FileAttributes.Normal);
                }
            }
        }
    }
}
