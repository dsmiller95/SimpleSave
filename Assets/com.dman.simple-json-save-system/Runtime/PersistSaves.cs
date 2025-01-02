using Dman.Utilities.Logger;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Dman.SimpleJson
{
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
        
        public void PersistFile(SimpleSaveFile saveFile, string file)
        {
            using var writer = TextPersistence.WriteTo(file);
            using var jsonWriter = new JsonTextWriter(writer);
            _serializer.Serialize(jsonWriter, saveFile.SavedToken);
        }
        
        [CanBeNull]
        public SimpleSaveFile LoadSave(string file)
        {
            using var reader = TextPersistence.ReadFrom(file);
            if (reader == null) return null;
            using var jsonReader = new JsonTextReader(reader);
            
            try
            {
                var data = JObject.Load(jsonReader);
                return SimpleSaveFile.Loaded(data, _serializer);
            }
            catch (JsonException e)
            {
                using var reader2 = TextPersistence.ReadFrom(file);
                Log.Error($"Failed to load data for {file}.json, malformed Json. Raw json: {reader2?.ReadToEnd()}");
                Debug.LogException(e);
                return null;
            }
        }

        public SimpleSaveFile CreateEmptySave()
        {
            return SimpleSaveFile.Empty(_serializer);
        }
    }
}