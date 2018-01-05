using System;

namespace Microsoft.TemplateEngine.Edge.Settings
{
    [Flags]
    public enum ScanResultStatus : int
    {
        None = 0x00000000,
        TemplateOrLocalizationPack = 0x00000001,
        Component = 0x00000002,
    }
}
