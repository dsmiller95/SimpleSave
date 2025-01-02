using Dman.Utilities.Logger;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Dman.SimpleJson
{
    /// <summary>
    /// Wrapper to provide a simple interface for saving and loading SaveData objects.
    /// </summary>
    public class PersistSaves
    {
        public readonly IPersistText TextPersistence;
        private readonly JsonSerializer _serializer;

        private PersistSaves(IPersistText textPersistence, JsonSerializer serializer)
        {
            TextPersistence = textPersistence;
            _serializer = serializer;
        }
        
        public static PersistSaves CreateDisk(string rootFolder, JsonSerializer serializerOverride = null)
        {
            var serializer = serializerOverride ?? JsonSaveSystemSettings.Serializer;
            var textPersistence = new FileSystemPersistence(rootFolder);
            return new PersistSaves(textPersistence, serializer);
        }
        
        public static PersistSaves Create(IPersistText textPersistence, JsonSerializer serializerOverride = null)
        {
            var serializer = serializerOverride ?? JsonSaveSystemSettings.Serializer;
            return new PersistSaves(textPersistence, serializer);
        }
        
        public void PersistSave(SaveData saveData, string file)
        {
            using var writer = TextPersistence.WriteTo(file);
            using var jsonWriter = new JsonTextWriter(writer);
            _serializer.Serialize(jsonWriter, saveData.SavedToken);
        }
        
        [CanBeNull]
        public SaveData LoadSave(string file)
        {
            using var reader = TextPersistence.ReadFrom(file);
            if (reader == null) return null;
            using var jsonReader = new JsonTextReader(reader);
            
            try
            {
                var data = JObject.Load(jsonReader);
                return SaveData.Loaded(data, _serializer);
            }
            catch (JsonException e)
            {
                Log.Error($"Failed to load data for {file}.json, malformed Json. Raw json: {reader.ReadToEnd()}");
                Debug.LogException(e);
                return null;
            }
        }

        public SaveData CreateEmptySave()
        {
            return SaveData.Empty(_serializer);
        }
    }
}