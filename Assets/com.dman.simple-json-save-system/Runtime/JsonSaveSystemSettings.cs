using System.Collections.Generic;
using Dman.SimpleJson.FancyJsonSerializer;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using UnityEditor;
using UnityEngine;

namespace Dman.SimpleJson
{
    public enum JsonSerializationType
    {
        // TODO: fully regular Unity, somehow
        UnityPlusPlus
    }
    
    public class JsonSaveSystemSettings : ScriptableObject
    {
        public static JsonSaveSystemSettings Singleton => _singleton ??= GetSingleton();
        private static JsonSaveSystemSettings _singleton;
        
        public static string SaveFolderName => Singleton.saveFolderName;
        public static string DefaultSaveFileName => Singleton.defaultSaveFileName;
        public static JsonSerializer Serializer => _defaultSerializer ??= Singleton.GetSerializer();
        private static JsonSerializer _defaultSerializer;
        
        [Header("All values are read on first use of save system. Changes during runtime are ignored.")]
        [SerializeField] private string saveFolderName = "SaveContexts";
        [SerializeField] private string defaultSaveFileName = "root";
        [SerializeField] private JsonSerializationType serializationType = JsonSerializationType.UnityPlusPlus;

        public static JsonSaveSystemSettings Create(string folder, string defaultFile)
        {
            var newSettings = CreateInstance<JsonSaveSystemSettings>();
            newSettings.saveFolderName = folder;
            newSettings.defaultSaveFileName = defaultFile;
            return newSettings;
        }
        
#if UNITY_EDITOR
        [MenuItem("SaveSystem/Create Json Save System Settings")]
        private static void CreateSettingsObject()
        {
            var newSettings = CreateInstance<JsonSaveSystemSettings>();
            if(!AssetDatabase.IsValidFolder("Assets/Resources"))
            {
                AssetDatabase.CreateFolder("Assets", "Resources");
            }
            AssetDatabase.CreateAsset(newSettings, "Assets/Resources/JsonSaveSystemSettings.asset");
            AssetDatabase.SaveAssets();
        }
#endif
        
        public static void ForceOverrideSettingsObject(JsonSaveSystemSettings settings, bool suppressWarningDangerously = false)
        {
            if(!suppressWarningDangerously && _singleton != null)
            {
                Debug.LogWarning("Forcing override of JsonSaveSystemSettings object after it has already been used. " +
                                 "This is dangerous and should only be done during startup.");
            }
            _singleton = settings;
            _defaultSerializer = null;
        }
        
        public static JsonSaveSystemSettings GetSingleton()
        {
            var settingsList = Resources.LoadAll<JsonSaveSystemSettings>("JsonSaveSystemSettings");
            if(settingsList.Length == 0)
            {
                var newSettings = CreateInstance<JsonSaveSystemSettings>();
                return newSettings;
            }
            if (settingsList.Length != 1)
            {
                Debug.LogWarning("The number of JsonSaveSystemSettings objects should be 1 or less: " + settingsList.Length);
            }
            return settingsList[0];
        }

        private JsonSerializer GetSerializer()
        {
            switch (serializationType)
            {
                case JsonSerializationType.UnityPlusPlus:
                    return JsonSerializer.CreateDefault(GetUnityPlusPlusSerializerSettings());
                default:
                    return JsonSerializer.CreateDefault();
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