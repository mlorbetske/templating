using System;
using System.Collections.Generic;

namespace Microsoft.TemplateEngine.Core.Contracts
{
    public interface IGlobalRunConfigWithServiceList : IGlobalRunConfig
    {
        IReadOnlyList<Guid> AvailableServices { get; }
    }
}
