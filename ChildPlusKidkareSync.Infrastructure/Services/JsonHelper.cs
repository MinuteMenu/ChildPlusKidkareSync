using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ChildPlusKidkareSync.Infrastructure.Services;

public static class JsonHelper
{
    public static readonly JsonSerializerSettings DefaultJsonSettings = new()
    {
        ContractResolver = new CamelCasePropertyNamesContractResolver(),
        NullValueHandling = NullValueHandling.Ignore,
        //DateFormatString = "yyyy-MM-ddTHH:mm:ss",
        Formatting = Formatting.None
    };

    public static object PreparePayload(object payload)
    {
        if (payload == null) return null;

        var json = JsonConvert.SerializeObject(payload, DefaultJsonSettings);
        return JsonConvert.DeserializeObject<object>(json);
    }
}