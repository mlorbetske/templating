using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TemplateEngine.Cli
{
    public class Dotnet : GenericProcess<Dotnet>
    {
        public static Dotnet Restore(params string[] args)
        {
            return EscapeArgsAndConfigure("dotnet", new[] { "restore" }.Concat(args));
        }

        public static Dotnet AddProjectToProjectReference(string projectFile, params string[] args)
        {
            return EscapeArgsAndConfigure("dotnet", new[] { "add", projectFile, "reference" }.Concat(args));
        }

        public static Dotnet AddPackageReference(string projectFile, string packageName, string version = null)
        {
            string argString;
            if (version == null)
            {
                argString = ArgumentEscaper.EscapeAndConcatenateArgArrayForProcessStart(new[] { "add", projectFile, "package", packageName });
            }
            else
            {
                argString = ArgumentEscaper.EscapeAndConcatenateArgArrayForProcessStart(new[] { "add", projectFile, "package", packageName, "--version", version });
            }

            return Configure("dotnet", argString);
        }

        public static Dotnet AddProjectsToSolution(string solutionFile, IReadOnlyList<string> projects)
        {
            List<string> allArgs = new List<string>()
            {
                "sln",
                solutionFile,
                "add"
            };

            allArgs.AddRange(projects);
            string argString = ArgumentEscaper.EscapeAndConcatenateArgArrayForProcessStart(allArgs);

            return Configure("dotnet", argString);
        }

        public static Dotnet Version()
        {
            return Configure("dotnet", "--version");
        }
    }
}
