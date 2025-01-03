using Dman.Utilities.Logger;
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
        public static void PersistSave(this IPersistText persistText, string file, SaveData saveData, JsonSerializer serializer)
        {
            using var writer = persistText.WriteTo(file);
            using var jsonWriter = new JsonTextWriter(writer);
            serializer.Serialize(jsonWriter, saveData.SavedToken);
        }
        
        [CanBeNull]
        public static SaveData LoadSave(this IPersistText persistText, string file)
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
                Log.Error($"Failed to load data for {file}.json, malformed Json. Raw json: {reader.ReadToEnd()}");
                Debug.LogException(e);
                return null;
            }
        }
    }
}