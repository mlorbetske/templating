namespace Microsoft.TemplateEngine.Abstractions.PhysicalFileSystem
{
    public interface IGlobalPaths
    {
        string BaseDir { get; }

        string BuiltInsFeed { get; }

        string DefaultInstallPackageList { get; }

        string DefaultInstallTemplateList { get; }
    }
}
