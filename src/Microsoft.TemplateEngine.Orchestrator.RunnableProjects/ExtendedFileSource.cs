using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.TemplateEngine.Abstractions.Mount;
using Microsoft.TemplateEngine.Utils.Json;

namespace Microsoft.TemplateEngine.Orchestrator.RunnableProjects
{
    public class ExtendedFileSource
    {
        internal static readonly DeserializationPlan<Func<IFile, ExtendedFileSource>> DeserializationPlan = Deserializer.CreateDeserializerBuilder()
            .IfObject()
                .Property("condition")
                    .IfString("condition")
                .Pop()
                .Property("source")
                    .IfString("source")
                .Pop()
                .Property("target")
                    .IfString("target")
                .Pop()
                .HandleIceDeserialize("include")
                .HandleIceDeserialize("copyOnly")
                .HandleIceDeserialize("exclude")
                .Property("rename")
                    .IfObject()
                        .StoreAsDictionary("rename")
                            .IfString("value")
                        .Pop()
                    .Pop()
                .Pop()
                .Property("modifiers")
                    .IfArray("modifiers")
                        .IfObject("value", SourceModifier.DeserializationPlan)
                    .Pop()
                .Pop()
            .Pop()
            .ToPlan(CreateExtendedFileSourceFromDeserializationContext);

        private static Func<IFile, ExtendedFileSource> CreateExtendedFileSourceFromDeserializationContext(DeserializationContext context)
        {
            string condition = context.GetValue<string>("condition");
            string source = context.GetValue<string>("source");
            string target = context.GetValue<string>("target");
            Func<IFile, IReadOnlyList<string>> includeFunc = context.GetIcePropertyAsFunc("include", IceDeserializerUtil.IncludePatternDefaults);
            Func<IFile, IReadOnlyList<string>> copyOnlyFunc = context.GetIcePropertyAsFunc("copyOnly", IceDeserializerUtil.CopyOnlyPatternDefaults);
            Func<IFile, IReadOnlyList<string>> excludeFunc = context.GetIcePropertyAsFunc("exclude", IceDeserializerUtil.ExcludePatternDefaults);
            IReadOnlyDictionary<string, string> rename = context.GetValue<IReadOnlyDictionary<string, DeserializationContext>>("rename").Select(x => (x.Key, Value: x.Value.GetValue<string>("value"))).ToDictionary(x => x.Key, x => x.Value);
            IReadOnlyList<SourceModifier> modifiers = context.GetValue<IReadOnlyList<DeserializationContext>>("modifiers").Select(x => x.GetValue<SourceModifier>("value")).ToList();

            return f =>
            {
                IReadOnlyList<string> copyOnly = copyOnlyFunc(f);
                IReadOnlyList<string> include = includeFunc(f);
                IReadOnlyList<string> exclude = excludeFunc(f);

                ExtendedFileSource result = new ExtendedFileSource
                {
                    Source = source,
                    Target = target,
                    Condition = condition,
                    Include = include,
                    CopyOnly = copyOnly,
                    Exclude = exclude,
                    Rename = rename
                };

                result._modifiers.AddRange(modifiers);

                return result;
            };
        }

        private readonly List<SourceModifier> _modifiers = new List<SourceModifier>();

        public IReadOnlyList<string> CopyOnly { get; private set; }

        public IReadOnlyList<string> Include { get; private set; }

        public IReadOnlyList<string> Exclude { get; private set; }

        public IReadOnlyDictionary<string, string> Rename { get; private set; }

        public string Source { get; private set; }

        public string Target { get; private set; }

        public string Condition { get; private set; }

        public IReadOnlyList<SourceModifier> Modifiers => _modifiers;
    }
}
