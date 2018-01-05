namespace Microsoft.TemplateEngine.Abstractions.PhysicalFileSystem
{
    public interface IUserPaths
    {
        string AliasesFile { get; }

        string BaseDir { get; }

        string Content { get; }

        string Packages { get; }

        string FirstRunCookie { get; }

        string NuGetConfig { get; }

        string PackageCache { get; }

        string ScratchDir { get; }

        string SettingsFile { get; }

        string InstallUnitDescriptorsFile { get; }

        string CultureNeutralTemplateCacheFile { get; }

        string CurrentLocaleTemplateCacheFile { get; }

        string ExplicitLocaleTemplateCacheFile(string locale);
    }
}
