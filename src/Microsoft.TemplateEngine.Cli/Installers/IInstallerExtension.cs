using System;
using System.Collections.Generic;
using Microsoft.TemplateEngine.Abstractions;
using Microsoft.TemplateEngine.Abstractions.PhysicalFileSystem;

namespace Microsoft.TemplateEngine.Cli.Installers
{
    public interface IInstallerExtension : IIdentifiedComponent
    {
        int Order { get; }

        bool CanHandle(IEngineEnvironmentSettings environmentSettings, IPaths paths, string installationRequest);

        IReadOnlyList<Guid> Install(IEngineEnvironmentSettings environmentSettings, IPaths paths, IReadOnlyList<string> installationRequests, IReadOnlyList<string> sources);
    }
}
