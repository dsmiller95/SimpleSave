using JetBrains.Annotations;
using UnityEngine;

namespace Dman.SimpleJson
{
    public static class SimpleSave
    {
        public static string SaveFileName
        {
            get => _cachedSaveFileName ??= JsonSaveSystemSettings.DefaultSaveFileName;
            private set => _cachedSaveFileName = value;
        }
        private static string _cachedSaveFileName;

        public static string SaveFolderName => JsonSaveSystemSettings.SaveFolderName;
        
        [NotNull]
        private static SimpleSaveFile CurrentSaveData
        {
            get
            {
                if (_currentSaveData == null)
                {
                    string file = SaveFileName;
                    _currentSaveData = Saves.LoadSave(file);
                    // if Load did not load any data, then create empty data
                    _currentSaveData ??= Saves.CreateEmptySave();
                }
                return _currentSaveData;
            }
            set => _currentSaveData = value;
        }
        private static SimpleSaveFile _currentSaveData;

        public static PersistSaves Saves => _saves ??= PersistSaves.CreateDisk(SaveFolderName, JsonSaveSystemSettings.Serializer);
        private static PersistSaves _saves;

        /// <summary>
        /// Save the current file to disk.
        /// </summary>
        public static void Save()
        {
            SimpleSaveFile data = CurrentSaveData;
            string file = SaveFileName;
            Saves.PersistFile(data, file);
        }
        
        /// <summary>
        /// Load the current file from disk, overwriting any unsaved changes in memory.
        /// </summary>
        /// <remarks>
        /// Loading happens automatically on first access. ForceLoad is required when loading changes made
        /// to the file outside the SaveSystem apis during runtime. For example, edits in a text editor
        /// or modifications made by other applications.
        /// </remarks>
        public static void Refresh()
        {
            string file = SaveFileName;
            var loadedData = Saves.LoadSave(file);
            if(loadedData != null)
            {
                CurrentSaveData = loadedData;
            }
        }
        
        /// <summary>
        /// Change the save file currently written to. This will save the current file before switching, if different. 
        /// </summary>
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
        public static void ChangeSaveFileToDefault() => ChangeSaveFile(JsonSaveSystemSettings.DefaultSaveFileName);
        
        public static string GetString(string key, string defaultValue = "") => Get(key, defaultValue, TokenMode.Newtonsoft);
        public static void SetString(string key, string value) => Set(key, value, TokenMode.Newtonsoft);

        public static int GetInt(string key, int defaultValue = 0) => Get(key, defaultValue, TokenMode.Newtonsoft);
        public static void SetInt(string key, int value) => Set(key, value, TokenMode.Newtonsoft);
        
        public static float GetFloat(string key, float defaultValue = 0) => Get(key, defaultValue, TokenMode.Newtonsoft);
        public static void SetFloat(string key, float value) => Set(key, value, TokenMode.Newtonsoft);
        
        /// <summary>
        /// Get generic data. Supports JsonUtility style serializable types.
        /// </summary>
        /// <returns>
        /// Data from the shared store, or <paramref name="defaultValue"/> if the data at <paramref name="key"/>
        /// is not present or not deserializable into <typeparamref name="T"/>
        /// </returns>
        public static T Get<T>(string key, T defaultValue = default, TokenMode mode = TokenMode.UnityJson)
        {
            if(!CurrentSaveData.TryLoad(key, out T value, mode)) return defaultValue;

            return value;
        }
        /// <summary>
        /// Set generic data. Supports JsonUtility style serializable types.
        /// </summary>
        public static void Set<T>(string key, T value, TokenMode mode = TokenMode.UnityJson)
        {
            CurrentSaveData.Save(key, value, mode);
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
            string file = SaveFileName;
            Saves.TextPersistence.Delete(file);
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
