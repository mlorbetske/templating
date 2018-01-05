using System.Collections.Generic;
using Microsoft.TemplateEngine.Abstractions;
using Microsoft.TemplateEngine.Abstractions.PhysicalFileSystem;
using Microsoft.TemplateEngine.Edge.Settings;

namespace Microsoft.TemplateEngine.Cli.Installers
{
    public interface IInstallerExtension : IIdentifiedComponent
    {
        int Order { get; }

        bool CanHandle(IEngineEnvironmentSettings environmentSettings, IPaths paths, string installationRequest);

        IReadOnlyList<ScanResultEntry> Install(IEngineEnvironmentSettings environmentSettings, IPaths paths, IReadOnlyList<string> installationRequests, IReadOnlyList<string> sources);
    }
}
