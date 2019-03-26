using System;
using System.Collections.Generic;
using Microsoft.TemplateEngine.Abstractions.Json;
using Microsoft.TemplateEngine.Utils.Json;

namespace Microsoft.TemplateEngine.Edge.Settings
{
    // TODO: Move this out of edge.
    internal static class JsonSerializerHelpers
    {
        public static bool TrySerializeStringDictionary(IJsonDocumentObjectModelFactory domFactory, IReadOnlyDictionary<string, string> toSerialize, out IJsonObject serialized)
        {
            try
            {
                serialized = domFactory.CreateObject();

                foreach (KeyValuePair<string, string> entry in toSerialize)
                {
                    serialized.SetValue(entry.Key, entry.Value);
                }

                return true;
            }
            catch
            {
                serialized = null;
                return false;
            }
        }

        public static Func<string, string> StringKeyConverter = input => input;
        public static Func<Guid, string> GuidKeyConverter = input => input.ToString();

        public static Func<IJsonDocumentObjectModelFactory, string, IJsonToken> StringValueConverter = (domFactory, input) => { return domFactory.CreateValue(input); };
        public static Func<IJsonDocumentObjectModelFactory, Guid, IJsonToken> GuidValueConverter = (domFactory, input) => { return domFactory.CreateValue(input); };

        public static bool TrySerializeDictionary<TKey, TValue>(IJsonDocumentObjectModelFactory domFactory, IReadOnlyDictionary<TKey, TValue> toSerialize, Func<TKey, string> keyConverter, Func<IJsonDocumentObjectModelFactory, TValue, IJsonToken> valueConverter, out IJsonObject serialized)
        {
            try
            {
                serialized = domFactory.CreateObject();

                foreach (KeyValuePair<TKey, TValue> entry in toSerialize)
                {
                    string key = keyConverter(entry.Key);
                    IJsonToken value = valueConverter(domFactory, entry.Value);
                    serialized.SetValue(key, value);
                }

                return true;
            }
            catch
            {
                serialized = null;
                return false;
            }
        }

        public static bool TrySerializeIEnumerable<T>(IJsonDocumentObjectModelFactory domFactory, IEnumerable<T> toSerialize, Func<IJsonDocumentObjectModelFactory, T, IJsonToken> elementConverter, out IJsonArray serialized)
        {
            try
            {
                IJsonArray arr = domFactory.CreateArray();

                foreach (T entry in toSerialize)
                {
                    arr.Add(elementConverter(domFactory, entry));
                }

                serialized = arr;
                return true;
            }
            catch
            {
                serialized = null;
                return false;
            }
        }
    }
}
