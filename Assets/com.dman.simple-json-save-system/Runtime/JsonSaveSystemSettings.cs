using System.IO;
using Dman.SimpleJson.FancyJsonSerializer;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace Dman.SimpleJson
{
    public class JsonSaveSystemSettings : ScriptableObject
    {
        /// <summary>
        /// The save folder under which all files are saved
        /// </summary>
        public static string FullSaveFolderPath => Path.Combine(Application.persistentDataPath, Singleton.saveFolderName);
        public static string DefaultSaveFileName => Singleton.defaultSaveFileName;
        
        [Header("All values are read on first use of save system. Changes during runtime are ignored.")]
        [SerializeField] protected string saveFolderName = "SaveContexts";
        [SerializeField] protected string defaultSaveFileName = "root";
        
        public static JsonSerializer Serializer => _defaultSerializer ??= Singleton.CreateSerializer();
        private static JsonSerializer _defaultSerializer;
        private static JsonSaveSystemSettings Singleton => _singleton ??= GetSingleton();
        private static JsonSaveSystemSettings _singleton;
        
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
                                 "This may lead to save file inconsistencies and should only be done during early startup.");
            }
            _singleton = settings;
            _defaultSerializer = null;
        }

        private static JsonSaveSystemSettings GetSingleton()
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

        protected virtual JsonSerializer CreateSerializer()
        {
            var settings = JsonSerializerSettingFactory.Create();
            return JsonSerializer.CreateDefault(settings);
        }
    }
}