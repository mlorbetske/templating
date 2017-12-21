using System;
using System.Collections.Generic;
using Microsoft.TemplateEngine.Core.Contracts;

namespace Microsoft.TemplateEngine.Core.LanguageSpecifics
{
    public class VisualBasicKeyworder : IKeyworder
    {
        public Guid Id { get; } = new Guid("4B7F76BF-A512-4287-BC18-EB5E14C5CDE9");

        // Keyword list from: https://docs.microsoft.com/en-us/dotnet/visual-basic/language-reference/keywords/
        private static readonly ISet<string> ReservedWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "AddHandler", "AddressOf", "Alias", "And",
            "AndAlso", "As", "Boolean", "ByRef",
            "Byte", "ByVal", "Call", "Case",
            "Catch", "CBool", "CByte", "CChar",
            "CDate", "CDbl", "CDec", "Char",
            "CInt", "Class", "CLng",
            "CObj", "Const", "Continue", "CSByte",
            "CShort", "CSng", "CStr", "CType",
            "CUInt", "CULng", "CUShort", "Date",
            "Decimal", "Declare", "Default", "Delegate",
            "Dim", "DirectCast", "Do", "Double",
            "Each", "Else", "ElseIf", "End",
            "EndIf", "Enum", "Erase",
            "Error", "Event", "Exit", "False",
            "Finally", "For", "Friend",
            "Function", "Get", "GetType", "GetXMLNamespace",
            "Global", "GoSub", "GoTo", "Handles",
            "If", "Implements",
            "Imports", "In",
            "Inherits", "Integer", "Interface", "Is",
            "IsNot", "Let", "Lib", "Like",
            "Long", "Loop", "Me", "Mod",
            "Module", "MustInherit", "MustOverride",
            "MyBase", "MyClass", "Namespace", "Narrowing",
            "New", "Next",
            "Not", "Nothing", "NotInheritable", "NotOverridable",
            "Object", "Of", "On", "Operator",
            "Option", "Optional", "Or", "OrElse",
            "Out", "Overloads", "Overridable", "Overrides",
            "ParamArray", "Partial", "Private", "Property",
            "Protected", "Public", "RaiseEvent", "ReadOnly",
            "ReDim", "REM", "RemoveHandler", "Resume",
            "Return", "SByte", "Select", "Set",
            "Shadows", "Shared", "Short", "Single",
            "Static", "Step", "Stop", "String",
            "Structure", "Sub", "SyncLock",
            "Then", "Throw", "To", "True",
            "Try", "TryCast", "TypeOf", "UInteger",
            "ULong", "UShort", "Using", "Variant",
            "Wend", "When", "While", "Widening",
            "With", "WithEvents", "WriteOnly", "Xor"
            //More entries are listed but are not valid identifiers even with escaping
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
                    parts[i] = $"[{parts[i]}]";
                }
            }

            return string.Join(".", parts);
        }
    }
}
