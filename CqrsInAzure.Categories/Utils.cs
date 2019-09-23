using System.IO;
using Newtonsoft.Json;

namespace CqrsInAzure.Categories
{
    public static class Utils
    {
        public static void SerializeToJsonStream(MemoryStream ms, object obj)
        {
            var json = JsonConvert.SerializeObject(obj);
            StreamWriter writer = new StreamWriter(ms);
            writer.Write(json);
            writer.Flush();
            ms.Position = 0;
        }
    }
}
