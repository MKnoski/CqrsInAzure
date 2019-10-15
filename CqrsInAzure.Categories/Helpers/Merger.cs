using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace CqrsInAzure.Candidates.Helpers
{
    public static class Merger
    {
        public static T Merge<T>(this T source, T original)
        {
            var result = original.DeepClone();

            typeof(T)
            .GetProperties()
            .Select((PropertyInfo x) => new KeyValuePair<PropertyInfo, object>(x, x.GetValue(source, null)))
            .Where((KeyValuePair<PropertyInfo, object> x) => x.Value != null && !x.Key.Name.Equals("id", StringComparison.InvariantCultureIgnoreCase)).ToList()
            .ForEach((KeyValuePair<PropertyInfo, object> x) => x.Key.SetValue(result, x.Value, null));

            return result;
        }

        public static T DeepClone<T>(this T a)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, a);
                stream.Position = 0;
                return (T)formatter.Deserialize(stream);
            }
        }
    }
}