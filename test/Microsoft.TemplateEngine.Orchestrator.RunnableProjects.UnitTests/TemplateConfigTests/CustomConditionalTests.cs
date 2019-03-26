using System.Collections.Generic;
using dotnet_new3;
using Microsoft.TemplateEngine.Abstractions.Json;
using Microsoft.TemplateEngine.Core.Contracts;
using Microsoft.TemplateEngine.Core.Operations;
using Microsoft.TemplateEngine.Orchestrator.RunnableProjects.Config;
using Microsoft.TemplateEngine.TestHelper;
using Xunit;

namespace Microsoft.TemplateEngine.Orchestrator.RunnableProjects.UnitTests.TemplateConfigTests
{
    public class CustomConditionalTests : TestBase
    {
        // defines a template configuration with a custom conditional configuration.
        // The style is omitted, which implies custon
        private static IJsonObject CustomConditionalSetupNoStyleSpecification
        {
            get
            {
                string configString = @"
{
             ""actionableIf"": [ ""<!--#if"" ],
             ""actionableElse"": [ ""#else"", ""<!--#else"" ],
             ""actionableElseif"": [ ""#elseif"", ""<!--#elseif"" ],
             ""endif"": [ ""#endif"", ""<!--#endif"" ],
             ""trim"" : ""true"",
             ""wholeLine"": ""true"",
}
";
                new JsonDomFactory().TryParse(configString, out IJsonToken root);
                return (IJsonObject)root;
            }
        }


        [Fact(DisplayName = nameof(TestCustomConditionalSetupNoStyleSpecification))]
        public void TestCustomConditionalSetupNoStyleSpecification()
        {
            IEnumerable<IOperationProvider> ops = new ConditionalConfig().ConfigureFromJson(CustomConditionalSetupNoStyleSpecification, null);
            IList<IOperationProvider> operations = new List<IOperationProvider>(ops);

            Assert.Equal(1, operations.Count);
            Assert.True(operations[0] is Conditional);

            Conditional conditionalOp = operations[0] as Conditional;
            Assert.Equal(1, conditionalOp.Tokens.ActionableIfTokens.Count);
            Assert.Equal("<!--#if", conditionalOp.Tokens.ActionableIfTokens[0].Value);

            Assert.Equal(2, conditionalOp.Tokens.ActionableElseTokens.Count);
            Assert.Contains(conditionalOp.Tokens.ActionableElseTokens, x => x.Value == "<!--#else");
            Assert.Contains(conditionalOp.Tokens.ActionableElseTokens, x => x.Value == "#else");

            Assert.Equal(2, conditionalOp.Tokens.ActionableElseIfTokens.Count);
            Assert.Contains(conditionalOp.Tokens.ActionableElseIfTokens, x => x.Value == "<!--#elseif");
            Assert.Contains(conditionalOp.Tokens.ActionableElseIfTokens, x => x.Value == "#elseif");

            Assert.Equal(2, conditionalOp.Tokens.EndIfTokens.Count);
            Assert.Contains(conditionalOp.Tokens.EndIfTokens, x => x.Value == "<!--#endif");
            Assert.Contains(conditionalOp.Tokens.EndIfTokens, x => x.Value == "#endif");

            Assert.True(conditionalOp.WholeLine);
            Assert.True(conditionalOp.TrimWhitespace);
        }

        private static IJsonObject CustomConditionalSetupExplicitStyleSpecification
        {
            get
            {
                string configString = @"
{
             ""style"": ""custom"",
             ""actionableIf"": [ ""<!--#if"" ],
             ""actionableElse"": [ ""#else"", ""<!--#else"" ],
             ""actionableElseif"": [ ""#elseif"", ""<!--#elseif"" ],
             ""endif"": [ ""#endif"", ""<!--#endif"" ],
             ""trim"" : ""true"",
             ""wholeLine"": ""true"",
}";

                new JsonDomFactory().TryParse(configString, out IJsonToken root);
                return (IJsonObject)root;
            }
        }

        [Fact(DisplayName = nameof(TestCustomConditionalSetupExplicitStyleSpecification))]
        public void TestCustomConditionalSetupExplicitStyleSpecification()
        {
            IEnumerable<IOperationProvider> ops = new ConditionalConfig().ConfigureFromJson(CustomConditionalSetupExplicitStyleSpecification, null);
            IList<IOperationProvider> operations = new List<IOperationProvider>(ops);

            Assert.Equal(1, operations.Count);
            Assert.True(operations[0] is Conditional);

            Conditional conditionalOp = operations[0] as Conditional;
            Assert.Equal(1, conditionalOp.Tokens.ActionableIfTokens.Count);
            Assert.Equal("<!--#if", conditionalOp.Tokens.ActionableIfTokens[0].Value);

            Assert.Equal(2, conditionalOp.Tokens.ActionableElseTokens.Count);
            Assert.Contains(conditionalOp.Tokens.ActionableElseTokens, x => x.Value == "<!--#else");
            Assert.Contains(conditionalOp.Tokens.ActionableElseTokens, x => x.Value == "#else");

            Assert.Equal(2, conditionalOp.Tokens.ActionableElseIfTokens.Count);
            Assert.Contains(conditionalOp.Tokens.ActionableElseIfTokens, x => x.Value == "<!--#elseif");
            Assert.Contains(conditionalOp.Tokens.ActionableElseIfTokens, x => x.Value == "#elseif");

            Assert.Equal(2, conditionalOp.Tokens.EndIfTokens.Count);
            Assert.Contains(conditionalOp.Tokens.EndIfTokens, x => x.Value == "<!--#endif");
            Assert.Contains(conditionalOp.Tokens.EndIfTokens, x => x.Value == "#endif");

            Assert.True(conditionalOp.WholeLine);
            Assert.True(conditionalOp.TrimWhitespace);
        }

        private static IJsonObject LineConditionalSetup
        {
            get
            {
                string configString = @"
{
        ""style"": ""line"",
        ""token"": ""//""
}
";

                new JsonDomFactory().TryParse(configString, out IJsonToken root);
                return (IJsonObject)root;
            }
        }

        [Fact(DisplayName = nameof(TestLineCommentConditionalSetup))]
        public void TestLineCommentConditionalSetup()
        {
            IEnumerable<IOperationProvider> ops = new ConditionalConfig().ConfigureFromJson(LineConditionalSetup, null);
            IList<IOperationProvider> operations = new List<IOperationProvider>(ops);

            Assert.Equal(3, operations.Count);
            Assert.True(operations[0] is Conditional);

            Conditional conditionalOp = operations[0] as Conditional;

            Assert.Equal(1, conditionalOp.Tokens.IfTokens.Count);
            Assert.Equal("//#if", conditionalOp.Tokens.IfTokens[0].Value);

            Assert.Equal(2, conditionalOp.Tokens.ElseIfTokens.Count);
            Assert.Contains(conditionalOp.Tokens.ElseIfTokens, x => x.Value == "//#elseif");
            Assert.Contains(conditionalOp.Tokens.ElseIfTokens, x => x.Value == "//#elif");

            Assert.Equal(1, conditionalOp.Tokens.ElseTokens.Count);
            Assert.Equal("//#else", conditionalOp.Tokens.ElseTokens[0].Value);

            Assert.Equal(2, conditionalOp.Tokens.EndIfTokens.Count);
            Assert.Contains(conditionalOp.Tokens.EndIfTokens, x => x.Value == "//#endif");
            Assert.Contains(conditionalOp.Tokens.EndIfTokens, x => x.Value == "////#endif");

            Assert.Equal(1, conditionalOp.Tokens.ActionableIfTokens.Count);
            Assert.Equal("////#if", conditionalOp.Tokens.ActionableIfTokens[0].Value);

            Assert.Equal(2, conditionalOp.Tokens.ActionableElseIfTokens.Count);
            Assert.Contains(conditionalOp.Tokens.ActionableElseIfTokens, x => x.Value == "////#elseif");
            Assert.Contains(conditionalOp.Tokens.ActionableElseIfTokens, x => x.Value == "////#elif");

            Assert.Equal(1, conditionalOp.Tokens.ActionableElseTokens.Count);
            Assert.Equal("////#else", conditionalOp.Tokens.ActionableElseTokens[0].Value);

            Assert.True(conditionalOp.WholeLine);
            Assert.True(conditionalOp.TrimWhitespace);
        }

        private static IJsonObject BlockConditionalSetup
        {
            get
            {
                string configString = @"
{
        ""style"": ""block"",
        ""startToken"": ""/*"",
        ""endToken"": ""*/"",
        ""pseudoEndToken"": ""* /""
}";
                new JsonDomFactory().TryParse(configString, out IJsonToken root);
                return (IJsonObject)root;
            }
        }

        [Fact(DisplayName = nameof(TestBlockCommentConditionalSetup))]
        public void TestBlockCommentConditionalSetup()
        {
            IEnumerable<IOperationProvider> ops = new ConditionalConfig().ConfigureFromJson(BlockConditionalSetup, null);
            IList<IOperationProvider> operations = new List<IOperationProvider>(ops);

            Assert.Equal(2, operations.Count);  // conditional & pseudo comment balancer
            Assert.True(operations[0] is Conditional);

            Conditional conditionalOp = operations[0] as Conditional;

            Assert.Equal(2, conditionalOp.Tokens.EndIfTokens.Count);
            Assert.Contains(conditionalOp.Tokens.EndIfTokens, x => x.Value == "#endif");
            Assert.Contains(conditionalOp.Tokens.EndIfTokens, x => x.Value == "/*#endif");

            Assert.Equal(1, conditionalOp.Tokens.ActionableIfTokens.Count);
            Assert.Equal("/*#if", conditionalOp.Tokens.ActionableIfTokens[0].Value);

            Assert.Equal(4, conditionalOp.Tokens.ActionableElseIfTokens.Count);
            Assert.Contains(conditionalOp.Tokens.ActionableElseIfTokens, x => x.Value == "#elseif");
            Assert.Contains(conditionalOp.Tokens.ActionableElseIfTokens, x => x.Value == "/*#elseif");
            Assert.Contains(conditionalOp.Tokens.ActionableElseIfTokens, x => x.Value == "#elif");
            Assert.Contains(conditionalOp.Tokens.ActionableElseIfTokens, x => x.Value == "/*#elif");

            Assert.Equal(2, conditionalOp.Tokens.ActionableElseTokens.Count);
            Assert.Contains(conditionalOp.Tokens.ActionableElseTokens, x => x.Value == "#else");
            Assert.Contains(conditionalOp.Tokens.ActionableElseTokens, x => x.Value == "/*#else");

            Assert.True(conditionalOp.WholeLine);
            Assert.True(conditionalOp.TrimWhitespace);
        }
    }
}
