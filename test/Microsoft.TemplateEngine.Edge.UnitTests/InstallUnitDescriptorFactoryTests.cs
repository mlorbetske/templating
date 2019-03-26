using dotnet_new3;
using Microsoft.TemplateEngine.Abstractions.Json;
using Microsoft.TemplateEngine.Abstractions.TemplateUpdates;
using Microsoft.TemplateEngine.Edge.TemplateUpdates;
using Microsoft.TemplateEngine.TestHelper;
using Xunit;

namespace Microsoft.TemplateEngine.Edge.UnitTests
{
    public class InstallUnitDescriptorFactoryTests : TestBase
    {
        [Fact(DisplayName = nameof(InstallUnitDescriptorFactoryTryParseFailsGracefullyOnNullDescriptorObjectTest))]
        public void InstallUnitDescriptorFactoryTryParseFailsGracefullyOnNullDescriptorObjectTest()
        {
            Assert.False(InstallUnitDescriptorFactory.TryParse(EngineEnvironmentSettings, null, out _));
        }

        [Fact(DisplayName = nameof(InstallUnitDescriptorFactoryFailsGracefullyOnUnknownDescriptorFactoryIdTest))]
        public void InstallUnitDescriptorFactoryFailsGracefullyOnUnknownDescriptorFactoryIdTest()
        {
            // guid was randomly generated, doesn't match any descriptor factory
            string serializedDescriptor = @"
{
    ""FactoryId"": ""25AB3648-DC67-4A95-A658-5EEE8ADC2695"",
    ""Details"": {
    }
}";
            IJsonDocumentObjectModelFactory domFactory = new JsonDomFactory();
            Assert.True(domFactory.TryParse(serializedDescriptor, out IJsonToken descriptorToken));
            Assert.False(InstallUnitDescriptorFactory.TryParse(EngineEnvironmentSettings, (IJsonObject)descriptorToken, out _));
        }

        [Fact(DisplayName = nameof(InstallUnitDescriptorFactoryFailsGracefullyOnMissingFactoryIdTest))]
        public void InstallUnitDescriptorFactoryFailsGracefullyOnMissingFactoryIdTest()
        {
            string serializedDescriptor = @"
{
    ""Details"": {
    }
}";
            IJsonDocumentObjectModelFactory domFactory = new JsonDomFactory();
            Assert.True(domFactory.TryParse(serializedDescriptor, out IJsonToken descriptorToken));
            Assert.False(InstallUnitDescriptorFactory.TryParse(EngineEnvironmentSettings, (IJsonObject)descriptorToken, out _));
        }

        [Fact(DisplayName = nameof(InstallUnitDescriptorFactoryFailsGracefullyOnMissingDetailsTest))]
        public void InstallUnitDescriptorFactoryFailsGracefullyOnMissingDetailsTest()
        {
            string serializedDescriptor = @"
{
    ""FactoryId"": ""25AB3648-DC67-4A95-A658-5EEE8ADC2695"",
}";
            IJsonDocumentObjectModelFactory domFactory = new JsonDomFactory();
            domFactory.TryParse(serializedDescriptor, out IJsonToken token);
            IJsonObject descriptorObject = (IJsonObject)token;
            Assert.False(InstallUnitDescriptorFactory.TryParse(EngineEnvironmentSettings, descriptorObject, out _));
        }

        [Fact(DisplayName = nameof(InstallUnitDescriptorFactoryFailsGracefullyOnStructuredDetailsDataTest))]
        public void InstallUnitDescriptorFactoryFailsGracefullyOnStructuredDetailsDataTest()
        {
            string serializedDescriptor = @"
{
    ""FactoryId"": ""25AB3648-DC67-4A95-A658-5EEE8ADC2695"",
    ""Details"": {
        ""OuterKey"" : {
            ""InnerKey"": ""InnerValue""
        }
    }
}";
            IJsonDocumentObjectModelFactory domFactory = new JsonDomFactory();
            domFactory.TryParse(serializedDescriptor, out IJsonToken token);
            IJsonObject descriptorObject = (IJsonObject)token;
            Assert.False(InstallUnitDescriptorFactory.TryParse(EngineEnvironmentSettings, descriptorObject, out _));
        }
    }
}
