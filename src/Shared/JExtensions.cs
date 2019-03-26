using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.TemplateEngine.Abstractions.Json;
using Microsoft.TemplateEngine.Utils;
using Microsoft.TemplateEngine.Utils.Json;

namespace Microsoft.TemplateEngine
{
    internal static class JExtensions
    {
        public static string ToString(this IJsonToken token, string key)
        {
            if (key == null)
            {
                if (token == null || token.TokenType != JsonTokenType.String)
                {
                    return null;
                }

                return ((IJsonValue)token).Value.ToString();
            }

            if (!(token is IJsonObject obj))
            {
                return null;
            }

            if (!obj.TryGetValue(key, StringComparison.OrdinalIgnoreCase, out IJsonToken element) || element.TokenType != JsonTokenType.String)
            {
                return null;
            }

            return ((IJsonValue)element).Value.ToString();
        }

        public static bool ToBool(this IJsonToken token, string key = null, bool defaultValue = false)
        {
            IJsonToken checkToken;

            // determine which token to bool-ify
            if (token == null)
            {
                return defaultValue;
            }
            else if (key == null)
            {
                checkToken = token;
            }
            else if (!((IJsonObject)token).TryGetValue(key, StringComparison.OrdinalIgnoreCase, out checkToken))
            {
                return defaultValue;
            }

            // do the conversion on checkToken
            if (checkToken.TokenType == JsonTokenType.Boolean || checkToken.TokenType == JsonTokenType.String)
            {
                return string.Equals(((IJsonValue)checkToken).Value.ToString(), "true", StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                return defaultValue;
            }
        }

        public static int ToInt32(this IJsonToken token, string key = null, int defaultValue = 0)
        {
            int value;
            if (key == null)
            {
                if (token is IJsonValue valueToken)
                {
                    if (token.TokenType == JsonTokenType.Number)
                    {
                        return (int)(double)valueToken.Value;
                    }

                    if (int.TryParse(valueToken.Value.ToString(), out value))
                    {
                        return value;
                    }
                }

                return defaultValue;
            }

            if (!(token is IJsonObject obj))
            {
                return defaultValue;
            }

            if (!obj.TryGetValue(key, StringComparison.OrdinalIgnoreCase, out IJsonToken element))
            {
                return defaultValue;
            }
            else if (element.TokenType == JsonTokenType.Number)
            {
                return element.ToInt32();
            }
            else if (int.TryParse(((IJsonValue)element).Value.ToString(), out value))
            {
                return value;
            }

            return defaultValue;
        }

        public static T ToEnum<T>(this IJsonToken token, string key = null, T defaultValue = default)
            where T : struct
        {
            string val = token.ToString(key);
            if (val == null || !Enum.TryParse(val, out T result))
            {
                return defaultValue;
            }

            return result;
        }

        public static Guid ToGuid(this IJsonToken token, string key = null, Guid defaultValue = default)
        {
            string val = token.ToString(key);
            if (val == null || !Guid.TryParse(val, out Guid result))
            {
                return defaultValue;
            }

            return result;
        }

        public static IEnumerable<KeyValuePair<string, IJsonToken>> PropertiesOf(this IJsonToken token, string key = null)
        {
            if (!(token is IJsonObject obj))
            {
                return Empty<KeyValuePair<string, IJsonToken>>.List.Value;
            }

            if (key != null)
            {
                if (!obj.ExtractValues((key, t => obj = t as IJsonObject)).Contains(key))
                {
                    return Empty<KeyValuePair<string, IJsonToken>>.List.Value;
                }
            }

            if (obj == null)
            {
                return Empty<KeyValuePair<string, IJsonToken>>.List.Value;
            }

            return obj.Properties();
        }

        public static T Get<T>(this IJsonToken token, string key)
            where T : class, IJsonToken
        {
            if (!(token is IJsonObject obj))
            {
                return default;
            }

            if (!obj.TryGetValue(key, StringComparison.OrdinalIgnoreCase, out IJsonToken res))
            {
                return default;
            }

            return res as T;
        }

        public static IReadOnlyDictionary<string, string> ToStringDictionary(this IJsonToken token, StringComparer comparer = null, string propertyName = null)
        {
            Dictionary<string, string> result = new Dictionary<string, string>(comparer ?? StringComparer.Ordinal);

            foreach (KeyValuePair<string, IJsonToken> property in token.PropertiesOf(propertyName))
            {
                if (property.Value == null || property.Value.TokenType != JsonTokenType.String)
                {
                    continue;
                }

                result[property.Key] = ((IJsonValue)property.Value).Value.ToString();
            }

            return result;
        }

        public static string GetJsonString(this IJsonToken token)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                token.WriteToStream(ms);
                ms.Position = 0;

                using (StreamReader reader = new StreamReader(ms, Encoding.UTF8, true, 1024))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        // reads a dictionary whose values can either be string literals, or arrays of strings.
        public static IReadOnlyDictionary<string, IReadOnlyList<string>> ToStringListDictionary(this IJsonToken token, StringComparer comparer = null, string propertyName = null)
        {
            Dictionary<string, IReadOnlyList<string>> result = new Dictionary<string, IReadOnlyList<string>>(comparer ?? StringComparer.Ordinal);

            foreach (KeyValuePair<string, IJsonToken> property in token.PropertiesOf(propertyName))
            {
                if (property.Value == null)
                {
                    continue;
                }
                else if (property.Value.TokenType == JsonTokenType.String)
                {
                    result[property.Key] = new List<string>() { ((IJsonValue)property.Value).Value.ToString() };
                }
                else if (property.Value.TokenType == JsonTokenType.Array)
                {
                    result[property.Key] = property.Value.ArrayAsStrings();
                }
            }

            return result;
        }

        // Leaves the values as JTokens.
        public static IReadOnlyDictionary<string, IJsonToken> ToJsonTokenDictionary(this IJsonToken token, StringComparer comparaer = null, string propertyName = null)
        {
            Dictionary<string, IJsonToken> result = new Dictionary<string, IJsonToken>(comparaer ?? StringComparer.Ordinal);

            foreach (KeyValuePair<string, IJsonToken> property in token.PropertiesOf(propertyName))
            {
                result[property.Key] = property.Value;
            }

            return result;
        }

        public static IReadOnlyList<string> ArrayAsStrings(this IJsonToken token, string propertyName = null)
        {
            if (propertyName != null)
            {
                token = token.Get<IJsonArray>(propertyName);
            }

            if (!(token is IJsonArray arr))
            {
                return Empty<string>.List.Value;
            }

            List<string> values = new List<string>();

            foreach (IJsonToken item in arr)
            {
                if (item != null && item.TokenType == JsonTokenType.String)
                {
                    values.Add(((IJsonValue)item).Value.ToString());
                }
            }

            return values;
        }

        public static IReadOnlyList<Guid> ArrayAsGuids(this IJsonToken token, string propertyName = null)
        {
            if (propertyName != null)
            {
                token = token.Get<IJsonArray>(propertyName);
            }

            if (!(token is IJsonArray arr))
            {
                return Empty<Guid>.List.Value;
            }

            List<Guid> values = new List<Guid>();

            foreach (IJsonToken item in arr)
            {
                if (item != null && item.TokenType == JsonTokenType.String)
                {
                    if (Guid.TryParse(((IJsonValue)item).Value.ToString(), out Guid val))
                    {
                        values.Add(val);
                    }
                }
            }

            return values;
        }

        public static IEnumerable<T> Items<T>(this IJsonToken token, string propertyName = null)
            where T : IJsonToken
        {
            if (propertyName != null)
            {
                token = token.Get<IJsonArray>(propertyName);
            }

            if (!(token is IJsonArray arr))
            {
                yield break;
            }

            foreach (IJsonToken item in arr)
            {
                if (item is T res)
                {
                    yield return res;
                }
            }
        }
    }
}
