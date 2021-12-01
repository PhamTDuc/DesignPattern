using System;
using Newtonsoft.Json;

namespace Guinea.Core.DataHandler
{
    public static class JsonHandler
    {
        private static readonly bool debug = false;
#if !ENABLE_IL2CPP
        public static dynamic Deserialize(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                throw new ArgumentException("Json parameter could not be Null or Empty");
            }
            dynamic data;
            try
            {
                data = JsonConvert.DeserializeObject<dynamic>(json);
                Commons.Logger.LogIf(debug, $"Deserialize: {data}");
                return data;
            }
            catch (Exception ex)
            {
                Commons.Logger.LogIf(debug, $"Deserialize: {ex.Message.ToString()}");
            }
            return default(dynamic);
        }
#endif

        public static T Deserialize<T>(string json, JsonSerializerSettings setting = null)
        {
            if (string.IsNullOrEmpty(json))
            {
                throw new ArgumentException("Json parameter could not be Null or Empty");
            }
            T data;
            try
            {
                data = JsonConvert.DeserializeObject<T>(json, setting);
                Commons.Logger.LogIf(debug, $"Deserialize: {data}");
                return data;
            }
            catch (Exception ex)
            {
                throw new Exception($"Deserialize Failed: {ex.Message.ToString()}. \n\"{json}\" \nto type <{typeof(T)}>!!");
                // return default(T);
            }
        }

        public static string SerializeObject(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        // TODO: Implement Asynchronous Serialize and Deserialize
    }
}