using System.Diagnostics.CodeAnalysis;

namespace Dman.SimpleJson
{
    /// <summary>
    /// Implementation behind the SimpleSave static API
    /// </summary>
    public class SimpleSaveObject
    {
        public string SaveFileName { get; private set; }
        public readonly IPersistText FileSystem;
        [NotNull] 
        private SaveData _currentSaveData;
        public SimpleSaveObject(string absoluteSaveFolderPath, string saveFileName)
        {
            SaveFileName = saveFileName;
            FileSystem = FileSystemPersistence.CreateAtAbsoluteFolderPath(absoluteSaveFolderPath);
            _currentSaveData = FileSystem.LoadSaveFrom(saveFileName) ?? SaveData.Empty();
        }
        
        /// <summary>
        /// Save the current file to disk synchronously.
        /// </summary>
        public void Save()
        {
            FileSystem.PersistSaveTo(SaveFileName, _currentSaveData);
        }
        
        /// <summary>
        /// Load the current file from disk synchronously, overwriting any unsaved changes in memory.
        /// </summary>
        /// <remarks>
        /// Loading happens automatically on first access. ForceLoad is required when loading changes made
        /// to the file outside the SaveSystem apis during runtime. For example, edits in a text editor
        /// or modifications made by other applications.
        /// </remarks>
        public void Refresh()
        {
            var loadedData = FileSystem.LoadSaveFrom(SaveFileName);
            if (loadedData != null)
            {
                _currentSaveData = loadedData;
            }
        }
        
        /// <summary>
        /// Change the save file currently written to. This will save the current file before switching, if different. 
        /// </summary>
        /// <remarks>
        /// Causes synchronous disk access if save file is different.
        /// </remarks>
        public void ChangeSaveFile(string newSaveFileName)
        {
            if (newSaveFileName == SaveFileName) return;
            
            // save the old file before switching
            Save();
            SaveFileName = newSaveFileName;
            // load the new save file after switching
            Refresh();
        }
        
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
        public T Get<T>(string key, T defaultValue, TokenMode mode)
        {
            if (_currentSaveData.TryGet(key, out T value, mode))
            {
                return value;
            }
            
            return defaultValue;
        }
        
        /// <summary>
        /// Set generic data.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="mode">Configures how the type is converted to JSON.</param>
        public void Set<T>(string key, T value, TokenMode mode)
        {
            _currentSaveData.Set(key, value, mode);
        }
        
        public bool HasKey(string key) => _currentSaveData.HasKey(key);
        public bool DeleteKey(string key) => _currentSaveData.DeleteKey(key);
        public void DeleteAll()
        {
            _currentSaveData = SaveData.Empty();
            FileSystem.Delete(SaveFileName);
        }
    }
}