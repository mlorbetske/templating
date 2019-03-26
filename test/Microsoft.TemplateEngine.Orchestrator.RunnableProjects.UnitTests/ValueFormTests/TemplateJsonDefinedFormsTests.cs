using dotnet_new3;
using Microsoft.TemplateEngine.Abstractions.Json;
using Microsoft.TemplateEngine.Core.Contracts;
using Microsoft.TemplateEngine.Orchestrator.RunnableProjects.Macros.Config;
using Microsoft.TemplateEngine.TestHelper;
using Xunit;

namespace Microsoft.TemplateEngine.Orchestrator.RunnableProjects.UnitTests.ValueFormTests
{
    public class TemplateJsonDefinedFormsTests : TestBase
    {
        [Fact(DisplayName = nameof(UnknownFormNameOnParameterSymbolDoesNotThrow))]
        public void UnknownFormNameOnParameterSymbolDoesNotThrow()
        {
            string templateJson = @"
{
  ""name"": ""TestTemplate"",
  ""identity"": ""TestTemplate"",
  ""symbols"": {
    ""mySymbol"": {
      ""type"": ""parameter"",
      ""replaces"": ""whatever"",
      ""forms"": {
        ""global"": [ ""fakeName"" ],
      }
    }
  }
}
";
            new JsonDomFactory().TryParse(templateJson, out IJsonToken token);
            SimpleConfigModel configModel = SimpleConfigModel.FromJson(EngineEnvironmentSettings, (IJsonObject)token);
            IGlobalRunConfig runConfig = null;

            try
            {
                runConfig = ((IRunnableProjectConfig)configModel).OperationConfig;
            }
            catch
            {
                Assert.True(false, "Should not throw on unknown value form name");
            }

            Assert.NotNull(runConfig);
            Assert.Equal(1, runConfig.Macros.Count);
            
            Assert.True(runConfig.Macros[0] is ProcessValueFormMacroConfig);
            ProcessValueFormMacroConfig identityFormConfig = runConfig.Macros[0] as ProcessValueFormMacroConfig;
            Assert.Equal("identity", identityFormConfig.FormName);
        }

        [Fact(DisplayName = nameof(UnknownFormNameForDerivedSymbolValueDoesNotThrow))]
        public void UnknownFormNameForDerivedSymbolValueDoesNotThrow()
        {
            string templateJson = @"
{
  ""name"": ""TestTemplate"",
  ""identity"": ""TestTemplate"",
  ""symbols"": {
    ""original"": {
      ""type"": ""parameter"",
      ""replaces"": ""whatever"",
    },
    ""myDerivedSym"": {
      ""type"": ""derived"",
      ""valueSource"": ""original"",
      ""valueTransform"": ""fakeForm"",
      ""replaces"": ""something""
    }
  }
}
";
            new JsonDomFactory().TryParse(templateJson, out IJsonToken token);
            SimpleConfigModel configModel = SimpleConfigModel.FromJson(EngineEnvironmentSettings, (IJsonObject)token);
            IGlobalRunConfig runConfig = null;

            try
            {
                runConfig = ((IRunnableProjectConfig)configModel).OperationConfig;
            }
            catch
            {
                Assert.True(false, "Should not throw on unknown value form name");
            }

            Assert.NotNull(runConfig);
        }
    }
}
