using System;

namespace Microsoft.TemplateEngine.Edge.Settings
{
    public class ScanResultEntry
    {
        public string Location { get; set; }

        public Guid MountPointId { get; set; }

        public ScanResultStatus Status { get; set; }
    }
}
