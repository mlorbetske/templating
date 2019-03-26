using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.TemplateEngine.Abstractions.Json;

namespace Microsoft.TemplateEngine.Utils.Json
{
    internal class JsonSerialize<T>
        where T : IJsonSerializable<T>
    {
        public static Lazy<T> _instance;

        public static void Configure(Func<T> creator) => _instance = _instance ?? new Lazy<T>(creator);

        public static T Deserialize(IJsonToken token) => _instance.Value.JsonBuilder.Deserialize(token);

        public static IJsonToken Serialize(IJsonDocumentObjectModelFactory domFactory, T instance) => _instance.Value.JsonBuilder.Serialize(domFactory, instance);
    }

    internal class JsonSerialize<T, TConcrete>
        where T : IJsonSerializable<T>
        where TConcrete : T, new()
    {
        private static readonly TConcrete _instance = new TConcrete();

        public static TConcrete Deserialize(IJsonToken token) => (TConcrete)_instance.JsonBuilder.Deserialize(token);

        public static IJsonToken Serialize(IJsonDocumentObjectModelFactory domFactory, T instance) => _instance.JsonBuilder.Serialize(domFactory, instance);
    }

    public class DictionaryOperations<TResult, TResultConcrete, TElement, TElementConcrete>
        where TResultConcrete : TResult
        where TElementConcrete : TElement
    {
        private readonly JsonBuilder<TResult, TResultConcrete> _builder;
        private readonly Serialize<TElement> _elementSerializer;
        private readonly DirectDeserialize<TElementConcrete> _elementDeserializer;

        public DictionaryOperations(JsonBuilder<TResult, TResultConcrete> builder, Serialize<TElement> serializer, DirectDeserialize<TElementConcrete> deserializer)
        {
            _builder = builder;
            _elementSerializer = serializer;
            _elementDeserializer = deserializer;
        }

        public JsonBuilder<TResult, TResultConcrete> Map<TDictionary, TDictionaryConcrete>
            (Expression<Getter<TResultConcrete, TDictionary>> property,
                Func<TDictionaryConcrete> dictionaryCreator,
                Func<TDictionary, IEnumerable<KeyValuePair<string, TElement>>> getEnumerator,
                Action<TDictionaryConcrete, string, TElementConcrete> setElement,
                string propertyName = null)
            where TDictionaryConcrete : TDictionary
        {
            JsonBuilderExtensions.ProcessExpression(property, out Getter<TResultConcrete, TDictionary> getter, out Setter<TResultConcrete, TDictionary> setter, ref propertyName);
            return _builder.MapCore(getter, JsonBuilderExtensions.SerializeDictionary(getEnumerator, _elementSerializer), JsonBuilderExtensions.Deserialize(dictionaryCreator, _elementDeserializer, setElement), setter, propertyName);
        }

        public JsonBuilder<TResult, TResultConcrete> Map<TDictionary, TDictionaryConcrete>
            (Expression<Getter<TResultConcrete, TDictionary>> property,
                Func<TDictionaryConcrete> collectionCreator,
                string propertyName = null)
            where TDictionary : IEnumerable<KeyValuePair<string, TElement>>
            where TDictionaryConcrete : TDictionary, IDictionary<string, TElement>
            => Map(property, collectionCreator, x => x, (c, k, v) => c[k] = v, propertyName);

        public JsonBuilder<TResult, TResultConcrete> Map<TCollection, TCollectionConcrete>
            (Expression<Getter<TResultConcrete, TCollection>> property,
                string propertyName = null)
            where TCollection : IEnumerable<KeyValuePair<string, TElement>>
            where TCollectionConcrete : TCollection, IDictionary<string, TElement>, new()
            => Map(property, () => new TCollectionConcrete(), x => x, (c, k, v) => c[k] = v, propertyName);
    }

    public class ListOperations<TResult, TResultConcrete, TElement, TElementConcrete>
        where TResultConcrete : TResult
        where TElementConcrete : TElement
    {
        private readonly JsonBuilder<TResult, TResultConcrete> _builder;
        private readonly Serialize<TElement> _elementSerializer;
        private readonly DirectDeserialize<TElementConcrete> _elementDeserializer;

        public ListOperations(JsonBuilder<TResult, TResultConcrete> builder, Serialize<TElement> serializer, DirectDeserialize<TElementConcrete> deserializer)
        {
            _builder = builder;
            _elementSerializer = serializer;
            _elementDeserializer = deserializer;
        }

        public JsonBuilder<TResult, TResultConcrete> Map<TCollection, TCollectionConcrete>
            (Expression<Getter<TResultConcrete, TCollection>> property,
                Func<TCollectionConcrete> collectionCreator,
                Func<TCollection, IEnumerable<TElement>> getEnumerator,
                Action<TCollectionConcrete, TElementConcrete> addElement,
                string propertyName = null)
            where TCollectionConcrete : TCollection
        {
            JsonBuilderExtensions.ProcessExpression(property, out Getter<TResultConcrete, TCollection> getter, out Setter<TResultConcrete, TCollection> setter, ref propertyName);
            return _builder.MapCore(getter, JsonBuilderExtensions.SerializeCollection(getEnumerator, _elementSerializer), JsonBuilderExtensions.Deserialize(collectionCreator, _elementDeserializer, addElement), setter, propertyName);
        }

        public JsonBuilder<TResult, TResultConcrete> Map<TCollection, TCollectionConcrete>
            (Expression<Getter<TResultConcrete, TCollection>> property,
                Func<TCollectionConcrete> collectionCreator,
                string propertyName = null)
            where TCollection : IEnumerable<TElement>
            where TCollectionConcrete : TCollection, ICollection<TElement>
            => Map(property, collectionCreator, x => x, (c, v) => c.Add(v), propertyName);

        public JsonBuilder<TResult, TResultConcrete> Map<TCollection, TCollectionConcrete>
            (Expression<Getter<TResultConcrete, TCollection>> property,
                string propertyName = null)
            where TCollection : IEnumerable<TElement>
            where TCollectionConcrete : TCollection, ICollection<TElement>, new()
            => Map(property, () => new TCollectionConcrete(), x => x, (c, v) => c.Add(v), propertyName);

        public JsonBuilder<TResult, TResultConcrete> Map<TCollection>
            (Expression<Getter<TResultConcrete, TCollection>> property,
                string propertyName = null)
            where TCollection : ICollection<TElement>, new()
            => Map(property, () => new TCollection(), x => x, (c, v) => c.Add(v), propertyName);
    }

    public static class JsonBuilderExtensions
    {
        public static IJsonToken NullSerializer<T>(IJsonDocumentObjectModelFactory domFactory, T item) => null;

        public static DirectDeserialize<TCollection> Deserialize<TCollection, TElement>(Func<TCollection> collectionCreator, Func<TElement> elementCreator, Deserialize<TElement> elementDeserializer, Action<TCollection, TElement> setter)
            => Deserialize(collectionCreator, t => elementDeserializer(t, elementCreator), setter);

        public static DirectDeserialize<TCollection> Deserialize<TCollection, TElement>(Func<TCollection> collectionCreator, DirectDeserialize<TElement> elementDeserializer, Action<TCollection, TElement> setter)
        {
            return t =>
            {
                TCollection result = collectionCreator();

                if (t.TokenType == JsonTokenType.Array)
                {
                    IJsonArray source = (IJsonArray)t;

                    foreach (IJsonToken entry in source)
                    {
                        TElement element = elementDeserializer(entry);
                        setter(result, element);
                    }
                }
                else
                {
                    TElement element = elementDeserializer(t);
                    setter(result, element);
                }

                return result;
            };
        }

        public static DirectDeserialize<TDictionary> Deserialize<TDictionary, TElement>(Func<TDictionary> dictionaryCreator, Func<TElement> elementCreator, Deserialize<TElement> elementDeserializer, Action<TDictionary, string, TElement> setter)
            => Deserialize(dictionaryCreator, t => elementDeserializer(t, elementCreator), setter);

        public static DirectDeserialize<TDictionary> Deserialize<TDictionary, TElement>(Func<TDictionary> dictionaryCreator, DirectDeserialize<TElement> elementDeserializer, Action<TDictionary, string, TElement> setter)
        {
            return t =>
            {
                TDictionary result = dictionaryCreator();
                IJsonObject source = (IJsonObject)t;

                foreach (KeyValuePair<string, IJsonToken> entry in source.Properties())
                {
                    TElement element = elementDeserializer(entry.Value);
                    setter(result, entry.Key, element);
                }

                return result;
            };
        }

        public static bool DeserializeBool(IJsonToken token)
            => (bool)((IJsonValue)token).Value;

        public static int DeserializeInt(IJsonToken token)
            => (int)(double)((IJsonValue)token).Value;

        public static Guid DeserializeGuid(IJsonToken token)
            => Guid.Parse(DeserializeString(token));

        public static DateTime? DeserializeNullableDateTime(IJsonToken token)
        {
            string s = DeserializeString(token);

            if (s is null)
            {
                return null;
            }

            return DateTime.Parse(s, null, DateTimeStyles.RoundtripKind);
        }

        public static string DeserializeString(IJsonToken token)
            => ((IJsonValue)token).Value?.ToString();

        public static JsonBuilder<TResult, TResultConcrete> Map<TResult, TResultConcrete, T>(this JsonBuilder<TResult, TResultConcrete> builder, Expression<Getter<TResultConcrete, T>> property, string propertyName = null)
            where TResultConcrete : TResult
            where T : IJsonSerializable<T>, new()
            => builder.Map(property, () => new T(), propertyName);

        public static JsonBuilder<TResult, TResultConcrete> Map<TResult, TResultConcrete>(this JsonBuilder<TResult, TResultConcrete> builder, Expression<Getter<TResultConcrete, bool>> parameter, string propertyName = null)
            where TResultConcrete : TResult
            => builder.Map(parameter, Serialize, DeserializeBool, propertyName);

        public static JsonBuilder<TResult, TResultConcrete> Map<TResult, TResultConcrete>(this JsonBuilder<TResult, TResultConcrete> builder, Expression<Getter<TResultConcrete, DateTime?>> parameter, string propertyName = null)
            where TResultConcrete : TResult
            => builder.Map(parameter, Serialize, DeserializeNullableDateTime, propertyName);

        public static JsonBuilder<TResult, TResultConcrete> Map<TResult, TResultConcrete>(this JsonBuilder<TResult, TResultConcrete> builder, Expression<Getter<TResultConcrete, string>> parameter, string propertyName = null)
            where TResultConcrete : TResult
            => builder.Map(parameter, Serialize, DeserializeString, propertyName);

        public static JsonBuilder<TResult, TResultConcrete> Map<TResult, TResultConcrete>(this JsonBuilder<TResult, TResultConcrete> builder, Expression<Getter<TResultConcrete, int>> parameter, string propertyName = null)
            where TResultConcrete : TResult
            => builder.Map(parameter, Serialize, DeserializeInt, propertyName);

        public static JsonBuilder<TResult, TResultConcrete> Map<TResult, TResultConcrete>(this JsonBuilder<TResult, TResultConcrete> builder, Expression<Getter<TResultConcrete, Guid>> parameter, string propertyName = null)
            where TResultConcrete : TResult
            => builder.Map(parameter, Serialize, DeserializeGuid, propertyName);

        public static JsonBuilder<TResult, TResultConcrete> Map<TResult, TResultConcrete, T, TConcrete>(this JsonBuilder<TResult, TResultConcrete> builder, Expression<Getter<TResultConcrete, T>> property, Serialize<T> serialize, Deserialize<TConcrete> deserialize, Func<TConcrete> itemCreator, string propertyName = null)
            where TResultConcrete : TResult
            where TConcrete : T
        {
            ProcessExpression(property, out Getter<TResultConcrete, T> getter, out Setter<TResultConcrete, T> setter, ref propertyName);
            return builder.MapCore(getter, serialize, deserialize, setter, itemCreator, propertyName);
        }

        public static JsonBuilder<TResult, TResultConcrete> Map<TResult, TResultConcrete, T, TConcrete>(this JsonBuilder<TResult, TResultConcrete> builder, Expression<Getter<TResultConcrete, T>> property, Serialize<T> serialize, Deserialize<TConcrete> deserialize, string propertyName = null)
            where TResultConcrete : TResult
            where TConcrete : T, new()
            => builder.Map(property, serialize, deserialize, () => new TConcrete(), propertyName);

        public static JsonBuilder<TResult, TResultConcrete> Map<TResult, TResultConcrete, T>(this JsonBuilder<TResult, TResultConcrete> builder, Expression<Getter<TResultConcrete, T>> property, Serialize<T> serializer, DirectDeserialize<T> deserializer, string propertyName = null)
            where TResultConcrete : TResult
        {
            ProcessExpression(property, out Getter<TResultConcrete, T> getter, out Setter<TResultConcrete, T> setter, ref propertyName);
            return builder.MapCore(getter, serializer, deserializer, setter, propertyName);
        }

        public static JsonBuilder<TResult, TResultConcrete> Map<TResult, TResultConcrete, T>(this JsonBuilder<TResult, TResultConcrete> builder, Expression<Getter<TResultConcrete, T>> property, Func<T> itemCreator, string propertyName = null)
            where TResultConcrete : TResult
            where T : IJsonSerializable<T>
        {
            JsonSerialize<T>.Configure(itemCreator);
            ProcessExpression(property, out Getter<TResultConcrete, T> getter, out Setter<TResultConcrete, T> setter, ref propertyName);
            return builder.MapCore(getter, JsonSerialize<T>.Serialize, JsonSerialize<T>.Deserialize, setter, propertyName);
        }

        public static Serialize<TCollection> SerializeCollection<TCollection, TElement>(Serialize<TElement> elementSerializer)
            where TCollection : IEnumerable<TElement>
            => SerializeCollection<TCollection, TElement>(x => x, elementSerializer);

        public static Serialize<TCollection> SerializeCollection<TCollection, TElement>(Func<TCollection, IEnumerable<TElement>> getEnumerator, Serialize<TElement> elementSerializer)
        {
            return (domFactory, source) =>
            {
                IJsonArray result = domFactory.CreateArray();

                foreach (TElement entry in getEnumerator(source))
                {
                    result.Add(elementSerializer(domFactory, entry));
                }

                return result;
            };
        }

        public static Serialize<TDictionary> SerializeDictionary<TDictionary, TElement>(Serialize<TElement> elementSerializer)
            where TDictionary : IReadOnlyDictionary<string, TElement>
            => SerializeDictionary<TDictionary, TElement>(x => x, elementSerializer);

        public static Serialize<TDictionary> SerializeDictionary<TDictionary, TElement>(Func<TDictionary, IEnumerable<KeyValuePair<string, TElement>>> getEnumerator, Serialize<TElement> elementSerializer)
        {
            return (domFactory, source) =>
            {
                IJsonObject result = domFactory.CreateObject();

                foreach (KeyValuePair<string, TElement> entry in getEnumerator(source))
                {
                    result.SetValue(entry.Key, elementSerializer(domFactory, entry.Value));
                }

                return result;
            };
        }

        public static DictionaryOperations<TResult, TResultConcrete, TElement, TElementConcrete> Dictionary<TResult, TResultConcrete, TElement, TElementConcrete>(this JsonBuilder<TResult, TResultConcrete> builder, IWrapper<TElement, TElementConcrete> wrapper)
            where TResultConcrete : TResult
            where TElementConcrete : TElement
            => new DictionaryOperations<TResult, TResultConcrete, TElement, TElementConcrete>(builder, wrapper.Serialize, wrapper.Deserialize);

        public static DictionaryOperations<TResult, TResultConcrete, TElement, TElementConcrete> Dictionary<TResult, TResultConcrete, TElement, TElementConcrete>(this JsonBuilder<TResult, TResultConcrete> builder, Serialize<TElement> serializer, DirectDeserialize<TElementConcrete> deserializer)
            where TResultConcrete : TResult
            where TElementConcrete : TElement
            => new DictionaryOperations<TResult, TResultConcrete, TElement, TElementConcrete>(builder, serializer, deserializer);

        public static DictionaryOperations<TResult, TResultConcrete, TElement, TElementConcrete> Dictionary<TResult, TResultConcrete, TElement, TElementConcrete>(this JsonBuilder<TResult, TResultConcrete> builder)
            where TResultConcrete : TResult
            where TElement : IJsonSerializable<TElement>
            where TElementConcrete : TElement, new()
            => new DictionaryOperations<TResult, TResultConcrete, TElement, TElementConcrete>(builder, JsonSerialize<TElement, TElementConcrete>.Serialize, JsonSerialize<TElement, TElementConcrete>.Deserialize);

        public static DictionaryOperations<TResult, TResultConcrete, Guid, Guid> DictionaryOfGUid<TResult, TResultConcrete>(this JsonBuilder<TResult, TResultConcrete> builder)
            where TResultConcrete : TResult
            => builder.Dictionary<TResult, TResultConcrete, Guid, Guid>(Serialize, DeserializeGuid);

        public static DictionaryOperations<TResult, TResultConcrete, int, int> DictionaryOfInt<TResult, TResultConcrete>(this JsonBuilder<TResult, TResultConcrete> builder)
            where TResultConcrete : TResult
            => builder.Dictionary<TResult, TResultConcrete, int, int>(Serialize, DeserializeInt);

        public static DictionaryOperations<TResult, TResultConcrete, string, string> DictionaryOfString<TResult, TResultConcrete>(this JsonBuilder<TResult, TResultConcrete> builder)
            where TResultConcrete : TResult
            => builder.Dictionary<TResult, TResultConcrete, string, string>(Serialize, DeserializeString);

        public static DictionaryOperations<TResult, TResultConcrete, TElement, TElement> Dictionary<TResult, TResultConcrete, TElement>(this JsonBuilder<TResult, TResultConcrete> builder, Serialize<TElement> serializer, DirectDeserialize<TElement> deserializer)
            where TResultConcrete : TResult
            => builder.Dictionary<TResult, TResultConcrete, TElement, TElement>(serializer, deserializer);

        public static ListOperations<TResult, TResultConcrete, TElement, TElementConcrete> List<TResult, TResultConcrete, TElement, TElementConcrete>(this JsonBuilder<TResult, TResultConcrete> builder, IWrapper<TElement, TElementConcrete> wrapper)
            where TResultConcrete : TResult
            where TElementConcrete : TElement
            => new ListOperations<TResult, TResultConcrete, TElement, TElementConcrete>(builder, wrapper.Serialize, wrapper.Deserialize);

        public static ListOperations<TResult, TResultConcrete, TElement, TElementConcrete> List<TResult, TResultConcrete, TElement, TElementConcrete>(this JsonBuilder<TResult, TResultConcrete> builder, Serialize<TElement> serializer, DirectDeserialize<TElementConcrete> deserializer)
            where TResultConcrete : TResult
            where TElementConcrete : TElement
            => new ListOperations<TResult, TResultConcrete, TElement, TElementConcrete>(builder, serializer, deserializer);

        public static ListOperations<TResult, TResultConcrete, Guid, Guid> ListOfGuid<TResult, TResultConcrete>(this JsonBuilder<TResult, TResultConcrete> builder)
            where TResultConcrete : TResult
            => builder.List<TResult, TResultConcrete, Guid, Guid>(Serialize, DeserializeGuid);

        public static ListOperations<TResult, TResultConcrete, int, int> ListOfInt<TResult, TResultConcrete>(this JsonBuilder<TResult, TResultConcrete> builder)
            where TResultConcrete : TResult
            => builder.List<TResult, TResultConcrete, int, int>(Serialize, DeserializeInt);

        public static ListOperations<TResult, TResultConcrete, string, string> ListOfString<TResult, TResultConcrete>(this JsonBuilder<TResult, TResultConcrete> builder)
            where TResultConcrete : TResult
            => builder.List<TResult, TResultConcrete, string, string>(Serialize, DeserializeString);

        public static ListOperations<TResult, TResultConcrete, TElement, TElement> List<TResult, TResultConcrete, TElement>(this JsonBuilder<TResult, TResultConcrete> builder)
            where TResultConcrete : TResult
            where TElement : IJsonSerializable<TElement>, new()
            => builder.List<TResult, TResultConcrete, TElement, TElement>(JsonSerialize<TElement, TElement>.Serialize, JsonSerialize<TElement, TElement>.Deserialize);

        public static ListOperations<TResult, TResultConcrete, TElement, TElement> List<TResult, TResultConcrete, TElement>(this JsonBuilder<TResult, TResultConcrete> builder, Serialize<TElement> serializer, DirectDeserialize<TElement> deserializer)
            where TResultConcrete : TResult
            => builder.List<TResult, TResultConcrete, TElement, TElement>(serializer, deserializer);

        public static IJsonToken Serialize(IJsonDocumentObjectModelFactory domFactory, bool item)
            => domFactory.CreateValue(item);

        public static IJsonToken Serialize(IJsonDocumentObjectModelFactory domFactory, int item)
            => domFactory.CreateValue(item);

        public static IJsonToken Serialize(IJsonDocumentObjectModelFactory domFactory, Guid item)
            => domFactory.CreateValue(item.ToString());

        public static IJsonToken Serialize(IJsonDocumentObjectModelFactory domFactory, DateTime? item)
            => item.HasValue ? domFactory.CreateValue(item.Value.ToString("o")) : domFactory.CreateNull();

        public static IJsonToken Serialize(IJsonDocumentObjectModelFactory domFactory, string item)
            => domFactory.CreateValue(item);

        public static void ProcessExpression<TResult, T>(Expression<Getter<TResult, T>> property, out Getter<TResult, T> getter, out Setter<TResult, T> setter, ref string propertyName)
        {
            MemberExpression memberExpression = (MemberExpression)property.Body;
            ParameterExpression targetExpression = (ParameterExpression)memberExpression.Expression;
            getter = Expression.Lambda<Getter<TResult, T>>(memberExpression, targetExpression).Compile();

            ParameterExpression valueExpression = Expression.Parameter(typeof(T), "value");
            setter = Expression.Lambda<Setter<TResult, T>>(Expression.Assign(memberExpression, valueExpression), targetExpression, valueExpression).Compile();

            propertyName = propertyName ?? memberExpression.Member.Name;
        }
    }

    public interface IWrapper<T, TConcrete>
        where TConcrete : T
    {
        T Value { get; set; }

        IJsonToken Serialize(IJsonDocumentObjectModelFactory f, T o);

        TConcrete Deserialize(IJsonToken o);
    }

    public class Wrapper<T, TConcrete> : IWrapper<T, TConcrete>
        where TConcrete : T
    {
        private readonly IJsonBuilder<IWrapper<T, TConcrete>> _builder;

        private Wrapper()
        {
        }

        public Wrapper(Func<JsonBuilder<IWrapper<T, TConcrete>, Wrapper<T, TConcrete>>, JsonBuilder<IWrapper<T, TConcrete>, Wrapper<T, TConcrete>>> builderConfigurer)
        {
            _builder = builderConfigurer(new JsonBuilder<IWrapper<T, TConcrete>, Wrapper<T, TConcrete>>(() => new Wrapper<T, TConcrete>(builderConfigurer)));
        }

        public T Value { get; set; }

        public IJsonToken Serialize(IJsonDocumentObjectModelFactory f, T o)
        {
            Wrapper<T, TConcrete> wrapped = new Wrapper<T, TConcrete>() { Value = o };
            IJsonObject result = _builder.Serialize(f, wrapped);
            return result.Properties().FirstOrDefault().Value;
        }

        public TConcrete Deserialize(IJsonToken o)
        {
            IJsonObject temp = o.Factory.CreateObject();
            temp.SetValue("Value", o);
            IWrapper<T, TConcrete> wrapper = _builder.Deserialize(temp);
            return (TConcrete)wrapper.Value;
        }
    }

    public class Wrapper
    {
        public static IWrapper<T, TConcrete> For<T, TConcrete>(Func<JsonBuilder<IWrapper<T, TConcrete>, Wrapper<T, TConcrete>>, JsonBuilder<IWrapper<T, TConcrete>, Wrapper<T, TConcrete>>> builderConfigurer)
            where TConcrete : T
            => new Wrapper<T, TConcrete>(builderConfigurer);
    }

    public class JsonBuilder<TResult, TResultConcrete> : IJsonBuilder<TResult>
        where TResultConcrete : TResult
    {
        private readonly Func<TResult> _creator;
        private readonly Dictionary<string, Action<IJsonToken, TResult>> _deserializeSteps = new Dictionary<string, Action<IJsonToken, TResult>>(StringComparer.OrdinalIgnoreCase);
        private readonly List<Action<TResult, IJsonObject>> _serializeSteps = new List<Action<TResult, IJsonObject>>();

        public JsonBuilder(Func<TResult> creator)
        {
            _creator = creator;
        }

        public TResult Deserialize(IJsonToken source)
        {
            if (source.TokenType == JsonTokenType.Object)
            {
                TResult result = _creator();
                ((IJsonObject)source).ExtractValues(result, _deserializeSteps);
                return result;
            }

            return default(TResult);
        }

        public JsonBuilder<TResult, TResultConcrete> MapCore<T, TConcrete>(Getter<TResultConcrete, T> getter, Serialize<T> serialize, Deserialize<TConcrete> deserialize, Setter<TResultConcrete, T> setter, Func<TConcrete> itemCreator, string propertyName)
            where TConcrete : T
            => MapInternal(getter, serialize, t => deserialize(t, itemCreator), setter, propertyName);

        public JsonBuilder<TResult, TResultConcrete> MapCore<T, TConcrete>(Getter<TResultConcrete, T> getter, Serialize<T> serialize, DirectDeserialize<TConcrete> deserialize, Setter<TResultConcrete, T> setter, string propertyName)
            where TConcrete : T
            => MapInternal(getter, serialize, deserialize, setter, propertyName);

        public IJsonObject Serialize(IJsonDocumentObjectModelFactory domFactory, TResult item)
        {
            IJsonObject obj = domFactory.CreateObject();

            foreach (Action<TResult, IJsonObject> action in _serializeSteps)
            {
                action(item, obj);
            }

            return obj;
        }

        private JsonBuilder<TResult, TResultConcrete> MapInternal<T, TConcrete>(Getter<TResultConcrete, T> getter, Serialize<T> serialize, DirectDeserialize<TConcrete> deserialize, Setter<TResultConcrete, T> setter, string propertyName)
                    where TConcrete : T
        {
            _serializeSteps.Add((source, target) =>
            {
                T value = getter((TResultConcrete)source);
                IJsonToken token = serialize(target.Factory, value);
                target.SetValue(propertyName, token);
            });

            _deserializeSteps[propertyName] = (source, target) => setter((TResultConcrete)target, deserialize(source));

            return this;
        }
    }
}
