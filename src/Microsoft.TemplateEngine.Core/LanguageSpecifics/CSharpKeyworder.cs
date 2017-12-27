using System;
using System.Collections.Generic;
using Microsoft.TemplateEngine.Core.Contracts;

namespace Microsoft.TemplateEngine.Core.LanguageSpecifics
{
    public class CSharpKeyworder : IKeyworder
    {
        public Guid Id { get; } = Identifier;

        public static readonly Guid Identifier = new Guid("96CE6542-6D63-47B1-BC38-8EF52E81C6AF");

        //Keyword list from: https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/
        private static readonly ISet<string> ReservedWords = new HashSet<string>(StringComparer.Ordinal)
        {
            "abstract", "as", "base", "bool",
            "break", "byte", "case", "catch",
            "char", "checked", "class", "const",
            "continue", "decimal", "default", "delegate",
            "do", "double", "else", "enum",
            "event", "explicit", "extern", "false",
            "finally", "fixed", "float", "for",
            "foreach", "goto", "if", "implicit",
            "in", "int", "interface",
            "internal", "is", "lock", "long",
            "namespace", "new", "null", "object",
            "operator", "out", "override",
            "params", "private", "protected", "public",
            "readonly", "ref", "return", "sbyte",
            "sealed", "short", "sizeof", "stackalloc",
            "static", "string", "struct", "switch",
            "this", "throw", "true", "try",
            "typeof", "uint", "ulong", "unchecked",
            "unsafe", "ushort", "using", //"using static" is listed in the documentation but isn't valid as an identifier
            "virtual", "void", "volatile", "while"
        };

        public string EscapeKeywords(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            string[] parts = input.Split('.');

            for (int i = 0; i < parts.Length; ++i)
            {
                if (ReservedWords.Contains(parts[i]))
                {
                    parts[i] = $"@{parts[i]}";
                }
            }

            return string.Join(".", parts);
        }
    }
}
