using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace Dman.SimpleJson
{
    public static class SimpleSave
    {
        public static string FullSaveFolderPath => JsonSaveSystemSettings.FullSaveFolderPath;
        public static string SaveFileName
        {
            get => I.SaveFileName;
            private set => I.SaveFileName = value;
        }
        public static IPersistText FileSystem => I.FileSystem;
        
        private static SimpleSaveProperties I => _properties ??= new SimpleSaveProperties(
            JsonSaveSystemSettings.FullSaveFolderPath, 
            JsonSaveSystemSettings.DefaultSaveFileName);
        private static SimpleSaveProperties _properties;
        
        /// <summary>
        /// Data class used to wrap up initialization logic for SimpleSave.
        /// </summary>
        [SuppressMessage("ReSharper", "MemberHidesStaticFromOuterClass")]
        private class SimpleSaveProperties
        {
            public string SaveFileName;
            [JetBrains.Annotations.NotNull]
            public SaveData CurrentSaveData;
            public readonly IPersistText FileSystem;
            public SimpleSaveProperties(string absoluteSaveFolderPath, string saveFileName)
            {
                SaveFileName = saveFileName;
                FileSystem = FileSystemPersistence.CreateAtAbsoluteFolderPath(absoluteSaveFolderPath);
                CurrentSaveData = FileSystem.LoadSave(saveFileName) ?? SaveData.Empty();
            }
        }

        /// <summary>
        /// Save the current file to disk synchronously.
        /// </summary>
        public static void Save()
        {
            FileSystem.PersistSave(SaveFileName, I.CurrentSaveData);
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
            var loadedData = FileSystem.LoadSave(SaveFileName);
            if(loadedData != null)
            {
                I.CurrentSaveData = loadedData;
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
        
        public static string GetString(string key, string defaultValue = "") => Get(key, defaultValue, TokenMode.Primitive);
        public static void SetString(string key, string value) => Set(key, value, TokenMode.Primitive);

        public static int GetInt(string key, int defaultValue = 0) => Get(key, defaultValue, TokenMode.Primitive);
        public static void SetInt(string key, int value) => Set(key, value, TokenMode.Primitive);
        
        public static float GetFloat(string key, float defaultValue = 0) => Get(key, defaultValue, TokenMode.Primitive);
        public static void SetFloat(string key, float value) => Set(key, value, TokenMode.Primitive);
        
        public static T GetEnum<T>(string key, T defaultValue = default) where T : Enum => Get(key, defaultValue, TokenMode.Primitive);
        public static void SetEnum<T>(string key, T value) where T : Enum => Set(key, value, TokenMode.Primitive);
        
        /// <summary>
        /// Get generic data.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <param name="mode">Configures whether to use primitive or object style serialization.</param>
        /// <returns>
        /// Data from the shared store, or <paramref name="defaultValue"/> if the data at <paramref name="key"/>
        /// is not present or not deserializable into <typeparamref name="T"/>
        /// </returns>
        public static T Get<T>(string key, T defaultValue = default, TokenMode mode = TokenMode.SerializableObject)
        {
            if (I.CurrentSaveData.TryGet(key, out T value, mode))
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
        public static void Set<T>(string key, T value, TokenMode mode = TokenMode.SerializableObject)
        {
            I.CurrentSaveData.Set(key, value, mode);
        }

        public static bool HasKey(string key)
        {
            return I.CurrentSaveData.HasKey(key);
        }

        public static void DeleteKey(string key)
        {
            I.CurrentSaveData.DeleteKey(key);
        }

        public static void DeleteAll()
        {
            I.CurrentSaveData = SaveData.Empty();
            FileSystem.Delete(SaveFileName);
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
            _properties = null;
        }

        internal static void EmulateManagedApplicationQuit()
        {
            Save();
            _properties = null;
        }
    }
}
