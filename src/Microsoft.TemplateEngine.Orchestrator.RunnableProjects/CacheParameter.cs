using Microsoft.TemplateEngine.Abstractions;
using Microsoft.TemplateEngine.Abstractions.Json;
using Microsoft.TemplateEngine.Utils.Json;

namespace Microsoft.TemplateEngine.Orchestrator.RunnableProjects
{
    public class CacheParameter : ICacheParameter, IAllowDefaultIfOptionWithoutValue
    {
        private CacheParameter()
        {
        }

        public CacheParameter(string dataType, string defaultValue, string description)
            :this(dataType, defaultValue, description, null)
        {
        }

        public CacheParameter(string dataType, string defaultValue, string description, string defaultIfOptionWithoutValue)
        {
            DataType = dataType;
            DefaultValue = defaultValue;
            Description = description;
            DefaultIfOptionWithoutValue = defaultIfOptionWithoutValue;
        }

        public string DataType { get; }

        public string DefaultValue { get; }

        public string Description { get; }

        public string DefaultIfOptionWithoutValue { get; set; }

        public bool ShouldSerializeDefaultIfOptionWithoutValue()
        {
            return !string.IsNullOrEmpty(DefaultIfOptionWithoutValue);
        }

        public IJsonBuilder<ICacheParameter> JsonBuilder { get; } = new JsonBuilder<ICacheParameter, CacheParameter>(() => new CacheParameter())
            .Map(p => p.DataType)
            .Map(p => p.DefaultValue)
            .Map(p => p.Description)
            .Map(p => p.DefaultIfOptionWithoutValue);
    }
}
