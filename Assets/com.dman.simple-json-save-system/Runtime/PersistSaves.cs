
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Dman.SimpleJson
{
    /// <summary>
    /// Extensions used to create, load, and save SaveData objects.
    /// </summary>
    public static class PersistSavesExtensions
    { 
        public static void PersistSaveTo(this IPersistText persistText, string file, SaveData saveData)
        {
            using var writer = persistText.WriteTo(file);
            using var jsonWriter = new JsonTextWriter(writer);
            JsonSaveSystemSettings.Serializer.Serialize(jsonWriter, saveData.SavedToken);
        }
        
        [CanBeNull]
        public static SaveData LoadSaveFrom(this IPersistText persistText, string file)
        {
            using var reader = persistText.ReadFrom(file);
            if (reader == null) return null;
            using var jsonReader = new JsonTextReader(reader);
            
            try
            {
                var data = JObject.Load(jsonReader);
                return SaveData.Loaded(data);
            }
            catch (JsonException e)
            {
                Debug.LogError($"Failed to load data for {file}.json, malformed Json. Raw json: {reader.ReadToEnd()}");
                Debug.LogException(e);
                return null;
            }
        }
    }
}