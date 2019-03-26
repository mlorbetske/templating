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
    public class ConstantMacroTests : TestBase
    {
        [Fact(DisplayName = nameof(TestConstantConfig))]
        public void TestConstantConfig()
        {
            string variableName = "myConstant";
            string value = "1048576";
            ConstantMacroConfig macroConfig = new ConstantMacroConfig(null, variableName, value);

            IVariableCollection variables = new VariableCollection();
            IRunnableProjectConfig config = new SimpleConfigModel();
            IParameterSet parameters = new ParameterSet(config);
            ParameterSetter setter = MacroTestHelpers.TestParameterSetter(EngineEnvironmentSettings, parameters);

            ConstantMacro macro = new ConstantMacro();
            macro.EvaluateConfig(EngineEnvironmentSettings, variables, macroConfig, parameters, setter);

            Assert.True(parameters.TryGetParameterDefinition(variableName, out ITemplateParameter constParameter));
            string constParamValue = (parameters.ResolvedValues[constParameter]).ToString();
            Assert.Equal(constParamValue, value);
        }

        [Fact(DisplayName = nameof(TestConstantDeferredConfig))]
        public void TestConstantDeferredConfig()
        {
            string variableName = "myConstant";
            string value = "1048576";

            IJsonDocumentObjectModelFactory domFactory = new JsonDomFactory();
            Dictionary<string, IJsonToken> jsonParameters = new Dictionary<string, IJsonToken>
            {
                { "value", domFactory.CreateValue(value) }
            };
            GeneratedSymbolDeferredMacroConfig deferredConfig = new GeneratedSymbolDeferredMacroConfig("ConstantMacro", null, variableName, jsonParameters);

            IVariableCollection variables = new VariableCollection();
            IRunnableProjectConfig config = new SimpleConfigModel();
            IParameterSet parameters = new ParameterSet(config);
            ParameterSetter setter = MacroTestHelpers.TestParameterSetter(EngineEnvironmentSettings, parameters);

            ConstantMacro macro = new ConstantMacro();
            IMacroConfig realConfig = macro.CreateConfig(EngineEnvironmentSettings, deferredConfig);
            macro.EvaluateConfig(EngineEnvironmentSettings, variables, realConfig, parameters, setter);

            Assert.True(parameters.TryGetParameterDefinition(variableName, out ITemplateParameter constParameter));
            string constParamValue = (parameters.ResolvedValues[constParameter]).ToString();
            Assert.Equal(constParamValue, value);
        }
    }
}
