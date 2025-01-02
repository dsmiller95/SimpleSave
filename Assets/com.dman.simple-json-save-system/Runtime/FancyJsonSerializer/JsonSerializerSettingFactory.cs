using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Dman.SimpleJson.FancyJsonSerializer
{
    public enum JsonSerializationType
    {
        UnityPlusPlus
    }
    
    public static class JsonSerializerSettingFactory
    {
        [CanBeNull]
        public static JsonSerializerSettings GetSettings(JsonSerializationType serializationType)
        {
            switch (serializationType)
            {
                case JsonSerializationType.UnityPlusPlus:
                    return GetUnityPlusPlusSerializerSettings();
                default:
                    return null;
            }
        }
        
        private static JsonSerializerSettings GetUnityPlusPlusSerializerSettings()
        {
            return new JsonSerializerSettings
            {
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                ContractResolver = new UnitySerializationCompatibleContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy
                    {
                        OverrideSpecifiedNames = false
                    },
                    IgnoreSerializableAttribute = false,
                },
                ReferenceLoopHandling = ReferenceLoopHandling.Error,
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.Auto,
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
                Converters = new List<JsonConverter>
                {
                    new StringEnumConverter(),
                    new Vector3IntConverter(),
                    new Vector2IntConverter(),
                    new UnityJsonUtilityJsonConverter(),
                },
                MissingMemberHandling = MissingMemberHandling.Error,
            };
        }
    }
}