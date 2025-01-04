using System;
using UnityEngine;

namespace Dman.SimpleJson
{
    public static class SimpleSave
    {
        public static string FullSaveFolderPath => JsonSaveSystemSettings.FullSaveFolderPath;
        public static string SaveFileName => Instance.SaveFileName;
        public static IPersistText FileSystem => Instance.FileSystem;
        
        private static SimpleSaveObject Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SimpleSaveObject(
                        JsonSaveSystemSettings.FullSaveFolderPath,
                        JsonSaveSystemSettings.DefaultSaveFileName);
                }

                return _instance;
            }
        }

        private static SimpleSaveObject _instance;

        #region Static Redirect methods
        
        /// <inheritdoc cref="SimpleSaveObject.Save"/> 
        public static void Save() => Instance.Save();

        /// <inheritdoc cref="SimpleSaveObject.Refresh"/> 
        public static void Refresh() => Instance.Refresh();

        /// <inheritdoc cref="SimpleSaveObject.ChangeSaveFile"/> 
        public static void ChangeSaveFile(string newSaveFileName) => Instance.ChangeSaveFile(newSaveFileName);
        
        /// <inheritdoc cref="SimpleSaveObject.Get{T}"/> 
        public static T Get<T>(string key, T defaultValue = default, TokenMode mode = TokenMode.SerializableObject) 
            => Instance.Get(key, defaultValue, mode);
        
        /// <inheritdoc cref="SimpleSaveObject.Set{T}"/> 
        public static void Set<T>(string key, T value, TokenMode mode = TokenMode.SerializableObject) 
            => Instance.Set(key, value, mode);

        public static bool HasKey(string key) => Instance.HasKey(key);
        public static void DeleteKey(string key) => Instance.DeleteKey(key);
        public static void DeleteAll() => Instance.DeleteAll();
        
        #endregion
        
        /// <summary>
        /// Same as ChangeSaveFile, but sets to the default save file name.
        /// </summary>
        /// <remarks>
        /// Causes synchronous disk access if save file is different.
        /// </remarks>
        public static void ChangeSaveFileToDefault() => ChangeSaveFile(JsonSaveSystemSettings.DefaultSaveFileName);
        
        public static string GetString(string key, string defaultValue = "") => Get(key, defaultValue, TokenMode.Primitive);
        public static void SetString(string key, string value) => Set(key, value, TokenMode.Primitive);

        public static int GetInt(string key, int defaultValue = 0) => Get(key, defaultValue, TokenMode.Primitive);
        public static void SetInt(string key, int value) => Set(key, value, TokenMode.Primitive);
        
        public static float GetFloat(string key, float defaultValue = 0) => Get(key, defaultValue, TokenMode.Primitive);
        public static void SetFloat(string key, float value) => Set(key, value, TokenMode.Primitive);
        
        public static bool GetBool(string key, bool defaultValue = false) => Get(key, defaultValue, TokenMode.Primitive);
        public static void SetBool(string key, bool value) => Set(key, value, TokenMode.Primitive);
        
        public static T GetEnum<T>(string key, T defaultValue = default) where T : Enum => Get(key, defaultValue, TokenMode.Primitive);
        public static void SetEnum<T>(string key, T value) where T : Enum => Set(key, value, TokenMode.Primitive);

        
        [RuntimeInitializeOnLoadMethod]
        private static void RunOnStart()
        {
            Application.quitting += OnApplicationQuit;
        }

        private static void OnApplicationQuit()
        {
            Save();
        }

        // below are methods for testing purposes only
        internal static void EmulateForcedQuit()
        {
            _instance = null;
        }

        internal static void EmulateManagedApplicationQuit()
        {
            Save();
            _instance = null;
        }
    }
}
