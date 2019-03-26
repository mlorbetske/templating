using Microsoft.TemplateEngine.Abstractions;
using Microsoft.TemplateEngine.Abstractions.Json;
using Microsoft.TemplateEngine.Utils.Json;

namespace Microsoft.TemplateEngine.Utils
{
    public class CacheParameter : ICacheParameter, IAllowDefaultIfOptionWithoutValue
    {
        public string DataType { get; set; }

        public string DefaultValue { get; set; }

        public string Description { get; set; }

        public string DefaultIfOptionWithoutValue { get; set; }

        public IJsonBuilder<ICacheParameter> JsonBuilder { get; } = new JsonBuilder<ICacheParameter, CacheParameter>(() => new CacheParameter())
            .Map(p => p.DataType)
            .Map(p => p.DefaultValue)
            .Map(p => p.Description)
            .Map(p => p.DefaultIfOptionWithoutValue);
    }
}
