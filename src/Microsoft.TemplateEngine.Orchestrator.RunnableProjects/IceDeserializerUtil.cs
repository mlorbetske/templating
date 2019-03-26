using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.TemplateEngine.Abstractions.Mount;
using Microsoft.TemplateEngine.Utils;
using Microsoft.TemplateEngine.Utils.Json;

namespace Microsoft.TemplateEngine.Orchestrator.RunnableProjects
{
    internal static class IceDeserializerUtil
    {
        public static readonly IReadOnlyList<string> IncludePatternDefaults = new[] { "**/*" };
        public static readonly IReadOnlyList<string> ExcludePatternDefaults = new[]
        {
            "**/[Bb]in/**",
            "**/[Oo]bj/**",
            $"**/{RunnableProjectGenerator.TemplateConfigDirectoryName}/**",
            "**/*.filelist",
            "**/*.user",
            "**/*.lock.json"
        };
        public static readonly IReadOnlyList<string> CopyOnlyPatternDefaults = new[] { "**/node_modules/**" };

        public static ObjectBuilder<T> HandleIceDeserialize<T>(this ObjectBuilder<T> builder, string propertyName)
        {
            return builder
                .Property(propertyName)
                    .IfNull(propertyName)
                    .IfString((tokenValue, context) => context[propertyName] = (Func<IFile, IReadOnlyList<string>>)(sourceFile =>
                    {
                        if ((tokenValue.IndexOfAny(Path.GetInvalidPathChars()) != -1)
                           || !sourceFile.Parent.FileInfo(tokenValue).Exists)
                        {
                            return new List<string>(new[] { tokenValue });
                        }
                        else
                        {
                            using (Stream excludeList = sourceFile.Parent.FileInfo(tokenValue).OpenRead())
                            using (TextReader reader = new StreamReader(excludeList, Encoding.UTF8, true, 4096, true))
                            {
                                return reader.ReadToEnd().Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                            }
                        }
                    }))
                    .IfArray(propertyName).IfString("value").Pop()
                .Pop();
        }

        public static Func<IFile, IReadOnlyList<string>> GetIcePropertyAsFunc(this DeserializationContext context, string propertyName, IReadOnlyList<string> defaults)
        {
            Func<IFile, IReadOnlyList<string>> copyOnlyFunc = context.GetValue<Func<IFile, IReadOnlyList<string>>>(propertyName);

            if (copyOnlyFunc is null)
            {
                IReadOnlyList<DeserializationContext> contextList = context.GetValue<IReadOnlyList<DeserializationContext>>(propertyName);

                if (contextList is null)
                {
                    copyOnlyFunc = f => defaults;
                }
                else
                {
                    List<string> values = contextList.Select(x => x.GetValue<string>("value")).ToList();
                    copyOnlyFunc = f => values;
                }
            }

            return copyOnlyFunc;
        }
    }
}
