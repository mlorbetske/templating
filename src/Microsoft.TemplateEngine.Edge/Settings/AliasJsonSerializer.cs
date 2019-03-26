using System.Collections.Generic;
using Microsoft.TemplateEngine.Abstractions.Json;

namespace Microsoft.TemplateEngine.Edge.Settings
{
    internal static class AliasJsonSerializer
    {
        public static bool TrySerialize(IJsonDocumentObjectModelFactory domFactory, AliasModel aliasModel, out string serialized)
        {
            IJsonObject commandAliasesObject = domFactory.CreateObject();

            foreach (KeyValuePair<string, IReadOnlyList<string>> alias in aliasModel.CommandAliases)
            {
                if (JsonSerializerHelpers.TrySerializeIEnumerable(domFactory, alias.Value, JsonSerializerHelpers.StringValueConverter, out IJsonArray serializedAlias))
                {
                    commandAliasesObject.SetValue(alias.Key, serializedAlias);
                }
                else
                {
                    serialized = null;
                    return false;
                }
            }

            IJsonObject serializedObject = domFactory.CreateObject();
            serializedObject.SetValue(nameof(aliasModel.CommandAliases), commandAliasesObject);

            serialized = serializedObject.GetJsonString();
            return true;
        }
    }
}
