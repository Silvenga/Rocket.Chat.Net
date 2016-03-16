namespace Rocket.Chat.Net.Helpers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public static class ObjectExtensions
    {
        // http://stackoverflow.com/questions/4943817/mapping-object-to-dictionary-and-vice-versa/4944547
        // https://jacobcarpenter.wordpress.com/2008/03/13/dictionary-to-anonymous-type/

        // ReSharper disable once MemberCanBePrivate.Global
        public const BindingFlags DefaultBindingFlags =
            BindingFlags.DeclaredOnly
            | BindingFlags.Public
            | BindingFlags.Instance;

        public static T ToObject<T>(this IDictionary<string, object> source)
            where T : class, new()
        {
            var obj = new T();
            var objType = obj.GetType();

            foreach (var item in source)
            {
                objType
                    .GetProperty(item.Key)
                    .SetValue(obj, item.Value, null);
            }

            return obj;
        }

        public static T ToAnonymousObject<T, TValue>(this IDictionary<string, TValue> dictionary, T anonymousPrototype)
        {
            // get the sole constructor
            var ctor = anonymousPrototype
                .GetType()
                .GetConstructors()
                .Single();

            // conveniently named constructor parameters make this all possible...
            var args = (from p in ctor.GetParameters()
                        let val = dictionary.GetValueOrDefault(p.Name)
                        select val != null && p.ParameterType.IsInstanceOfType(val) ? (object) val : null)
                .ToArray();

            return (T) ctor.Invoke(args.ToArray());
        }

        public static IDictionary<string, object> AsDictionary(this object source,
                                                               BindingFlags bindingAttr = DefaultBindingFlags)
        {
            return source
                .GetType()
                .GetProperties(bindingAttr)
                .ToDictionary
                (
                    propInfo => propInfo.Name,
                    propInfo => propInfo.GetValue(source, null)
                );
        }

        private static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key)
        {
            TValue result;
            dict.TryGetValue(key, out result);
            return result;
        }
    }
}