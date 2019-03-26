//using System;
//using System.Collections.Generic;
//using System.Runtime.CompilerServices;
//using Microsoft.TemplateEngine.Abstractions.Json;

//namespace Microsoft.TemplateEngine.Utils.Json
//{
//    public static class JsonExtensions
//    {
//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        public static IJsonArray AddNull(this IJsonArray array) => array.Add(array.Factory.CreateNull());

//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        public static IJsonArray Add(this IJsonArray array, string value) => array.Add(array.Factory.CreateValue(value));

//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        public static IJsonArray Add(this IJsonArray array, int value) => array.Add(array.Factory.CreateValue(value));

//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        public static IJsonArray Add(this IJsonArray array, double value) => array.Add(array.Factory.CreateValue(value));

//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        public static IJsonArray Add(this IJsonArray array, Guid value) => array.Add(array.Factory.CreateValue(value));

//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        public static IJsonArray Add(this IJsonArray array, bool value) => array.Add(array.Factory.CreateValue(value));

//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        public static IJsonObject SetValueNull(this IJsonObject obj, string propertyName) => obj.SetValue(propertyName, obj.Factory.CreateNull());

//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        public static IJsonObject SetValue(this IJsonObject obj, string propertyName, Guid guid) => obj.SetValue(propertyName, obj.Factory.CreateValue(guid.ToString()));

//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        public static IJsonObject SetValue(this IJsonObject obj, string propertyName, string value) => obj.SetValue(propertyName, obj.Factory.CreateValue(value));

//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        public static IJsonObject SetValue(this IJsonObject obj, string propertyName, int value) => obj.SetValue(propertyName, obj.Factory.CreateValue(value));

//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        public static IJsonObject SetValue(this IJsonObject obj, string propertyName, double value) => obj.SetValue(propertyName, obj.Factory.CreateValue(value));

//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        public static IJsonObject SetValue(this IJsonObject obj, string propertyName, bool value) => obj.SetValue(propertyName, obj.Factory.CreateValue(value));

//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        public static IJsonObject SetValue(this IJsonObject obj, string propertyName, IEnumerable<string> value)
//        {
//            IJsonArray array = obj.Factory.CreateArray();

//            foreach (string v in value)
//            {
//                array.Add(v);
//            }

//            obj.SetValue(propertyName, array);
//            return obj;
//        }

//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        public static IJsonObject SetValue(this IJsonObject obj, string propertyName, IEnumerable<double> value)
//        {
//            IJsonArray array = obj.Factory.CreateArray();

//            foreach (double v in value)
//            {
//                array.Add(v);
//            }

//            obj.SetValue(propertyName, array);
//            return obj;
//        }

//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        public static IJsonObject SetValue(this IJsonObject obj, string propertyName, IEnumerable<Guid> value)
//        {
//            IJsonArray array = obj.Factory.CreateArray();

//            foreach (Guid v in value)
//            {
//                array.Add(v);
//            }

//            obj.SetValue(propertyName, array);
//            return obj;
//        }

//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        public static IJsonObject SetValue(this IJsonObject obj, string propertyName, IEnumerable<int> value)
//        {
//            IJsonArray array = obj.Factory.CreateArray();

//            foreach (int v in value)
//            {
//                array.Add(v);
//            }

//            obj.SetValue(propertyName, array);
//            return obj;
//        }

//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        public static IJsonObject SetValue(this IJsonObject obj, string propertyName, IEnumerable<bool> value)
//        {
//            IJsonArray array = obj.Factory.CreateArray();

//            foreach (bool v in value)
//            {
//                array.Add(v);
//            }

//            obj.SetValue(propertyName, array);
//            return obj;
//        }

//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        public static IJsonObject SetValue(this IJsonObject obj, string propertyName, IEnumerable<IJsonToken> value)
//        {
//            IJsonArray array = obj.Factory.CreateArray();

//            foreach (IJsonToken v in value)
//            {
//                array.Add(v);
//            }

//            obj.SetValue(propertyName, array);
//            return obj;
//        }

//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        public static IJsonValue CreateValue(this IJsonDocumentObjectModelFactory domFactory, Guid g) => domFactory.CreateValue(g.ToString());
//    }
//}
