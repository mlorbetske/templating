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
    public class SwtichMacroTests : TestBase
    {
        [Fact(DisplayName = nameof(TestSwitchConfig))]
        public void TestSwitchConfig()
        {
            string variableName = "mySwitchVar";
            string evaluator = "C++";
            string dataType = "string";
            string expectedValue = "this one";
            IList<KeyValuePair<string, string>> switches = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("(3 > 10)", "three greater than ten - false"),
                new KeyValuePair<string, string>("(false)", "false value"),
                new KeyValuePair<string, string>("(10 > 0)", expectedValue),
                new KeyValuePair<string, string>("(5 > 4)", "not this one")
            };
            SwitchMacroConfig macroConfig = new SwitchMacroConfig(variableName, evaluator, dataType, switches);

            IVariableCollection variables = new VariableCollection();
            IRunnableProjectConfig config = new SimpleConfigModel();
            IParameterSet parameters = new ParameterSet(config);
            ParameterSetter setter = MacroTestHelpers.TestParameterSetter(EngineEnvironmentSettings, parameters);

            SwitchMacro macro = new SwitchMacro();
            macro.EvaluateConfig(EngineEnvironmentSettings, variables, macroConfig, parameters, setter);

            Assert.True(parameters.TryGetParameterDefinition(variableName, out ITemplateParameter resultParam));
            string resultValue = (string)parameters.ResolvedValues[resultParam];
            Assert.Equal(resultValue, expectedValue);
        }

        [Fact(DisplayName = nameof(TestSwitchDeferredConfig))]
        public void TestSwitchDeferredConfig()
        {
            string variableName = "mySwitchVar";
            string evaluator = "C++";
            string dataType = "string";
            string expectedValue = "this one";
            string switchCases = @"[
                {
                    'condition': '(3 > 10)',
                    'value': 'three greater than ten'
                },
                {
                    'condition': '(false)',
                    'value': 'false value'
                },
                {
                    'condition': '(10 > 0)',
                    'value': '" + expectedValue + @"'
                },
                {
                    'condition': '(5 > 4)',
                    'value': 'not this one'
                }
            ]";

            IJsonDocumentObjectModelFactory domFactory = new JsonDomFactory();
            Dictionary<string, IJsonToken> jsonParameters = new Dictionary<string, IJsonToken>
            {
                { "evaluator", domFactory.CreateValue(evaluator) },
                { "datatype", domFactory.CreateValue(dataType) }
            };

            domFactory.TryParse(switchCases, out IJsonToken casesToken);
            jsonParameters.Add("cases", casesToken);

            GeneratedSymbolDeferredMacroConfig deferredConfig = new GeneratedSymbolDeferredMacroConfig("SwitchMacro", null, variableName, jsonParameters);

            IVariableCollection variables = new VariableCollection();
            IRunnableProjectConfig config = new SimpleConfigModel();
            IParameterSet parameters = new ParameterSet(config);
            ParameterSetter setter = MacroTestHelpers.TestParameterSetter(EngineEnvironmentSettings, parameters);

            SwitchMacro macro = new SwitchMacro();
            IMacroConfig realConfig = macro.CreateConfig(EngineEnvironmentSettings, deferredConfig);
            macro.EvaluateConfig(EngineEnvironmentSettings, variables, realConfig, parameters, setter);

            Assert.True(parameters.TryGetParameterDefinition(variableName, out ITemplateParameter resultParam));
            string resultValue = (string)parameters.ResolvedValues[resultParam];
            Assert.Equal(resultValue, expectedValue);
        }
    }
}
