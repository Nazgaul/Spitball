using Newtonsoft.Json;

namespace Cloudents.Functions
{
    public static class JsonConvertInheritance
    {
        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };

        public static string SerializeObject(object value)
        {
            return JsonConvert.SerializeObject(value, Settings);
        }
        public static T DeserializeObject<T>(string value)
        {
            return JsonConvert.DeserializeObject<T>(value, Settings);
        }
    }
}