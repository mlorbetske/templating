using System;
using System.Collections.Generic;
using Microsoft.TemplateEngine.Core.Contracts;

namespace Microsoft.TemplateEngine.Core.LanguageSpecifics
{
    public class FSharpKeyworder : IKeyworder
    {
        public Guid Id { get; } = new Guid("DEE2BDBE-CE6A-4AC2-BB6F-702A497843E7");

        // Keyword list from: https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/keyword-reference
        private static readonly ISet<string> ReservedWords = new HashSet<string>(StringComparer.Ordinal)
        {
            "abstract", "and", "as", "assert",
            "base", "begin", "class", "default",
            "delegate", "do", "done", "downcast",
            "downto", "elif", "else", "end",
            "exception", "extern", "false", "finally",
            "fixed", "for", "fun", "function",
            "global", "if", "in", "inherit",
            "inline", "interface"," internal", "lazy",
            "let", "match", "member", "module",
            "mutable", "namespace", "new", "not",
            "null", "of", "open", "or",
            "override", "private", "public", "rec",
            "return", "select", "static", "struct",
            "then", "to", "true", "try",
            "type", "upcast", "use", "val",
            "void", "when", "while", "with",
            "yield",
            //OCAML reserved words
            "asr", "land", "lor", "lsl",
            "lsr", "lxor", "mod", "sig",
            //Future reserved words (as of 12/21/2017)
            "atomic", "break", "checked", "component",
            "const", "constraint", "constructor", "continue",
            "eager", "event", "external", "fixed",
            "functor", "include", "method", "mixin",
            "object", "parallel", "process", "protected",
            "pure", "sealed", "tailcall", "trait",
            "virtual", "volatile"
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
                    parts[i] = $"``{parts[i]}``";
                }
            }

            return string.Join(".", parts);
        }
    }
}
