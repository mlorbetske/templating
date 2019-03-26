using Microsoft.TemplateEngine.Abstractions.Json;

namespace Microsoft.TemplateEngine.Abstractions
{
    public interface ICacheParameter : IJsonSerializable<ICacheParameter>
    {
        string DataType { get; }

        string DefaultValue { get; }

        string Description { get; }
    }
}
