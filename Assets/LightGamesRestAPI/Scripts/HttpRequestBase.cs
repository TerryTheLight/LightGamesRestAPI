namespace LightGames.RestAPI
{
    using Newtonsoft.Json;
    using UnityEngine;

    public class HttpRequestBase
    {
        private readonly JsonSerializerSettings _defaultJsonSerializerSettings = new()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        public string AsJson(Formatting formatting = Formatting.Indented, JsonSerializerSettings serializeSettings = null)
        {
            return JsonConvert.SerializeObject(this, formatting, serializeSettings ?? _defaultJsonSerializerSettings);
        }
    }

}

