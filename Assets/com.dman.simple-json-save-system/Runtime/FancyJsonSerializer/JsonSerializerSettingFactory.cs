using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Dman.SimpleJson.FancyJsonSerializer
{
    public static class JsonSerializerSettingFactory
    {
        [CanBeNull]
        public static JsonSerializerSettings Create()
        {
            return GetUnityPlusPlusSerializerSettings();
        }
        
        private static JsonSerializerSettings GetUnityPlusPlusSerializerSettings()
        {
            return new JsonSerializerSettings
            {
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented,
                Converters = new List<JsonConverter>
                {
                    new StringEnumConverter(),
                },
            };
        }
    }
}