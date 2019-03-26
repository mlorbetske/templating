using System.Collections.Generic;
using dotnet_new3;
using Microsoft.TemplateEngine.Abstractions;
using Microsoft.TemplateEngine.Abstractions.Json;
using Microsoft.TemplateEngine.Core;
using Microsoft.TemplateEngine.Core.Contracts;
using Microsoft.TemplateEngine.Orchestrator.RunnableProjects.Macros;
using Microsoft.TemplateEngine.Orchestrator.RunnableProjects.Macros.Config;
using Microsoft.TemplateEngine.TestHelper;
using Xunit;
using static Microsoft.TemplateEngine.Orchestrator.RunnableProjects.RunnableProjectGenerator;

namespace Microsoft.TemplateEngine.Orchestrator.RunnableProjects.UnitTests.MacroTests
{
    public class RegexMacroTests : TestBase
    {
        [Fact(DisplayName = nameof(TestRegexDeferredConfig))]
        public void TestRegexDeferredConfig()
        {
            string variableName = "myRegex";
            string sourceVariable = "originalValue";
            Dictionary<string, IJsonToken> jsonParameters = new Dictionary<string, IJsonToken>();
            IJsonDocumentObjectModelFactory domFactory = new JsonDomFactory();
            jsonParameters.Add("source", domFactory.CreateValue(sourceVariable));

            string jsonSteps = @"[
                { 
                    'regex': 'A',
                    'replacement': 'Z'
                }
            ]";
            domFactory.TryParse(jsonSteps, out IJsonToken token);
            jsonParameters.Add("steps", (IJsonArray)token);

            GeneratedSymbolDeferredMacroConfig deferredConfig = new GeneratedSymbolDeferredMacroConfig("RegexMacro", "string", variableName, jsonParameters);

            IVariableCollection variables = new VariableCollection();
            IRunnableProjectConfig config = new SimpleConfigModel();
            IParameterSet parameters = new ParameterSet(config);
            ParameterSetter setter = MacroTestHelpers.TestParameterSetter(EngineEnvironmentSettings, parameters);

            string sourceValue = "ABCAABBCC";
            string expectedValue = "ZBCZZBBCC";
            Parameter sourceParam = new Parameter
            {
                IsVariable = true,
                Name = sourceVariable
            };

            variables[sourceVariable] = sourceValue;
            setter(sourceParam, sourceValue);

            RegexMacro macro = new RegexMacro();
            IMacroConfig realConfig = macro.CreateConfig(EngineEnvironmentSettings, deferredConfig);
            macro.EvaluateConfig(EngineEnvironmentSettings, variables, realConfig, parameters, setter);
            Assert.True(parameters.TryGetParameterDefinition(variableName, out ITemplateParameter newParam));
            string newValue = (string)parameters.ResolvedValues[newParam];
            Assert.Equal(newValue, expectedValue);
        }

        [Fact(DisplayName = nameof(TestRegexMacro))]
        public void TestRegexMacro()
        {
            string variableName = "myRegex";
            string sourceVariable = "originalValue";
            IList<KeyValuePair<string, string>> steps = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("2+", "3"),
                new KeyValuePair<string, string>("13", "Z")
            };
            RegexMacroConfig macroConfig = new RegexMacroConfig(variableName, null, sourceVariable, steps);

            IVariableCollection variables = new VariableCollection();
            IRunnableProjectConfig config = new SimpleConfigModel();
            IParameterSet parameters = new ParameterSet(config);
            ParameterSetter setter = MacroTestHelpers.TestParameterSetter(EngineEnvironmentSettings, parameters);

            string sourceValue = "QQQ121222112";
            string expectedValue = "QQQZZ1Z";

            Parameter sourceParam = new Parameter
            {
                IsVariable = true,
                Name = sourceVariable
            };

            variables[sourceVariable] = sourceValue;
            setter(sourceParam, sourceValue);

            RegexMacro macro = new RegexMacro();
            macro.EvaluateConfig(EngineEnvironmentSettings, variables, macroConfig, parameters, setter);

            Assert.True(parameters.TryGetParameterDefinition(variableName, out ITemplateParameter newParam));
            string newValue = (string)parameters.ResolvedValues[newParam];
            Assert.Equal(newValue, expectedValue);
        }
    }
}
