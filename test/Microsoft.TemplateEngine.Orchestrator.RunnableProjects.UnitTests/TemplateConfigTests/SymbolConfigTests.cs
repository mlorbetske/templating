using System;
using System.Collections.Generic;
using System.Linq;
using dotnet_new3;
using Microsoft.TemplateEngine.Abstractions.Json;
using Microsoft.TemplateEngine.Orchestrator.RunnableProjects.ValueForms;
using Microsoft.TemplateEngine.TestHelper;
using Xunit;

namespace Microsoft.TemplateEngine.Orchestrator.RunnableProjects.UnitTests.TemplateConfigTests
{
    public class SymbolConfigTests : TestBase
    {
        // Test that when a config doesn't include a name parameter, one gets added - with the proper value forms.
        [Fact(DisplayName = nameof(NameSymbolGetsAddedWithDefaultValueForms))]
        public void NameSymbolGetsAddedWithDefaultValueForms()
        {
            SimpleConfigModel configModel = SimpleConfigModel.FromJson(EngineEnvironmentSettings, ArrayConfigForSymbolWithFormsButNotIdentity);
            Assert.True(configModel.Symbols.ContainsKey("name"));

            ISymbolModel symbolInfo = configModel.Symbols["name"];
            Assert.True(symbolInfo is ParameterSymbol);

            ParameterSymbol nameSymbol = symbolInfo as ParameterSymbol;
            IList<string> configuredValueFormNames = nameSymbol.Forms.GlobalForms.ToList();

            Assert.Equal(5, configuredValueFormNames.Count);
            Assert.Equal(IdentityValueForm.FormName, configuredValueFormNames[0]);
            Assert.Equal(DefaultSafeNameValueFormModel.FormName, configuredValueFormNames[1]);
            Assert.Equal(DefaultLowerSafeNameValueFormModel.FormName, configuredValueFormNames[2]);
            Assert.Equal(DefaultSafeNamespaceValueFormModel.FormName, configuredValueFormNames[3]);
            Assert.Equal(DefaultLowerSafeNamespaceValueFormModel.FormName, configuredValueFormNames[4]);
        }

        // Test that when a symbol doens't explicitly include the "identity" value form, it gets added as the first form.
        [Fact(DisplayName = nameof(ParameterSymbolWithoutIdentityValueFormGetsIdentityAddedAsFirst))]
        public void ParameterSymbolWithoutIdentityValueFormGetsIdentityAddedAsFirst()
        {
            SimpleConfigModel configModel = SimpleConfigModel.FromJson(EngineEnvironmentSettings, ArrayConfigForSymbolWithFormsButNotIdentity);
            Assert.True(configModel.Symbols.ContainsKey("testSymbol"));

            ISymbolModel symbolInfo = configModel.Symbols["testSymbol"];
            Assert.True(symbolInfo is ParameterSymbol);

            ParameterSymbol paramSymbol = symbolInfo as ParameterSymbol;
            Assert.Single(paramSymbol.Forms.GlobalForms.ToList()
                                                .Where(x => string.Equals(x, IdentityValueForm.FormName, StringComparison.OrdinalIgnoreCase))
);
            Assert.Equal(0, paramSymbol.Forms.GlobalForms.ToList().IndexOf(IdentityValueForm.FormName));
        }

        private static IJsonObject ArrayConfigForSymbolWithFormsButNotIdentity
        {
            get
            {
                string configString = @"
{
  ""author"": ""Test Asset"",
  ""classifications"": [ ""Test Asset"" ],
  ""name"": ""TemplateWithValueForms"",
  ""generatorVersions"": ""[1.0.0.0-*)"",
  ""groupIdentity"": ""TestAssets.TemplateWithValueForms"",
  ""precedence"": ""100"",
  ""identity"": ""TestAssets.TemplateWithValueForms"",
  ""shortName"": ""TestAssets.TemplateWithValueForms"",
  ""symbols"": {
    ""testSymbol"": {
      ""type"": ""parameter"",
      ""dataType"": ""string"",
      ""forms"": {
        ""global"": [ ""foo"", ""bar"", ""baz"" ]
      }
    }
  }
}";
                new JsonDomFactory().TryParse(configString, out IJsonToken root);
                return (IJsonObject)root;
            }
        }

        // Tests that a name symbol with explicitly defined value forms but no identity form
        // gets the identity form added as the first form.
        [Fact(DisplayName = nameof(ArrayConfigNameSymbolWithoutIdentityFormGetsIdentityFormAddedAsFirst))]
        public void ArrayConfigNameSymbolWithoutIdentityFormGetsIdentityFormAddedAsFirst()
        {
            SimpleConfigModel configModel = SimpleConfigModel.FromJson(EngineEnvironmentSettings, ArrayConfigWithNameSymbolAndValueFormsButNotIdentity);
            Assert.True(configModel.Symbols.ContainsKey("name"));

            ISymbolModel symbolInfo = configModel.Symbols["name"];
            Assert.True(symbolInfo is ParameterSymbol);

            ParameterSymbol nameSymbol = symbolInfo as ParameterSymbol;
            IList<string> configuredValueFormNames = nameSymbol.Forms.GlobalForms.ToList();
            Assert.Equal(4, configuredValueFormNames.Count);
            Assert.Equal(IdentityValueForm.FormName, configuredValueFormNames[0]);
            Assert.Equal("foo", configuredValueFormNames[1]);
            Assert.Equal("bar", configuredValueFormNames[2]);
            Assert.Equal("baz", configuredValueFormNames[3]);
        }

        private static IJsonObject ArrayConfigWithNameSymbolAndValueFormsButNotIdentity
        {
            get
            {
                string configString = @"
{
  ""author"": ""Test Asset"",
  ""classifications"": [ ""Test Asset"" ],
  ""name"": ""TemplateWithValueForms"",
  ""generatorVersions"": ""[1.0.0.0-*)"",
  ""groupIdentity"": ""TestAssets.TemplateWithValueForms"",
  ""precedence"": ""100"",
  ""identity"": ""TestAssets.TemplateWithValueForms"",
  ""shortName"": ""TestAssets.TemplateWithValueForms"",
  ""symbols"": {
    ""name"": {
      ""type"": ""parameter"",
      ""dataType"": ""string"",
      ""forms"": {
        ""global"": [ ""foo"", ""bar"", ""baz"" ]
      }
    }
  }
}";
                new JsonDomFactory().TryParse(configString, out IJsonToken root);
                return (IJsonObject)root;
            }
        }

        [Fact(DisplayName = nameof(ExplicitNameSymbolWithoutBindingGetsDefaultNameBinding))]
        public void ExplicitNameSymbolWithoutBindingGetsDefaultNameBinding()
        {
            SimpleConfigModel configModel = SimpleConfigModel.FromJson(EngineEnvironmentSettings, ConfigWithNameSymbolWithoutBinding);
            Assert.True(configModel.Symbols.ContainsKey("name"));

            ISymbolModel symbolInfo = configModel.Symbols["name"];
            Assert.True(symbolInfo is ParameterSymbol);

            Assert.Equal("name", symbolInfo.Binding);
        }

        private static IJsonObject ConfigWithNameSymbolWithoutBinding
        {
            get
            {
                string configString = @"
{
  ""author"": ""Test Asset"",
  ""classifications"": [ ""Test Asset"" ],
  ""name"": ""TemplateWithNameSymbolWithoutBinding"",
  ""generatorVersions"": ""[1.0.0.0-*)"",
  ""groupIdentity"": ""TestAssets.TemplateWithNameSymbolWithoutBinding"",
  ""precedence"": ""100"",
  ""identity"": ""TestAssets.TemplateWithNameSymbolWithoutBinding"",
  ""shortName"": ""TestAssets.TemplateWithNameSymbolWithoutBinding"",
  ""symbols"": {
    ""name"": {
      ""type"": ""parameter"",
      ""dataType"": ""string"",
    }
  }
}";
                new JsonDomFactory().TryParse(configString, out IJsonToken root);
                return (IJsonObject)root;
            }
        }

        [Fact(DisplayName = nameof(ExplicitNameSymbolWithCustomBindingRetainsCustomBinding))]
        public void ExplicitNameSymbolWithCustomBindingRetainsCustomBinding()
        {
            SimpleConfigModel configModel = SimpleConfigModel.FromJson(EngineEnvironmentSettings, ConfigWithNameSymbolWithCustomBinding);
            Assert.True(configModel.Symbols.ContainsKey("name"));

            ISymbolModel symbolInfo = configModel.Symbols["name"];
            Assert.True(symbolInfo is ParameterSymbol);

            Assert.Equal("customBinding", symbolInfo.Binding);
        }

        private static IJsonObject ConfigWithNameSymbolWithCustomBinding
        {
            get
            {
                string configString = @"
{
  ""author"": ""Test Asset"",
  ""classifications"": [ ""Test Asset"" ],
  ""name"": ""ConfigWithNameSymbolWithCustomBinding"",
  ""generatorVersions"": ""[1.0.0.0-*)"",
  ""groupIdentity"": ""TestAssets.ConfigWithNameSymbolWithCustomBinding"",
  ""precedence"": ""100"",
  ""identity"": ""TestAssets.ConfigWithNameSymbolWithCustomBinding"",
  ""shortName"": ""TestAssets.ConfigWithNameSymbolWithCustomBinding"",
  ""symbols"": {
    ""name"": {
      ""type"": ""parameter"",
      ""dataType"": ""string"",
      ""binding"": ""customBinding"",
    }
  }
}";
                new JsonDomFactory().TryParse(configString, out IJsonToken root);
                return (IJsonObject)root;
            }
        }



        [Fact(DisplayName = nameof(ArrayConfigNameSymbolWithIdentityFormRetainsConfiguredFormsExactly))]
        public void ArrayConfigNameSymbolWithIdentityFormRetainsConfiguredFormsExactly()
        {
            SimpleConfigModel configModel = SimpleConfigModel.FromJson(EngineEnvironmentSettings, ArrayConfigWithNameSymbolAndValueFormsWithIdentity);
            Assert.True(configModel.Symbols.ContainsKey("name"));

            ISymbolModel symbolInfo = configModel.Symbols["name"];
            Assert.True(symbolInfo is ParameterSymbol);

            ParameterSymbol nameSymbol = symbolInfo as ParameterSymbol;
            IList<string> configuredValueFormNames = nameSymbol.Forms.GlobalForms.ToList();
            Assert.Equal(4, configuredValueFormNames.Count);
            Assert.Equal("foo", configuredValueFormNames[0]);
            Assert.Equal("bar", configuredValueFormNames[1]);
            Assert.Equal("baz", configuredValueFormNames[2]);
            Assert.Equal(IdentityValueForm.FormName, configuredValueFormNames[3]);
        }

        private static IJsonObject ArrayConfigWithNameSymbolAndValueFormsWithIdentity
        {
            get
            {
                string configString = @"
{
  ""author"": ""Test Asset"",
  ""classifications"": [ ""Test Asset"" ],
  ""name"": ""TemplateWithValueForms"",
  ""generatorVersions"": ""[1.0.0.0-*)"",
  ""groupIdentity"": ""TestAssets.TemplateWithValueForms"",
  ""precedence"": ""100"",
  ""identity"": ""TestAssets.TemplateWithValueForms"",
  ""shortName"": ""TestAssets.TemplateWithValueForms"",
  ""symbols"": {
    ""name"": {
      ""type"": ""parameter"",
      ""dataType"": ""string"",
      ""forms"": {
        ""global"": [ ""foo"", ""bar"", ""baz"", ""identity"" ]
      }
    }
  }
}";
                new JsonDomFactory().TryParse(configString, out IJsonToken root);
                return (IJsonObject)root;
            }
        }

        [Fact(DisplayName = nameof(ObjectConfigNameSymbolWithIdentityFormAndAddIdentityFalseRetainsConfiguredFormsExactly))]
        public void ObjectConfigNameSymbolWithIdentityFormAndAddIdentityFalseRetainsConfiguredFormsExactly()
        {
            SimpleConfigModel configModel = SimpleConfigModel.FromJson(EngineEnvironmentSettings, ObjectConfigNameSymbolWithIdentityFormAndAddIdentityFalse);
            Assert.True(configModel.Symbols.ContainsKey("name"));

            ISymbolModel symbolInfo = configModel.Symbols["name"];
            Assert.True(symbolInfo is ParameterSymbol);

            ParameterSymbol nameSymbol = symbolInfo as ParameterSymbol;
            IList<string> configuredValueFormNames = nameSymbol.Forms.GlobalForms.ToList();
            Assert.Equal(4, configuredValueFormNames.Count);
            Assert.Equal("foo", configuredValueFormNames[0]);
            Assert.Equal("bar", configuredValueFormNames[1]);
            Assert.Equal(IdentityValueForm.FormName, configuredValueFormNames[2]);
            Assert.Equal("baz", configuredValueFormNames[3]);
        }

        private static IJsonObject ObjectConfigNameSymbolWithIdentityFormAndAddIdentityFalse
        {
            get
            {
                string configString = @"
{
  ""author"": ""Test Asset"",
  ""classifications"": [ ""Test Asset"" ],
  ""name"": ""TemplateWithValueForms"",
  ""generatorVersions"": ""[1.0.0.0-*)"",
  ""groupIdentity"": ""TestAssets.TemplateWithValueForms"",
  ""precedence"": ""100"",
  ""identity"": ""TestAssets.TemplateWithValueForms"",
  ""shortName"": ""TestAssets.TemplateWithValueForms"",
  ""symbols"": {
    ""name"": {
      ""type"": ""parameter"",
      ""dataType"": ""string"",
      ""forms"": {
        ""global"": {
            ""forms"": [ ""foo"", ""bar"", ""identity"", ""baz"" ],
            ""addIdentity"": ""false""
        }
      }
    }
  }
}";
                new JsonDomFactory().TryParse(configString, out IJsonToken root);
                return (IJsonObject)root;
            }
        }

        [Fact(DisplayName = nameof(ObjectConfigNameSymbolWithIdentityFormAndAddIdentityTrueRetainsConfiguredFormsExactly))]
        public void ObjectConfigNameSymbolWithIdentityFormAndAddIdentityTrueRetainsConfiguredFormsExactly()
        {
            SimpleConfigModel configModel = SimpleConfigModel.FromJson(EngineEnvironmentSettings, ObjectConfigNameSymbolWithIdentityFormAndAddIdentityTrue);
            Assert.True(configModel.Symbols.ContainsKey("name"));

            ISymbolModel symbolInfo = configModel.Symbols["name"];
            Assert.True(symbolInfo is ParameterSymbol);

            ParameterSymbol nameSymbol = symbolInfo as ParameterSymbol;
            IList<string> configuredValueFormNames = nameSymbol.Forms.GlobalForms.ToList();
            Assert.Equal(4, configuredValueFormNames.Count);
            Assert.Equal("foo", configuredValueFormNames[0]);
            Assert.Equal("bar", configuredValueFormNames[1]);
            Assert.Equal(IdentityValueForm.FormName, configuredValueFormNames[2]);
            Assert.Equal("baz", configuredValueFormNames[3]);
        }

        private static IJsonObject ObjectConfigNameSymbolWithIdentityFormAndAddIdentityTrue
        {
            get
            {
                string configString = @"
{
  ""author"": ""Test Asset"",
  ""classifications"": [ ""Test Asset"" ],
  ""name"": ""TemplateWithValueForms"",
  ""generatorVersions"": ""[1.0.0.0-*)"",
  ""groupIdentity"": ""TestAssets.TemplateWithValueForms"",
  ""precedence"": ""100"",
  ""identity"": ""TestAssets.TemplateWithValueForms"",
  ""shortName"": ""TestAssets.TemplateWithValueForms"",
  ""symbols"": {
    ""name"": {
      ""type"": ""parameter"",
      ""dataType"": ""string"",
      ""forms"": {
        ""global"": {
            ""forms"": [ ""foo"", ""bar"", ""identity"", ""baz"" ],
            ""addIdentity"": ""true""
        }
      }
    }
  }
}";
                new JsonDomFactory().TryParse(configString, out IJsonToken root);
                return (IJsonObject)root;
            }
        }

        private static IJsonObject ConfigWithObjectValueFormDefinitionAddIdentityFalse
        {
            get
            {
                string configString = @"
{
  ""author"": ""Test Asset"",
  ""classifications"": [ ""Test Asset"" ],
  ""name"": ""TemplateWithValueForms"",
  ""generatorVersions"": ""[1.0.0.0-*)"",
  ""groupIdentity"": ""TestAssets.TemplateWithValueForms"",
  ""precedence"": ""100"",
  ""identity"": ""TestAssets.TemplateWithValueForms"",
  ""shortName"": ""TestAssets.TemplateWithValueForms"",
  ""symbols"": {
    ""testSymbol"": {
      ""type"": ""parameter"",
      ""dataType"": ""string"",
      ""forms"": {
        ""global"": {
            ""forms"": [ ""foo"", ""bar"", ""baz"" ],
            ""addIdentity"": ""false""
        }
      }
    }
  }
}";
                new JsonDomFactory().TryParse(configString, out IJsonToken root);
                return (IJsonObject)root;
            }
        }

        [Fact(DisplayName = nameof(NameSymbolObjectValueFormDefinitionRespectsAddIdentityTrue))]
        public void NameSymbolObjectValueFormDefinitionRespectsAddIdentityTrue()
        {
            SimpleConfigModel configModel = SimpleConfigModel.FromJson(EngineEnvironmentSettings, NameConfigWithObjectValueFormDefinitionAddIdentityTrue);
            Assert.True(configModel.Symbols.ContainsKey("name"));

            ISymbolModel symbolInfo = configModel.Symbols["name"];
            Assert.True(symbolInfo is ParameterSymbol);

            ParameterSymbol paramSymbol = symbolInfo as ParameterSymbol;
            IList<string> configuredValueFormNames = paramSymbol.Forms.GlobalForms.ToList();

            Assert.Equal(4, configuredValueFormNames.Count);
            Assert.Equal(IdentityValueForm.FormName, configuredValueFormNames[0]);
            Assert.Equal("foo", configuredValueFormNames[1]);
            Assert.Equal("bar", configuredValueFormNames[2]);
            Assert.Equal("baz", configuredValueFormNames[3]);
        }

        private static IJsonObject NameConfigWithObjectValueFormDefinitionAddIdentityTrue
        {
            get
            {
                string configString = @"
{
  ""author"": ""Test Asset"",
  ""classifications"": [ ""Test Asset"" ],
  ""name"": ""TemplateWithValueForms"",
  ""generatorVersions"": ""[1.0.0.0-*)"",
  ""groupIdentity"": ""TestAssets.TemplateWithValueForms"",
  ""precedence"": ""100"",
  ""identity"": ""TestAssets.TemplateWithValueForms"",
  ""shortName"": ""TestAssets.TemplateWithValueForms"",
  ""symbols"": {
    ""name"": {
      ""type"": ""parameter"",
      ""dataType"": ""string"",
      ""forms"": {
        ""global"": {
            ""forms"": [ ""foo"", ""bar"", ""baz"" ],
            ""addIdentity"": ""true""
        }
      }
    }
  }
}";
                new JsonDomFactory().TryParse(configString, out IJsonToken root);
                return (IJsonObject)root;
            }
        }

        [Fact(DisplayName = nameof(NameSymbolObjectValueFormDefinitionRespectsAddIdentityFalse))]
        public void NameSymbolObjectValueFormDefinitionRespectsAddIdentityFalse()
        {
            SimpleConfigModel configModel = SimpleConfigModel.FromJson(EngineEnvironmentSettings, NameConfigWithObjectValueFormDefinitionAddIdentityFalse);
            Assert.True(configModel.Symbols.ContainsKey("name"));

            ISymbolModel symbolInfo = configModel.Symbols["name"];
            Assert.True(symbolInfo is ParameterSymbol);

            ParameterSymbol paramSymbol = symbolInfo as ParameterSymbol;
            IList<string> configuredValueFormNames = paramSymbol.Forms.GlobalForms.ToList();

            Assert.Equal(3, configuredValueFormNames.Count);
            Assert.Equal("foo", configuredValueFormNames[0]);
            Assert.Equal("bar", configuredValueFormNames[1]);
            Assert.Equal("baz", configuredValueFormNames[2]);
        }

        private static IJsonObject NameConfigWithObjectValueFormDefinitionAddIdentityFalse
        {
            get
            {
                string configString = @"
{
  ""author"": ""Test Asset"",
  ""classifications"": [ ""Test Asset"" ],
  ""name"": ""TemplateWithValueForms"",
  ""generatorVersions"": ""[1.0.0.0-*)"",
  ""groupIdentity"": ""TestAssets.TemplateWithValueForms"",
  ""precedence"": ""100"",
  ""identity"": ""TestAssets.TemplateWithValueForms"",
  ""shortName"": ""TestAssets.TemplateWithValueForms"",
  ""symbols"": {
    ""name"": {
      ""type"": ""parameter"",
      ""dataType"": ""string"",
      ""forms"": {
        ""global"": {
            ""forms"": [ ""foo"", ""bar"", ""baz"" ],
            ""addIdentity"": ""false""
        }
      }
    }
  }
}";
                new JsonDomFactory().TryParse(configString, out IJsonToken root);
                return (IJsonObject)root;
            }
        }

        [Fact(DisplayName = nameof(NameSymbolObjectValueFormDefinitionInfersAddIdentityTrue))]
        public void NameSymbolObjectValueFormDefinitionInfersAddIdentityTrue()
        {
            SimpleConfigModel configModel = SimpleConfigModel.FromJson(EngineEnvironmentSettings, NameConfigObjectValueFormWithoutIdentityAndAddIdentityUnspecified);
            Assert.True(configModel.Symbols.ContainsKey("name"));

            ISymbolModel symbolInfo = configModel.Symbols["name"];
            Assert.True(symbolInfo is ParameterSymbol);

            ParameterSymbol paramSymbol = symbolInfo as ParameterSymbol;
            IList<string> configuredValueFormNames = paramSymbol.Forms.GlobalForms.ToList();

            Assert.Equal(4, configuredValueFormNames.Count);
            Assert.Equal(IdentityValueForm.FormName, configuredValueFormNames[0]);
            Assert.Equal("foo", configuredValueFormNames[1]);
            Assert.Equal("bar", configuredValueFormNames[2]);
            Assert.Equal("baz", configuredValueFormNames[3]);
        }

        private static IJsonObject NameConfigObjectValueFormWithoutIdentityAndAddIdentityUnspecified
        {
            get
            {
                string configString = @"
{
  ""author"": ""Test Asset"",
  ""classifications"": [ ""Test Asset"" ],
  ""name"": ""TemplateWithValueForms"",
  ""generatorVersions"": ""[1.0.0.0-*)"",
  ""groupIdentity"": ""TestAssets.TemplateWithValueForms"",
  ""precedence"": ""100"",
  ""identity"": ""TestAssets.TemplateWithValueForms"",
  ""shortName"": ""TestAssets.TemplateWithValueForms"",
  ""symbols"": {
    ""name"": {
      ""type"": ""parameter"",
      ""dataType"": ""string"",
      ""forms"": {
        ""global"": {
            ""forms"": [ ""foo"", ""bar"", ""baz"" ],
        }
      }
    }
  }
}";
                new JsonDomFactory().TryParse(configString, out IJsonToken root);
                return (IJsonObject)root;
            }
        }

        [Fact(DisplayName = nameof(NameSymbolObjectValueFormWithIdentityWithoutAddIdentityRetainsConfiguredForms))]
        public void NameSymbolObjectValueFormWithIdentityWithoutAddIdentityRetainsConfiguredForms()
        {
            SimpleConfigModel configModel = SimpleConfigModel.FromJson(EngineEnvironmentSettings, NameConfigObjectValueFormWithIdentityAndAddIdentityUnspecified);
            Assert.True(configModel.Symbols.ContainsKey("name"));

            ISymbolModel symbolInfo = configModel.Symbols["name"];
            Assert.True(symbolInfo is ParameterSymbol);

            ParameterSymbol paramSymbol = symbolInfo as ParameterSymbol;
            IList<string> configuredValueFormNames = paramSymbol.Forms.GlobalForms.ToList();

            Assert.Equal(4, configuredValueFormNames.Count);
            Assert.Equal("foo", configuredValueFormNames[0]);
            Assert.Equal("bar", configuredValueFormNames[1]);
            Assert.Equal("baz", configuredValueFormNames[2]);
            Assert.Equal(IdentityValueForm.FormName, configuredValueFormNames[3]);
        }

        private static IJsonObject NameConfigObjectValueFormWithIdentityAndAddIdentityUnspecified
        {
            get
            {
                string configString = @"
{
  ""author"": ""Test Asset"",
  ""classifications"": [ ""Test Asset"" ],
  ""name"": ""TemplateWithValueForms"",
  ""generatorVersions"": ""[1.0.0.0-*)"",
  ""groupIdentity"": ""TestAssets.TemplateWithValueForms"",
  ""precedence"": ""100"",
  ""identity"": ""TestAssets.TemplateWithValueForms"",
  ""shortName"": ""TestAssets.TemplateWithValueForms"",
  ""symbols"": {
    ""name"": {
      ""type"": ""parameter"",
      ""dataType"": ""string"",
      ""forms"": {
        ""global"": {
            ""forms"": [ ""foo"", ""bar"", ""baz"", ""identity"" ],
        }
      }
    }
  }
}";
                new JsonDomFactory().TryParse(configString, out IJsonToken root);
                return (IJsonObject)root;
            }
        }

        [Fact(DisplayName = nameof(ParameterSymbolWithNoValueFormsGetsIdentityFormAdded))]
        public void ParameterSymbolWithNoValueFormsGetsIdentityFormAdded()
        {
            SimpleConfigModel configModel = SimpleConfigModel.FromJson(EngineEnvironmentSettings, ConfigForSymbolWithoutValueForms);
            Assert.True(configModel.Symbols.ContainsKey("testSymbol"));

            ISymbolModel symbolInfo = configModel.Symbols["testSymbol"];
            Assert.True(symbolInfo is ParameterSymbol);

            ParameterSymbol paramSymbol = symbolInfo as ParameterSymbol;
            IList<string> configuredValueFormNames = paramSymbol.Forms.GlobalForms.ToList();

            Assert.Equal(1, configuredValueFormNames.Count);
            Assert.Equal(IdentityValueForm.FormName, configuredValueFormNames[0]);
        }

        private static IJsonObject ConfigForSymbolWithoutValueForms
        {
            get
            {
                string configString = @"
{
  ""author"": ""Test Asset"",
  ""classifications"": [ ""Test Asset"" ],
  ""name"": ""TemplateWithValueForms"",
  ""generatorVersions"": ""[1.0.0.0-*)"",
  ""groupIdentity"": ""TestAssets.TemplateWithValueForms"",
  ""precedence"": ""100"",
  ""identity"": ""TestAssets.TemplateWithValueForms"",
  ""shortName"": ""TestAssets.TemplateWithValueForms"",
  ""symbols"": {
    ""testSymbol"": {
      ""type"": ""parameter"",
      ""dataType"": ""string""
    }
  }
}";
                new JsonDomFactory().TryParse(configString, out IJsonToken root);
                return (IJsonObject)root;
            }
        }

        // Test that when a symbol explicitly includes the "identity" value form, the value forms for the symbol remain unmodified.
        [Fact(DisplayName = nameof(ParameterSymbolWithArrayIdentityValueFormRetainsFormsUnmodified))]
        public void ParameterSymbolWithArrayIdentityValueFormRetainsFormsUnmodified()
        {
            SimpleConfigModel configModel = SimpleConfigModel.FromJson(EngineEnvironmentSettings, ArrayConfigForSymbolWithValueFormsIncludingIdentity);
            Assert.True(configModel.Symbols.ContainsKey("testSymbol"));

            ISymbolModel symbolInfo = configModel.Symbols["testSymbol"];
            Assert.True(symbolInfo is ParameterSymbol);

            ParameterSymbol paramSymbol = symbolInfo as ParameterSymbol;
            IList<string> configuredValueFormNames = paramSymbol.Forms.GlobalForms.ToList();

            Assert.Equal(4, configuredValueFormNames.Count);
            Assert.Equal("foo", configuredValueFormNames[0]);
            Assert.Equal("bar", configuredValueFormNames[1]);
            Assert.Equal("baz", configuredValueFormNames[2]);
            Assert.Equal(IdentityValueForm.FormName, configuredValueFormNames[3]);
        }

        private static IJsonObject ArrayConfigForSymbolWithValueFormsIncludingIdentity
        {
            get
            {
                string configString = @"
{
  ""author"": ""Test Asset"",
  ""classifications"": [ ""Test Asset"" ],
  ""name"": ""TemplateWithValueForms"",
  ""generatorVersions"": ""[1.0.0.0-*)"",
  ""groupIdentity"": ""TestAssets.TemplateWithValueForms"",
  ""precedence"": ""100"",
  ""identity"": ""TestAssets.TemplateWithValueForms"",
  ""shortName"": ""TestAssets.TemplateWithValueForms"",
  ""symbols"": {
    ""testSymbol"": {
      ""type"": ""parameter"",
      ""dataType"": ""string"",
      ""forms"": {
        ""global"": [ ""foo"", ""bar"", ""baz"", ""identity"" ]
      }
    }
  }
}";
                new JsonDomFactory().TryParse(configString, out IJsonToken root);
                return (IJsonObject)root;
            }
        }


        [Fact(DisplayName = nameof(ObjectValueFormDefinitionRespectsAddIdentityTrue))]
        public void ObjectValueFormDefinitionRespectsAddIdentityTrue()
        {
            SimpleConfigModel configModel = SimpleConfigModel.FromJson(EngineEnvironmentSettings, ConfigWithObjectValueFormDefinitionAddIdentityTrue);
            Assert.True(configModel.Symbols.ContainsKey("testSymbol"));

            ISymbolModel symbolInfo = configModel.Symbols["testSymbol"];
            Assert.True(symbolInfo is ParameterSymbol);

            ParameterSymbol paramSymbol = symbolInfo as ParameterSymbol;
            IList<string> configuredValueFormNames = paramSymbol.Forms.GlobalForms.ToList();

            Assert.Equal(4, configuredValueFormNames.Count);
            Assert.Equal(IdentityValueForm.FormName, configuredValueFormNames[0]);
            Assert.Equal("foo", configuredValueFormNames[1]);
            Assert.Equal("bar", configuredValueFormNames[2]);
            Assert.Equal("baz", configuredValueFormNames[3]);
        }

        private static IJsonObject ConfigWithObjectValueFormDefinitionAddIdentityTrue
        {
            get
            {
                string configString = @"
{
  ""author"": ""Test Asset"",
  ""classifications"": [ ""Test Asset"" ],
  ""name"": ""TemplateWithValueForms"",
  ""generatorVersions"": ""[1.0.0.0-*)"",
  ""groupIdentity"": ""TestAssets.TemplateWithValueForms"",
  ""precedence"": ""100"",
  ""identity"": ""TestAssets.TemplateWithValueForms"",
  ""shortName"": ""TestAssets.TemplateWithValueForms"",
  ""symbols"": {
    ""testSymbol"": {
      ""type"": ""parameter"",
      ""dataType"": ""string"",
      ""forms"": {
        ""global"": {
            ""forms"": [ ""foo"", ""bar"", ""baz"" ],
            ""addIdentity"": ""true""
        }
      }
    }
  }
}";
                new JsonDomFactory().TryParse(configString, out IJsonToken root);
                return (IJsonObject)root;
            }
        }

        [Fact(DisplayName = nameof(ObjectValueFormDefinitionRespectsAddIdentityFalse))]
        public void ObjectValueFormDefinitionRespectsAddIdentityFalse()
        {
            SimpleConfigModel configModel = SimpleConfigModel.FromJson(EngineEnvironmentSettings, ConfigWithObjectValueFormDefinitionAddIdentityFalse);
            Assert.True(configModel.Symbols.ContainsKey("testSymbol"));

            ISymbolModel symbolInfo = configModel.Symbols["testSymbol"];
            Assert.True(symbolInfo is ParameterSymbol);

            ParameterSymbol paramSymbol = symbolInfo as ParameterSymbol;
            IList<string> configuredValueFormNames = paramSymbol.Forms.GlobalForms.ToList();

            Assert.Equal(3, configuredValueFormNames.Count);
            Assert.Equal("foo", configuredValueFormNames[0]);
            Assert.Equal("bar", configuredValueFormNames[1]);
            Assert.Equal("baz", configuredValueFormNames[2]);
        }


        [Fact(DisplayName = nameof(ObjectConfigParameterSymbolWithIdentityFormAndAddIdentityFalseRetainsConfiguredFormsExactly))]
        public void ObjectConfigParameterSymbolWithIdentityFormAndAddIdentityFalseRetainsConfiguredFormsExactly()
        {
            SimpleConfigModel configModel = SimpleConfigModel.FromJson(EngineEnvironmentSettings, ObjectConfigParameterSymbolWithIdentityFormAndAddIdentityFalse);
            Assert.True(configModel.Symbols.ContainsKey("testSymbol"));

            ISymbolModel symbolInfo = configModel.Symbols["testSymbol"];
            Assert.True(symbolInfo is ParameterSymbol);

            ParameterSymbol nameSymbol = symbolInfo as ParameterSymbol;
            IList<string> configuredValueFormNames = nameSymbol.Forms.GlobalForms.ToList();
            Assert.Equal(4, configuredValueFormNames.Count);
            Assert.Equal("foo", configuredValueFormNames[0]);
            Assert.Equal("bar", configuredValueFormNames[1]);
            Assert.Equal(IdentityValueForm.FormName, configuredValueFormNames[2]);
            Assert.Equal("baz", configuredValueFormNames[3]);
        }

        private static IJsonObject ObjectConfigParameterSymbolWithIdentityFormAndAddIdentityFalse
        {
            get
            {
                string configString = @"
{
  ""author"": ""Test Asset"",
  ""classifications"": [ ""Test Asset"" ],
  ""name"": ""TemplateWithValueForms"",
  ""generatorVersions"": ""[1.0.0.0-*)"",
  ""groupIdentity"": ""TestAssets.TemplateWithValueForms"",
  ""precedence"": ""100"",
  ""identity"": ""TestAssets.TemplateWithValueForms"",
  ""shortName"": ""TestAssets.TemplateWithValueForms"",
  ""symbols"": {
    ""testSymbol"": {
      ""type"": ""parameter"",
      ""dataType"": ""string"",
      ""forms"": {
        ""global"": {
            ""forms"": [ ""foo"", ""bar"", ""identity"", ""baz"" ],
            ""addIdentity"": ""false""
        }
      }
    }
  }
}";
                new JsonDomFactory().TryParse(configString, out IJsonToken root);
                return (IJsonObject)root;
            }
        }

        [Fact(DisplayName = nameof(ObjectConfigParameterSymbolWithIdentityFormAndAddIdentityTrueRetainsConfiguredFormsExactly))]
        public void ObjectConfigParameterSymbolWithIdentityFormAndAddIdentityTrueRetainsConfiguredFormsExactly()
        {
            SimpleConfigModel configModel = SimpleConfigModel.FromJson(EngineEnvironmentSettings, ObjectConfigParameterSymbolWithIdentityFormAndAddIdentityTrue);
            Assert.True(configModel.Symbols.ContainsKey("testSymbol"));

            ISymbolModel symbolInfo = configModel.Symbols["testSymbol"];
            Assert.True(symbolInfo is ParameterSymbol);

            ParameterSymbol nameSymbol = symbolInfo as ParameterSymbol;
            IList<string> configuredValueFormNames = nameSymbol.Forms.GlobalForms.ToList();
            Assert.Equal(4, configuredValueFormNames.Count);
            Assert.Equal("foo", configuredValueFormNames[0]);
            Assert.Equal("bar", configuredValueFormNames[1]);
            Assert.Equal(IdentityValueForm.FormName, configuredValueFormNames[2]);
            Assert.Equal("baz", configuredValueFormNames[3]);
        }

        private static IJsonObject ObjectConfigParameterSymbolWithIdentityFormAndAddIdentityTrue
        {
            get
            {
                string configString = @"
{
  ""author"": ""Test Asset"",
  ""classifications"": [ ""Test Asset"" ],
  ""name"": ""TemplateWithValueForms"",
  ""generatorVersions"": ""[1.0.0.0-*)"",
  ""groupIdentity"": ""TestAssets.TemplateWithValueForms"",
  ""precedence"": ""100"",
  ""identity"": ""TestAssets.TemplateWithValueForms"",
  ""shortName"": ""TestAssets.TemplateWithValueForms"",
  ""symbols"": {
    ""testSymbol"": {
      ""type"": ""parameter"",
      ""dataType"": ""string"",
      ""forms"": {
        ""global"": {
            ""forms"": [ ""foo"", ""bar"", ""identity"", ""baz"" ],
            ""addIdentity"": ""true""
        }
      }
    }
  }
}";
                new JsonDomFactory().TryParse(configString, out IJsonToken root);
                return (IJsonObject)root;
            }
        }

        [Fact(DisplayName = nameof(ParameterSymbolObjectValueFormWithIdentityWithoutAddIdentityRetainsConfiguredForms))]
        public void ParameterSymbolObjectValueFormWithIdentityWithoutAddIdentityRetainsConfiguredForms()
        {
            SimpleConfigModel configModel = SimpleConfigModel.FromJson(EngineEnvironmentSettings, ParameterConfigObjectValueFormWithIdentityAndAddIdentityUnspecified);
            Assert.True(configModel.Symbols.ContainsKey("testSymbol"));

            ISymbolModel symbolInfo = configModel.Symbols["testSymbol"];
            Assert.True(symbolInfo is ParameterSymbol);

            ParameterSymbol paramSymbol = symbolInfo as ParameterSymbol;
            IList<string> configuredValueFormNames = paramSymbol.Forms.GlobalForms.ToList();

            Assert.Equal(4, configuredValueFormNames.Count);
            Assert.Equal("foo", configuredValueFormNames[0]);
            Assert.Equal("bar", configuredValueFormNames[1]);
            Assert.Equal("baz", configuredValueFormNames[2]);
            Assert.Equal(IdentityValueForm.FormName, configuredValueFormNames[3]);
        }

        private static IJsonObject ParameterConfigObjectValueFormWithIdentityAndAddIdentityUnspecified
        {
            get
            {
                string configString = @"
{
  ""author"": ""Test Asset"",
  ""classifications"": [ ""Test Asset"" ],
  ""name"": ""TemplateWithValueForms"",
  ""generatorVersions"": ""[1.0.0.0-*)"",
  ""groupIdentity"": ""TestAssets.TemplateWithValueForms"",
  ""precedence"": ""100"",
  ""identity"": ""TestAssets.TemplateWithValueForms"",
  ""shortName"": ""TestAssets.TemplateWithValueForms"",
  ""symbols"": {
    ""testSymbol"": {
      ""type"": ""parameter"",
      ""dataType"": ""string"",
      ""forms"": {
        ""global"": {
            ""forms"": [ ""foo"", ""bar"", ""baz"", ""identity"" ],
        }
      }
    }
  }
}";
                new JsonDomFactory().TryParse(configString, out IJsonToken root);
                return (IJsonObject)root;
            }
        }

        [Fact(DisplayName = nameof(ParameterSymbolObjectValueFormDefinitionInfersAddIdentityTrue))]
        public void ParameterSymbolObjectValueFormDefinitionInfersAddIdentityTrue()
        {
            SimpleConfigModel configModel = SimpleConfigModel.FromJson(EngineEnvironmentSettings, ParameterConfigObjectValueFormWithoutIdentityAndAddIdentityUnspecified);
            Assert.True(configModel.Symbols.ContainsKey("testSymbol"));

            ISymbolModel symbolInfo = configModel.Symbols["testSymbol"];
            Assert.True(symbolInfo is ParameterSymbol);

            ParameterSymbol paramSymbol = symbolInfo as ParameterSymbol;
            IList<string> configuredValueFormNames = paramSymbol.Forms.GlobalForms.ToList();

            Assert.Equal(4, configuredValueFormNames.Count);
            Assert.Equal(IdentityValueForm.FormName, configuredValueFormNames[0]);
            Assert.Equal("foo", configuredValueFormNames[1]);
            Assert.Equal("bar", configuredValueFormNames[2]);
            Assert.Equal("baz", configuredValueFormNames[3]);
        }

        private static IJsonObject ParameterConfigObjectValueFormWithoutIdentityAndAddIdentityUnspecified
        {
            get
            {
                string configString = @"
{
  ""author"": ""Test Asset"",
  ""classifications"": [ ""Test Asset"" ],
  ""name"": ""TemplateWithValueForms"",
  ""generatorVersions"": ""[1.0.0.0-*)"",
  ""groupIdentity"": ""TestAssets.TemplateWithValueForms"",
  ""precedence"": ""100"",
  ""identity"": ""TestAssets.TemplateWithValueForms"",
  ""shortName"": ""TestAssets.TemplateWithValueForms"",
  ""symbols"": {
    ""testSymbol"": {
      ""type"": ""parameter"",
      ""dataType"": ""string"",
      ""forms"": {
        ""global"": {
            ""forms"": [ ""foo"", ""bar"", ""baz"" ],
        }
      }
    }
  }
}";
                new JsonDomFactory().TryParse(configString, out IJsonToken root);
                return (IJsonObject)root;
            }
        }
    }
}
