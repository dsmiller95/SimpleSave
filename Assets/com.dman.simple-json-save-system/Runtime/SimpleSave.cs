using JetBrains.Annotations;
using UnityEngine;

namespace Dman.SimpleJson
{
    public static class SimpleSave
    {
        public static string FullSaveFolderPath => JsonSaveSystemSettings.FullSaveFolderPath;
        public static string SaveFileName
        {
            get => _cachedSaveFileName ??= JsonSaveSystemSettings.DefaultSaveFileName;
            private set => _cachedSaveFileName = value;
        }
        private static string _cachedSaveFileName;
        
        public static PersistSaves Saves => _saves ??= PersistSaves.CreateDisk(FullSaveFolderPath, JsonSaveSystemSettings.Serializer);
        private static PersistSaves _saves;
        
        [NotNull]
        private static SaveData CurrentSaveData
        {
            get
            {
                if (_currentSaveData == null)
                {
                    _currentSaveData = Saves.LoadSave(SaveFileName);
                    // if Load did not load any data, then create empty data
                    _currentSaveData ??= Saves.CreateEmptySave();
                }
                return _currentSaveData;
            }
            set => _currentSaveData = value;
        }
        private static SaveData _currentSaveData;

        /// <summary>
        /// Save the current file to disk synchronously.
        /// </summary>
        public static void Save()
        {
            Saves.PersistSave(CurrentSaveData, SaveFileName);
        }
        
        /// <summary>
        /// Load the current file from disk synchronously, overwriting any unsaved changes in memory.
        /// </summary>
        /// <remarks>
        /// Loading happens automatically on first access. ForceLoad is required when loading changes made
        /// to the file outside the SaveSystem apis during runtime. For example, edits in a text editor
        /// or modifications made by other applications.
        /// </remarks>
        public static void Refresh()
        {
            var loadedData = Saves.LoadSave(SaveFileName);
            if(loadedData != null)
            {
                CurrentSaveData = loadedData;
            }
        }
        
        /// <summary>
        /// Change the save file currently written to. This will save the current file before switching, if different. 
        /// </summary>
        /// <remarks>
        /// Causes synchronous disk access if save file is different.
        /// </remarks>
        public static void ChangeSaveFile(string newSaveFileName)
        {
            if (newSaveFileName == SaveFileName) return;
            
            // save the old file before switching
            Save();
            SaveFileName = newSaveFileName;
            // load the new save file after switching
            Refresh();
        }
        
        /// <summary>
        /// Same as ChangeSaveFile, but sets to the default save file name.
        /// </summary>
        /// <remarks>
        /// Causes synchronous disk access if save file is different.
        /// </remarks>
        public static void ChangeSaveFileToDefault() => ChangeSaveFile(JsonSaveSystemSettings.DefaultSaveFileName);
        
        public static string GetString(string key, string defaultValue = "") => Get(key, defaultValue, TokenMode.Newtonsoft);
        public static void SetString(string key, string value) => Set(key, value, TokenMode.Newtonsoft);

        public static int GetInt(string key, int defaultValue = 0) => Get(key, defaultValue, TokenMode.Newtonsoft);
        public static void SetInt(string key, int value) => Set(key, value, TokenMode.Newtonsoft);
        
        public static float GetFloat(string key, float defaultValue = 0) => Get(key, defaultValue, TokenMode.Newtonsoft);
        public static void SetFloat(string key, float value) => Set(key, value, TokenMode.Newtonsoft);

        /// <summary>
        /// Get generic data. Supports JsonUtility style serializable types, or optionally newtonsoft style serialization.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <param name="mode">Configures how the type is converted from JSON.</param>
        /// <returns>
        /// Data from the shared store, or <paramref name="defaultValue"/> if the data at <paramref name="key"/>
        /// is not present or not deserializable into <typeparamref name="T"/>
        /// </returns>
        public static T Get<T>(string key, T defaultValue = default, TokenMode mode = TokenMode.UnityJson)
        {
            if (CurrentSaveData.TryGet(key, out T value, mode))
            {
                return value;
            }
            
            return defaultValue;
        }
        /// <summary>
        /// Set generic data. Supports JsonUtility style serializable types.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="mode">Configures how the type is converted to JSON.</param>
        public static void Set<T>(string key, T value, TokenMode mode = TokenMode.UnityJson)
        {
            CurrentSaveData.Set(key, value, mode);
        }

        public static bool HasKey(string key)
        {
            return CurrentSaveData.HasKey(key);
        }

        public static void DeleteKey(string key)
        {
            CurrentSaveData.DeleteKey(key);
        }

        public static void DeleteAll()
        {
            CurrentSaveData = Saves.CreateEmptySave();
            Saves.TextPersistence.Delete(SaveFileName);
        }

        
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
            _currentSaveData = null;
        }

        internal static void EmulateManagedApplicationQuit()
        {
            Save();
            _currentSaveData = null;
        }
    }
}
