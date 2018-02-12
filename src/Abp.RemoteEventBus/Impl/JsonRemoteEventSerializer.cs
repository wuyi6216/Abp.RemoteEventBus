using Abp.Dependency;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Abp.RemoteEventBus.Impl
{
    public class JsonRemoteEventSerializer : IRemoteEventSerializer,ISingletonDependency
    {
        private readonly JsonSerializerSettings settings;

        public JsonRemoteEventSerializer()
        {
            settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
            };
        }

        public T Deserialize<T>(string value)
        {
            return JsonConvert.DeserializeObject<T>(value, settings);
        }

        public string Serialize(object value)
        {
            return JsonConvert.SerializeObject(value, settings);
        }
    }
}