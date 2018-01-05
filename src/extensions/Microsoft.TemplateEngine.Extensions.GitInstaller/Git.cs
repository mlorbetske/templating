using System.Diagnostics;
using System.Linq;
using Microsoft.TemplateEngine.Cli;

namespace Microsoft.TemplateEngine.Extensions.GitInstaller
{
    public class Git : GenericProcess<Git>
    {
        public static Git Clone(params string[] args)
        {
            return EscapeArgsAndConfigure("git", new[] { "clone" }.Concat(args));
        }
    }
}
