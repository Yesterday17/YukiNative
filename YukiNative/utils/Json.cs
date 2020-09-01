using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace YukiNative.utils {
  public static class Json {
    private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings {
      ContractResolver = new DefaultContractResolver {
        NamingStrategy = new CamelCaseNamingStrategy(true, true, true),
      }
    };

    public static string Serialize(object obj) {
      return JsonConvert.SerializeObject(obj, Settings);
    }
  }
}