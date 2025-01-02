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
        
        /// <summary>
        /// A serialization type based on unity which attempts to handle some of the common native types. Not perfect.
        /// </summary>
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