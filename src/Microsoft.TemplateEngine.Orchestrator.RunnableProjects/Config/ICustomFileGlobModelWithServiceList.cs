using System;
using System.Collections.Generic;
using Microsoft.TemplateEngine.Core.Contracts;

namespace Microsoft.TemplateEngine.Orchestrator.RunnableProjects.Config
{
    public interface ICustomFileGlobModelWithServiceList : ICustomFileGlobModel
    {
        IReadOnlyList<Guid> AvailableServices { get; }
    }
}
