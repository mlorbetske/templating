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
    public class CaseChangeMacroTests : TestBase
    {
        [Fact(DisplayName = nameof(TestCaseChangeToLowerConfig))]
        public void TestCaseChangeToLowerConfig()
        {
            string variableName = "myString";
            string sourceVariable = "sourceString";
            bool toLower = true;

            CaseChangeMacroConfig macroConfig = new CaseChangeMacroConfig(variableName, null, sourceVariable, toLower);

            IVariableCollection variables = new VariableCollection();
            IRunnableProjectConfig config = new SimpleConfigModel();
            IParameterSet parameters = new ParameterSet(config);
            ParameterSetter setter = MacroTestHelpers.TestParameterSetter(EngineEnvironmentSettings, parameters);

            string sourceValue = "Original Value SomethingCamelCase";
            Parameter sourceParam = new Parameter
            {
                IsVariable = true,
                Name = sourceVariable
            };

            variables[sourceVariable] = sourceValue;
            setter(sourceParam, sourceValue);

            CaseChangeMacro macro = new CaseChangeMacro();
            macro.EvaluateConfig(EngineEnvironmentSettings, variables, macroConfig, parameters, setter);

            Assert.True(parameters.TryGetParameterDefinition(variableName, out ITemplateParameter convertedParam));
            string convertedValue = (string)parameters.ResolvedValues[convertedParam];
            Assert.Equal(convertedValue, sourceValue.ToLower());
        }

        [Fact(DisplayName = nameof(TestCaseChangeToUpperConfig))]
        public void TestCaseChangeToUpperConfig()
        {
            string variableName = "myString";
            string sourceVariable = "sourceString";
            bool toLower = false;

            CaseChangeMacroConfig macroConfig = new CaseChangeMacroConfig(variableName, null, sourceVariable, toLower);

            IVariableCollection variables = new VariableCollection();
            IRunnableProjectConfig config = new SimpleConfigModel();
            IParameterSet parameters = new ParameterSet(config);
            ParameterSetter setter = MacroTestHelpers.TestParameterSetter(EngineEnvironmentSettings, parameters);

            string sourceValue = "Original Value SomethingCamelCase";
            Parameter sourceParam = new Parameter
            {
                IsVariable = true,
                Name = sourceVariable
            };

            variables[sourceVariable] = sourceValue;
            setter(sourceParam, sourceValue);

            CaseChangeMacro macro = new CaseChangeMacro();
            macro.EvaluateConfig(EngineEnvironmentSettings, variables, macroConfig, parameters, setter);

            Assert.True(parameters.TryGetParameterDefinition(variableName, out ITemplateParameter convertedParam));
            string convertedValue = (string)parameters.ResolvedValues[convertedParam];
            Assert.Equal(convertedValue, sourceValue.ToUpper());
        }

        [Fact(DisplayName = nameof(TestDeferredCaseChangeConfig))]
        public void TestDeferredCaseChangeConfig()
        {
            string variableName = "myString";

            IJsonDocumentObjectModelFactory domFactory = new JsonDomFactory();
            Dictionary<string, IJsonToken> jsonParameters = new Dictionary<string, IJsonToken>();
            string sourceVariable = "sourceString";
            jsonParameters.Add("source", domFactory.CreateValue(sourceVariable));
            jsonParameters.Add("toLower", domFactory.CreateValue(false));
            GeneratedSymbolDeferredMacroConfig deferredConfig = new GeneratedSymbolDeferredMacroConfig("CaseChangeMacro", null, variableName, jsonParameters);

            CaseChangeMacro macro = new CaseChangeMacro();
            IVariableCollection variables = new VariableCollection();
            IRunnableProjectConfig config = new SimpleConfigModel();
            IParameterSet parameters = new ParameterSet(config);
            ParameterSetter setter = MacroTestHelpers.TestParameterSetter(EngineEnvironmentSettings, parameters);

            string sourceValue = "Original Value SomethingCamelCase";
            Parameter sourceParam = new Parameter
            {
                IsVariable = true,
                Name = sourceVariable
            };

            variables[sourceVariable] = sourceValue;
            setter(sourceParam, sourceValue);

            IMacroConfig realConfig = macro.CreateConfig(EngineEnvironmentSettings, deferredConfig);
            macro.EvaluateConfig(EngineEnvironmentSettings, variables, realConfig, parameters, setter);

            Assert.True(parameters.TryGetParameterDefinition(variableName, out ITemplateParameter convertedParam));
            string convertedValue = (string)parameters.ResolvedValues[convertedParam];
            Assert.Equal(convertedValue, sourceValue.ToUpper());
        }
    }
}
